using System;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Configuration.GeneralConfig
{
    [Serializable]
    public class GeneralSettings
    {
        public string AsioAutoDetectName = Common.ASIO_AUTO_DETECT_NAME;
        public int ASIODriverIndex = Common.ASIO_DRIVER_INDEX;

        // new settings
        public int AudioSampleRate = Common.SAMPLE_RATE;
        public string AudioSpeakerMode = Common.SPEAKER_MODE;
        public int DSPBufferCount = Common.DSP_BUFFER_COUNT;
        public int DSPBufferLength = Common.DSP_BUFFER_LENGTH;
        public bool Debug = Common.DEBUG;
        public string DreamCube = Common.GAME_BAY;
        public string LogDirectory = Common.LOG_DIRECTORY;
        public string LogLevel = Common.LOG_LEVEL;
        public string LogName = Common.LOG_NAME;
        public string RTLSLocalIP = Common.RTLS_LOCAL_IP;
        public int RTLSPort = Common.RTLS_PORT;
        public string RTLSRemoteIP = Common.RTLS_REMOTE_IP;
        public bool UseASIO = Common.USE_ASIO;
        public bool UseRTLS = Common.USE_RTLS;
        public bool UseMulticast = Common.USE_MULTICAST;
        public string HeartbeatIP = Common.HEARTBEAT_IP;
        public int HeartbeatPort = Common.HEARTBEAT_PORT;
        public bool UseHeartbeat = Common.USE_HEARTBEAT;
        public float Volume = Common.OUTPUT_VOLUME;

        public override string ToString()
        {
            // hide secret when printing
            var obj = MemberwiseClone() as GeneralSettings;
            //if (obj != null) obj.CMSClientSecret = "#####";
            return JsonUtility.ToJson(obj);
        }


    }
}