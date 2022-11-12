using System;
using System.Collections.Generic;
using System.Linq;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.General;
using dreamcube.unity.Core.Scripts.Util;
using Google.Protobuf.Collections;
using RTLSProtocol;
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
        [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

        [SerializeField] public int port = 8282;
        [SerializeField] public string remoteIP = "127.0.0.1";
        [SerializeField] public Vector3 scale = new Vector3(1, 1, 1);

        [SerializeField] private int system = 2; // enum from rtls-protocol; 2 = motive
        [SerializeField] private bool throwawayUnorderedFrames;
        [SerializeField] private int yRotations = 3;
        [SerializeField] private List<GameObject> _trackablePrefabs = new List<GameObject>();

        private Dictionary<string, GameObject> _trackbleObjectDictionary = new Dictionary<string, GameObject>();
        private int _currentPrefab;
        private void Start()
        {
            Debug.Log("Set RTLS settings using config manager");
            localIP = ConfigManager.Instance.generalSettings.RTLSLocalIP;
            remoteIP = ConfigManager.Instance.generalSettings.RTLSRemoteIP;
            port = ConfigManager.Instance.generalSettings.RTLSPort;
            acceptMulticast = ConfigManager.Instance.generalSettings.UseMulticast;
            _useRTLS = ConfigManager.Instance.generalSettings.UseRTLS;

            if (_useRTLS == false) return;
            _link = new SimpleUDPListener(localIP, remoteIP, port, acceptMulticast);
            _link.DataReceived += Link_DataReceived;
            Debug.Log($"Starting RTLS receiver local: {localIP}:{port} remote: {localIP}:{port}");
        }

        private void Update()
        {
            if (indicator != null) indicator.transform.localPosition = _position;

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
            Debug.Log("Quitting RTLS receiver");
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
                Debug.LogError("TrackableFrame.Parser error " + e.Message);
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
                Debug.LogError($"JSON.Parse error of frame.Context {e.Message}");
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

            var allData = frame.UnknownFields;
            //Debug.Log( $"Unknown fields {allData}");


            switch (frameType)
            {
                case 0: // markers (ball)

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
                        Debug.Log(message);
                    }

                    // Check if any cameras have dropped
                    var numCams = frame.Trackables.Count;
                    if (numCams != NumCameras)
                    {
                        var message = "Camera System: # cameras changed from " + NumCameras + " to " + numCams;
                        Debug.Log(message);
                    }

                    // send diagnostic information about the cameras if status has changed
                    if (numCams != NumCameras || misalignedCameras.Count > 0 || _isStartup)
                    {
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
            List<string> trackableIds = new List<string>();

            for (var i = 0; i < frameTrackables.Count; i++)
            {
                trackableIds.Add(frameTrackables[i].Cuid.ToStringUtf8());
                var t = frameTrackables[i];
                var pos = CalculateTrackablePosition(t);
                var id = t.Cuid.ToStringUtf8();

                GameObject obj;
                if (_trackbleObjectDictionary.TryGetValue(id, out obj))
                {
                    obj.transform.localPosition = pos;
                    var trackableObject = obj.GetComponent<TrackableGameObject>();
                    trackableObject.Position = pos;
                    trackableObject.Age++;

                    if (trackableObject.IdTextMesh != null)
                        trackableObject.IdTextMesh.text = $"ID: {id}\nage: {trackableObject.Age }\nsize: {t.CalculateSize()}";

                }
                else
                {

                    if (!_trackablePrefabs.Any()) return;

                    // get a prefab from the list and increment
                    var trackablePrefab = _trackablePrefabs[_currentPrefab];
                    _currentPrefab = (_currentPrefab + 1) % _trackablePrefabs.Count;

                    GameObject trkbl = Instantiate(trackablePrefab, pos, Quaternion.identity.normalized, gameObject.transform);
                    Color col = UnityEngine.Random.ColorHSV();
                    col.a = 0.7f;
                    trkbl.GetComponentInChildren<MeshRenderer>().materials.Last().color = col;
                    var trackableObject = trkbl.GetComponent<TrackableGameObject>();
                    var trackableCuid = t.Cuid.ToStringUtf8();
                    trkbl.name = $"Trackable-{trackableCuid}";
                    trackableObject.Id = t.Cuid.ToStringUtf8();
                    trackableObject.Position = pos;
                    _trackbleObjectDictionary.Add(trackableCuid, trkbl);
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
            pos.x = (float)trackable.Position.X;
            pos.y = (float)trackable.Position.Y;
            pos.z = (float)trackable.Position.Z;

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
                Debug.LogError("JSON.Parse error of frame.Trackables[i].Context " + e.Message);
                isAligned = false;
            }

            if (trackableNode["m"] == 1) isAligned = false;

            return isAligned;
        }
    }
}