using System;
using System.Collections.Generic;
using System.IO;
using dreamcube.unity.Core.Scripts.General;
using dreamcube.unity.Core.Scripts.Util;
using Serilog;
using UnityEngine;
using UniRx;


namespace dreamcube.unity.Core.Scripts.Configuration.GeneralConfig
{

    public static class DerivedSettings
    {
        public static string DreamcubeStateDataUrl { get; set; }
        public static string GameRoundDataUrl { get; set; }
        public static string ContentServiceScoringUrl { get; set; }
        public static string ContentServiceCategoriesUrl { get; set; }
        public static string ContentServiceActivitiesUrl { get; set; }
        public static string ContentServiceScreensUrl { get; set; }
    }

    public class ConfigManager : Singleton<ConfigManager>
    {

        public static readonly ReactiveProperty<bool> LoadingComplete = new ReactiveProperty<bool>();


        // settings holder
        [HideInInspector]
        public GeneralSettings generalSettings = new GeneralSettings();

        // we want to load these settings before log loading, so going to store some info during load
        public List<string> logs = new List<string>();

        public bool configDidLoad;

        // initialize the manager
        protected override void Awake()
        {
            base.Awake();
            var pathToUse = Common.CONFIG_FILE;


            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            if (!File.Exists(Path.Combine(Application.streamingAssetsPath, pathToUse)))
            {
                // save file with default settings
                SaveConfigurationFile(pathToUse);
            }

            configDidLoad = LoadGeneralSettings(Extensions.LoadStringFromFile(pathToUse));
            logs.Add($"Load config file {Path.Combine(Application.streamingAssetsPath, pathToUse)}");
            if (configDidLoad) SetDerivedSettings();

            // Save the configuration file if fields are missing or reading frpm file failed
            SaveConfigurationFile(Common.CONFIG_FILE);
            LoadingComplete.Value = true;
        }

        private void SaveConfigurationFile(string pathToUse)
        {
            var settingsJsonString = JsonUtility.ToJson(generalSettings, true);
            var savePath = Path.Combine(Application.streamingAssetsPath, pathToUse);
            logs.Add($"Saving local configuration file to {savePath}");
            File.WriteAllText(savePath, settingsJsonString);
        }

        private void SetDerivedSettings()
        {
            DerivedSettings.DreamcubeStateDataUrl =
                generalSettings.StateApiBaseUrl + generalSettings.ApiGcmStateName;

            DerivedSettings.GameRoundDataUrl =
                generalSettings.StateApiBaseUrl + generalSettings.ApiGrdStateName;

            DerivedSettings.ContentServiceCategoriesUrl =
                generalSettings.ContentServiceBaseUrl + generalSettings.ApiContentCategoriesName;

            DerivedSettings.ContentServiceActivitiesUrl =
                generalSettings.ContentServiceBaseUrl + generalSettings.ApiContentActivitiesName;

            DerivedSettings.ContentServiceScreensUrl =
                generalSettings.ContentServiceBaseUrl + generalSettings.ApiContentScreensName;

            DerivedSettings.ContentServiceScoringUrl = 
                generalSettings.ScoringServiceBaseUrl + generalSettings.ApiScoringSuffix;

        }

        // since the order of the script execution is specified, this will come after LogConfig
        private void Start()
        {
            Log.Information("Stored logs from Config Manager:");
            foreach (var x in logs) Log.Information($"-{x}");
        }

        private bool LoadGeneralSettings(string dataString)
        {
            try
            {
                // Override the default settings
                JsonUtility.FromJsonOverwrite(dataString, generalSettings);

                // Override with environment variables 
                generalSettings = new EnvironmentVariables().GetEnvironmentVariables(generalSettings);
                return true;
            }
            catch (Exception ex)
            {
                logs.Add($"Reading configuration from file failed {ex.Message}");
                return false;
            }

        }
    }
}