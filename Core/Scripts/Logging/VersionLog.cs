using System;
using System.Diagnostics;
using Serilog;
using UnityEngine;

namespace Assets.dreamcube.itp.pitch.core.unity.Core.Scripts.Logging
{
    public class VersionLog : MonoBehaviour
    {
        [NonSerialized] public static string ProductVersion = "";

        private void Start()
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule == null) return;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(processModule.FileName);
            ProductVersion = fileVersionInfo.ProductVersion;
            Log.Information($"[APP] Version {ProductVersion}");
        }
    }
}