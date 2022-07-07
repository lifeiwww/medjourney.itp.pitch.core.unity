using System.IO;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using manutd;
using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Logging
{
    public class LogConfig : MonoBehaviour
    {
        private string _directory = Common.LOG_DIRECTORY;
        private string _filename = Common.LOG_NAME;
        private string _level = Common.LOG_LEVEL;

        private void Start()
        {
            // load directory from config manager
            var configDirectory = ConfigManager.Instance.generalSettings.LogDirectory;
            if (string.IsNullOrEmpty(configDirectory) == false) _directory = configDirectory;

            // load minimum log level from config manager
            var configLevel = ConfigManager.Instance.generalSettings.LogLevel;
            if (string.IsNullOrEmpty(configLevel) == false) _level = configLevel;

            // load log name config manager
            var fileName = ConfigManager.Instance.generalSettings.LogName;
            if (string.IsNullOrEmpty(fileName) == false) _filename = fileName;

            // set log path
            var path = Directory.GetParent(Application.dataPath).FullName;
            var filepath = Path.Combine(Path.Combine(path, _directory ?? "Logs"), _filename);

            // set log level
            LogLevel.SetLevelSwitch(_level);

            // global configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(LogLevel.LevelSwitch)
#if UNITY_EDITOR
                .WriteTo.Unity3D()
#endif
                .WriteTo.File(filepath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 31)
                .CreateLogger();

            // global catch for unity logs
            UnityLog.ConvertToSerilog();

            // log configuration complete
            Log.Information("[APP] Initialized");
        }
    }
}