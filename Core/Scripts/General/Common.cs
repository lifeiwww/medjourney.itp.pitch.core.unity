// overridden in ConfigManager

public static class Common
{
    public const string CONFIG_FILE = "general_config.json";
    public const string SCENE_SETTINGS_MODIFIER_FILE = "scene_settings_modifier.json";
    public const string DEFAULT_QRCODE_FILE = "QRNotAvailable.png";

#if UNITY_EDITOR_OSX
    public const string CMS_DOWNLOAD_FOLDER = "Assets/StreamingAssets/CMSDownloads/";
#else
    public const string CMS_DOWNLOAD_FOLDER = "C:\\GameBay\\Media\\";
    public const string CMS_DATA_FOLDER = "C:\\GameBay\\Data\\";

#endif

    public const bool DEBUG = false;
    public const string GAME_BAY = "1";
    public const string LOG_DIRECTORY = "C:\\GameBay\\Logs";
    public const string LOG_LEVEL = "information";
    public const string LOG_NAME = "pitch-.txt";
    public const string RTLS_LOCAL_IP = "127.0.0.1";
    public const string RTLS_REMOTE_IP = "127.0.0.1";
    public const int RTLS_PORT = 8282;
    public const string SIGNALR_URI = "http://alibabamigrationtest.eastus.cloudapp.azure.com:8080/gameBayHub";
    public const bool USE_MULTICAST = false;
    public const bool USE_RTLS = false;
    public const bool USE_ASIO = false;

    public const int ASIO_DRIVER_INDEX = 0;
    public const string ASIO_AUTO_DETECT_NAME = "RedNet PCIe";

    public const int DSP_BUFFER_LENGTH = 128;
    public const int DSP_BUFFER_COUNT = 4;

    public const int SAMPLE_RATE = 48000;
    public const string SPEAKER_MODE = "QUAD";

    public const string HEARTBEAT_IP = "0.0.0.0";
    public const int HEARTBEAT_PORT = 13000;
    public const bool USE_HEARTBEAT = false;

    // ---- API vars ---- //
    public const bool USE_API = true;
    public const bool CONTINUOUS_POLLING = true;

    // state API addresses
    public const string API_GCM_STATE_NAME = "gcm";
    public const string API_GRD_STATE_NAME = "grd";
    public const string STATE_API_BASE_URL = "http://dreamcube.lp-docs.com:5556/api/v1/";

    // scoring API addresses
    public const string SCORES_API_SUFFIX = "score";
    public const string SCORING_API_BASE_URL = "http://dreamcube.lp-docs.com:5556/api/v1/";

    // content API addresses
    public const string API_CONTENT_CATEGORIES_NAME = "categories";
    public const string API_CONTENT_ACTIVITIES_NAME = "activities";
    public const string API_CONTENT_SCREENS_NAME = "screens";
    public const string CONTENT_SERVICE_BASE_URL = "http://dreamcube.lp-docs.com:5559/api/v1/";
    
    public const float OUTPUT_VOLUME = 1.0f;




}