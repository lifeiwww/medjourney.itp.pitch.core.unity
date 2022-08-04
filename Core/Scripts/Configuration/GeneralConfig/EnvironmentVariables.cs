using System;

namespace dreamcube.unity.Core.Scripts.Configuration.GeneralConfig
{
    public class EnvironmentVariables
    {
        public GeneralSettings GetEnvironmentVariables(GeneralSettings settings)
        {
            
            var value = Environment.GetEnvironmentVariable("AUTO_RESTORE_STATE");
     
            value = Environment.GetEnvironmentVariable("DREAM_CUBE");
            if (!string.IsNullOrEmpty(value))
                settings.DreamCube = value;

            value = Environment.GetEnvironmentVariable("LOG_DIRECTORY");
            if (!string.IsNullOrEmpty(value))
                settings.LogDirectory = value;

            value = Environment.GetEnvironmentVariable("LOG_LEVEL");
            if (!string.IsNullOrEmpty(value))
                settings.LogLevel = value;

            value = Environment.GetEnvironmentVariable("LOG_NAME");
            if (!string.IsNullOrEmpty(value))
                settings.LogName = value;

            value = Environment.GetEnvironmentVariable("RTLS_PORT");
            if (!string.IsNullOrEmpty(value))
            {
                var success = int.TryParse(value, out var number);
                if (success) settings.RTLSPort = number;
            }

            value = Environment.GetEnvironmentVariable("RTLS_LOCAL_IP");
            if (!string.IsNullOrEmpty(value))
                settings.RTLSLocalIP = value;

            value = Environment.GetEnvironmentVariable("RTLS_REMOTE_IP");
            if (!string.IsNullOrEmpty(value))
                settings.RTLSRemoteIP = value;

            value = Environment.GetEnvironmentVariable("HEARTBEAT_IP");
            if (!string.IsNullOrEmpty(value))
                settings.HeartbeatIP = value;

            value = Environment.GetEnvironmentVariable("HEARTBEAT_PORT");
            if (!string.IsNullOrEmpty(value))
            {
                var success = int.TryParse(value, out var number);
                if (success) settings.HeartbeatPort = number;
            }


            return settings;
        }
    }
}