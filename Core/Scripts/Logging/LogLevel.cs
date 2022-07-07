using Serilog.Core;
using Serilog.Events;

namespace manutd
{
    internal static class LogLevel
    {
        public static LoggingLevelSwitch LevelSwitch = new LoggingLevelSwitch
        {
            MinimumLevel = LogEventLevel.Information
        };

        public static void SetLevelSwitch(string level)
        {
            switch (level.ToLower())
            {
                case "verbose":
                    LevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                    break;
                case "debug":
                    LevelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;
                case "information":
                    LevelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;
                case "warning":
                    LevelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;
                case "error":
                    LevelSwitch.MinimumLevel = LogEventLevel.Error;
                    break;
                case "fatal":
                    LevelSwitch.MinimumLevel = LogEventLevel.Fatal;
                    break;
            }
        }
    }
}