
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client;
using Google.Protobuf.Collections;
using RTLSProtocol;
using Serilog;
using SimpleJSON;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Components.RTLS
{
    public class RTLSReciverService
    {
        private SimpleUDPListener _link;
        private Vector3 _position = Vector3.zero;
        private ulong _previousFrameId;
        private bool _useRTLS;

        public bool NewData { get; set; }
        public static int NumCameras { get; set; } = 0;
        private static bool _isStartup = true;

        [SerializeField] public bool acceptMulticast;
        [SerializeField] public string localIP = "127.0.0.1";
        [SerializeField] private Vector3 offset = new Vector3(1000, 0, 0);

        [SerializeField] public int port = 8282;
        [SerializeField] public string remoteIP = "127.0.0.1";
        [SerializeField] public Vector3 scale = new Vector3(1, 1, 1);

        //[Required("signalRClient is necessary for this component.")]
        [SerializeField]
        private BaseSignalRClient signalRClient = null;

        [SerializeField] private int system = 2; // enum from rtls-protocol; 2 = motive
        [SerializeField] private bool throwawayUnorderedFrames;
        [SerializeField] private int yRotations = 3;


        public RTLSReciverService()
        {
            Setup();
        }

        private void Setup()
        {
            localIP = ConfigManager.Instance.generalSettings.RTLSLocalIP;
            remoteIP = ConfigManager.Instance.generalSettings.RTLSRemoteIP;
            port = ConfigManager.Instance.generalSettings.RTLSPort;
            acceptMulticast = ConfigManager.Instance.generalSettings.UseMulticast;
            _useRTLS = ConfigManager.Instance.generalSettings.UseRTLS;

            if (_useRTLS == false) return;
            _link = new SimpleUDPListener(localIP, remoteIP, port, acceptMulticast);
            _link.DataReceived += Link_DataReceived;


            if (signalRClient == null)
            {
                signalRClient = GameObject.FindObjectOfType<BaseSignalRClient>();
            }

            Log.Debug($"Starting RTLS receiver local: {localIP}:{port} remote: {localIP}:{port}");
        }

        protected internal void Close()
        {
            if (_useRTLS == false) return;
            Log.Debug("Quitting RTLS Service");
            _link?.Close();
        }

        public Vector3 GetPosition()
        {
            return _position;
        }



        private void Link_DataReceived(object sender, EventArgs e)
        {
            NewData = true;
            var response = sender as byte[];
            ParseData(response);
        }

        private bool ParseData(byte[] serialized)
        {
            // Attempt to parse the frame
            TrackableFrame frame;
            try
            {
                frame = TrackableFrame.Parser.ParseFrom(serialized); //, 0, serialized.Length);
            }
            catch (Exception e)
            {
                Log.Error("TrackableFrame.Parser error " + e.Message);
                return false;
            }

            // Do not extract data from this frame is there is no context
            if (string.IsNullOrEmpty(frame.Context.ToStringUtf8())) return false;

            // Attempt to parse the frame's context
            JSONNode contextNode;
            try
            {
                contextNode = JSON.Parse(frame.Context.ToStringUtf8());
            }
            catch (Exception e)
            {
                Log.Error($"JSON.Parse error of frame.Context {e.Message}");
                return false;
            }

            // Confirm that this data is coming the correct type of tracking system
            if (contextNode["s"] != system) return false;

            // If this is an old (missed) frame, ignore it
            if (throwawayUnorderedFrames && frame.FrameID < _previousFrameId) return false;
            _previousFrameId = frame.FrameID;

            // If this data has no type, skip it
            if (contextNode["t"] == null) return false;

            // What kind of data is this?
            int frameType = contextNode["t"];
            switch (frameType)
            {
                case 0: // markers (ball)

                    // Pass the position of the first trackable
                    // TODO: This choice shouldn't be arbitrary
                    var trackable = frame.Trackables[0];
                    _position.x = (float)trackable.Position.X;
                    _position.y = (float)trackable.Position.Y;
                    _position.z = (float)trackable.Position.Z;
                    // Apply a rotation around the up vector (Y)
                    for (var i = 0; i < yRotations; i++)
                    {
                        var tmp = _position.x;
                        _position.x = -_position.z;
                        _position.z = tmp;
                    }

                    // Offset the position by some amount
                    _position += offset;

                    // Scale coordinates according to site specs
                    _position.x *= scale.x;
                    _position.y *= scale.y;
                    _position.z *= scale.z;

                    break;
                case 1: // reference objects (cameras)

                    // Check each camera to see if any are misaligned.
                    var misalignedCameras = new List<string>();
                    foreach (var trk in frame.Trackables)
                    {
                        var isAligned = CheckCameraStatus(trk);
                        if (!isAligned)
                            misalignedCameras.Add("#" + trk.Id + " [Serial " + trk.Cuid.ToStringUtf8() + "]");
                    }

                    // Print debug message about misalignment
                    if (misalignedCameras.Count > 0)
                    {
                        var message = "Camera System may require calibration.";
                        message += " Misaligned cameras may include: " + string.Join(", ", misalignedCameras);
                        Log.Debug(message);
                    }

                    // Check if any cameras have dropped
                    var numCams = frame.Trackables.Count;
                    if (numCams != NumCameras)
                    {
                        var message = "Camera System: # cameras changed from " + NumCameras + " to " + numCams;
                        Log.Debug(message);
                    }

                    // send diagnostic information about the cameras if status has changed
                    if (numCams != NumCameras || misalignedCameras.Count > 0 || _isStartup)
                    {
                        SendCameraStatus(frame.Trackables);
                        NumCameras = numCams;
                        _isStartup = false;
                    }

                    break;
            }

            return true;
        }

        private bool CheckCameraStatus(Trackable camTrackable)
        {
            var isAligned = true;
            JSONNode trackableNode = null;
            try
            {
                trackableNode = JSON.Parse(camTrackable.Context.ToStringUtf8());
            }
            catch (Exception e)
            {
                Log.Error($"JSON.Parse error of frame.Trackables[i].Context {e.Message}");
                isAligned = false;
            }

            if (trackableNode["m"] == 1) isAligned = false;

            return isAligned;
        }

        private void SendCameraStatus(RepeatedField<Trackable> trackables)
        {
            var gameBayIDIsNumber = int.TryParse(ConfigManager.Instance.generalSettings.DreamCube, out var gameBayID);
            var camDataList = new List<CameraData>();

            foreach (var cam in trackables)
            {
                var camData = new CameraData
                {
                    CameraAlignmentStatus = CheckCameraStatus(cam),
                    CameraID = cam.Id.ToString(),
                    CameraSerialNumber = cam.Cuid.ToStringUtf8()
                };
                camDataList.Add(camData);
            }

            var trackingData = new TrackingSystemData
            {
                GameBayID = gameBayID,
                Cameras = camDataList
            };

            Log.Information($"Sending camera data for {trackingData} cameras");

            if (signalRClient != null)
                Task.Run(() => { signalRClient.UpdateCameraStatus(trackingData, _isStartup); });
            else
                Log.Error("No signalR client found");
        }
    }
}