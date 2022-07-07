﻿using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;

namespace manutd
{
    public class SerilogTest : MonoBehaviour
    {
        private void Start()
        {
            // global configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Unity3D()
                .CreateLogger();

            // some logs
            Log.Information("Logging information");
            Log.Warning("Logging a warning");
            Log.Error("Logging an error");
        }
    }
}