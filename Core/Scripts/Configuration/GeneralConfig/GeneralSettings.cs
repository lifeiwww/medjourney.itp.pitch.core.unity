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
        public string SignalRUri = Common.SIGNALR_URI;

        // CMS 
        public string CMSDownloadFolder = Common.CMS_DOWNLOAD_FOLDER;
        public string CMSDataFolder = Common.CMS_DATA_FOLDER;

        // API settings
        public bool UseAPI = Common.USE_API;
        public bool ContinuousPolling = Common.CONTINUOUS_POLLING;

        public string StateApiBaseUrl = Common.STATE_API_BASE_URL;
        public string ContentServiceBaseUrl = Common.CONTENT_SERVICE_BASE_URL;
        public string ScoringServiceBaseUrl = Common.SCORING_API_BASE_URL;

        public string ApiGcmStateName = Common.API_GCM_STATE_NAME;
        public string ApiGrdStateName = Common.API_GRD_STATE_NAME;
        public string ApiScoringSuffix = Common.SCORES_API_SUFFIX;

        public string ApiContentCategoriesName = Common.API_CONTENT_CATEGORIES_NAME;
        public string ApiContentActivitiesName = Common.API_CONTENT_ACTIVITIES_NAME;
        public string ApiContentScreensName = Common.API_CONTENT_SCREENS_NAME;

        //public string DreamcubeStateDataUrl = Common.STATE_API_BASE_URL + Common.API_GCM_STATE_NAME;
        //public string GameRoundDataUrl = Common.STATE_API_BASE_URL + Common.API_GRD_STATE_NAME;
        //public string ContentServiceCategoriesUrl = Common.CONTENT_SERVICE_BASE_URL + Common.API_CONTENT_CATEGORIES_NAME;
        //public string ContentServiceActivitiesUrl = Common.CONTENT_SERVICE_BASE_URL + Common.API_CONTENT_ACTIVITIES_NAME;
        //public string ContentServiceScreensUrl = Common.CONTENT_SERVICE_BASE_URL + Common.API_CONTENT_SCREENS_NAME;

        public override string ToString()
        {
            // hide secret when printing
            var obj = MemberwiseClone() as GeneralSettings;
            //if (obj != null) obj.CMSClientSecret = "#####";
            return JsonUtility.ToJson(obj);
        }


    }
}