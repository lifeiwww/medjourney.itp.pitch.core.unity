using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.General;
using dreamcube.unity.Core.Scripts.Util;
using Google.Protobuf.Collections;
using manutd;
using RTLSProtocol;
using Serilog;
using SimpleJSON;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Components.RTLS
{
    public class RTLSReceiver : Singleton<RTLSReceiver>
    {
        public static int NumCameras;
        public static int NumTrackables;
        public static float RTLSfps;
        private SimpleUDPListener _link;
        private Vector3 _position = Vector3.zero;
        private ulong _previousFrameId;
        private bool _useRTLS;


        // to display frames per second
        private float _deltaTime;
        private bool _newData;
        private float _lastNewFrameRecievedTime;
        private static bool _isStartup = true;

        public GameObject indicator;
        [SerializeField] public bool acceptMulticast;
        [SerializeField] public string localIP = "127.0.0.1";
        [SerializeField] private Vector3 offset = new Vector3(1000, 0, 0);

        [SerializeField] public int port = 8282;
        [SerializeField] public string remoteIP = "127.0.0.1";
        [SerializeField] public Vector3 scale = new Vector3(1, 1, 1);

        [SerializeField] private int system = 2; // enum from rtls-protocol; 2 = motive
        [SerializeField] private bool throwawayUnorderedFrames;
        [SerializeField] private int yRotations = 3;

        [SerializeField] private GameObject _trackablePrefab;

        //private List<Trackable> trackableList = new List<Trackable>();
        private Dictionary<int, GameObject> _trackbleObjectDictionary = new Dictionary<int, GameObject>();

        private void Start()
        {
            Log.Debug("Set RTLS settings using config manager");
            localIP = ConfigManager.Instance.generalSettings.RTLSLocalIP;
            remoteIP = ConfigManager.Instance.generalSettings.RTLSRemoteIP;
            port = ConfigManager.Instance.generalSettings.RTLSPort;
            acceptMulticast = ConfigManager.Instance.generalSettings.UseMulticast;
            _useRTLS = ConfigManager.Instance.generalSettings.UseRTLS;

            if (_useRTLS == false) return;
            _link = new SimpleUDPListener(localIP, remoteIP, port, acceptMulticast);
            _link.DataReceived += Link_DataReceived;
            Log.Debug($"Starting RTLS receiver local: {localIP}:{port} remote: {localIP}:{port}");
        }

        private void Update()
        {
            if (indicator) indicator.transform.localPosition = _position;

            if (_newData)
            {
                // calculate RTLS FPS
                _lastNewFrameRecievedTime = Time.time;
                _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
                RTLSfps = 1.0f / _deltaTime;
                _newData = false;
            }



            // reset if no new data is received
            if (Time.time - _lastNewFrameRecievedTime > 5)
            {
                RTLSfps = 0;
                NumCameras = 0;
                NumTrackables = 0;
            }

        }

        protected override void OnApplicationQuit()
        {
            if (_useRTLS == false) return;
            Log.Debug("Quitting RTLS receiver");
            _link?.Close();
            base.OnApplicationQuit();
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        private void Link_DataReceived(object sender, EventArgs e)
        {
            _newData = true;
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
                    _position = CalculateTrackablePosition(trackable);

                    // create copy of trackables
                    var trackableList = new List<Trackable>(frame.Trackables);

                    // we must create game objects only on the main thread 
                    Dispatcher.RunOnMainThread(() => ProcessTrackables(trackableList));


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


        private void ProcessTrackables(List<Trackable> frameTrackables)
        {
            NumTrackables = frameTrackables.Count;
            List<int> trackableIds = new List<int>();
            for (var i = 0; i < frameTrackables.Count; i++)
            {
                trackableIds.Add(frameTrackables[i].Id);
                var t = frameTrackables[i];
                var pos = CalculateTrackablePosition(t);
                var id = t.Id;

                GameObject obj;
                if (_trackbleObjectDictionary.TryGetValue(id, out obj))
                {
                    obj.transform.localPosition = pos;
                    var trackableObject = obj.GetComponent<TrackableGameObject>();
                    trackableObject.Position = pos;
                    trackableObject.Age++;
                    trackableObject.IdTextMesh.text = $"ID: {id}\nage: {trackableObject.Age }\nacc: {t.Acceleration}\nangular: {t.AngularAcceleration}\nvel: {t.Velocity}\nsize: {t.CalculateSize()}";

                }
                else
                {
                    GameObject trkbl = Instantiate(_trackablePrefab, pos, Quaternion.identity.normalized, gameObject.transform);
                    trkbl.GetComponentInChildren<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV();
                    var trackableObject = trkbl.GetComponent<TrackableGameObject>();
                    trkbl.name = $"Trackable-{id}";
                    trackableObject.Id = id;
                    trackableObject.Position = pos;
                    _trackbleObjectDictionary.Add(id, trkbl);
                }
            }

            for (var i = 0; i < _trackbleObjectDictionary.Keys.Count; i++)
            {
                // this could be nicer if we wanted to add an effect of disappearance,
                // an effect could be spawned from the game object OnDestroy by a attaching a script to it

                var keyId = _trackbleObjectDictionary.ElementAt(i).Key;
                if (!trackableIds.Contains(_trackbleObjectDictionary.Keys.ElementAt(i)))
                {
                    // we can add a delay before destroying the object just for fun
                    // if we call a die function on the element
                    if (_trackbleObjectDictionary.TryGetValue(keyId, out var obj))
                    {
                        Destroy(obj);
                        _trackbleObjectDictionary.Remove(keyId);
                    }
                }
            }
        }

        private Vector3 CalculateTrackablePosition(Trackable trackable)
        {
            Vector3 pos = new Vector3();
            pos.x = (float)trackable.Position.X * 10;
            pos.y = (float)trackable.Position.Y * 10;
            pos.z = (float)trackable.Position.Z * 10;
            // Apply a rotation around the up vector (Y)
            for (var i = 0; i < yRotations; i++)
            {
                var tmp = pos.x;
                pos.x = -pos.z;
                pos.z = tmp;
            }

            // Offset the position by some amount
            pos += offset;

            // Scale coordinates according to site specs
            pos.x *= scale.x;
            pos.y *= scale.y;
            pos.z *= scale.z;
            return pos;
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
                Log.Error("JSON.Parse error of frame.Trackables[i].Context " + e.Message);
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
        }
    }
}