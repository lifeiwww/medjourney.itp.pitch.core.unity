using Serilog;
using UnityEngine;

namespace manutd
{
    internal class UnityLog
    {
        public static void ConvertToSerilog()
        {
            // catch Unity logs
            Application.logMessageReceived += (logString, stackTrace, type) =>
            {
#if UNITY_EDITOR
                // don't print twice when using Unity sink
                if (stackTrace.Contains("Serilog.Sinks.Unity3D")) return;

#endif
                // filter RI.Hid errors 
                if (logString.Contains("RI.Hid")) return;

                switch (type)
                {
                    case LogType.Error:
                        Log.Error(logString);
                        break;
                    case LogType.Warning:
                        Log.Warning(logString);
                        break;
                    case LogType.Assert:
                        Log.Debug(logString);
                        break;
                    case LogType.Log:
                        Log.Debug(logString);
                        break;
                    case LogType.Exception:
                        Log.Error(logString);
                        break;
                    default:
                        Log.Debug(logString);
                        break;
                }
            };
        }
    }
}