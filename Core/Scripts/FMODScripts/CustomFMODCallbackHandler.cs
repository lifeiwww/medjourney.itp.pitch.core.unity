using System;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using FMOD;
using FMODUnity;
using Serilog;
using UnityEngine;

[CreateAssetMenu(menuName = "New FMOD Callback Handler")]
public class CustomFMODCallbackHandler : PlatformCallbackHandler
{
    public SPEAKERMODE CustomSpeakerMode = SPEAKERMODE.QUAD;
    public int SampleRate = 48000;


    public override void PreInitialize(FMOD.Studio.System studioSystem, Action<RESULT, string> reportResult)
    {
        RESULT result;
        FMOD.System coreSystem;
        result = studioSystem.getCoreSystem(out coreSystem);
        reportResult(result, "studioSystem.getCoreSystem");

        coreSystem.getOutput(out var output);
        Log.Debug($"output is set to {output}");

        reportResult(result, "studioSystem.getCoreSystem");

        if (Enum.TryParse(ConfigManager.Instance.generalSettings.AudioSpeakerMode, out CustomSpeakerMode))
        {
            Log.Debug($"FMOD speakermode is {CustomSpeakerMode}");
            Log.Debug($"FMOD samplerate is {ConfigManager.Instance.generalSettings.AudioSampleRate}");
        }

        if (ConfigManager.Instance.generalSettings.UseASIO)
        {
            coreSystem.setOutput(OUTPUTTYPE.ASIO);
            //coreSystem.setSoftwareFormat(48000, SPEAKERMODE.QUAD, 0);
            coreSystem.setSoftwareFormat(ConfigManager.Instance.generalSettings.AudioSampleRate, CustomSpeakerMode, 0);

            // Adding setting for buffer size/count
            coreSystem.setDSPBufferSize((uint) ConfigManager.Instance.generalSettings.DSPBufferLength,
                ConfigManager.Instance.generalSettings.DSPBufferCount);
            Log.Debug(
                $"FMOD setting DSP buffer to {ConfigManager.Instance.generalSettings.DSPBufferLength}|{ConfigManager.Instance.generalSettings.DSPBufferCount} ");

            var numDrivers = 0;
            result = coreSystem.getNumDrivers(out numDrivers);
            Log.Debug($"FMOD reporting number of drivers {numDrivers}");

            var autoDetectIndex = -1;
            var autoDetectName = "";

            Log.Debug("Enumerating FMOD ASIO drivers");
            for (var i = 0; i < numDrivers; i++)
            {
                var name = GetDeviceName(coreSystem, i);
                Log.Debug($"FMOD driver index: {i} name: {name}");

                if (string.IsNullOrWhiteSpace(ConfigManager.Instance.generalSettings.AsioAutoDetectName) == false)
                    if (name == ConfigManager.Instance.generalSettings.AsioAutoDetectName)
                    {
                        autoDetectIndex = i;
                        autoDetectName = name;
                    }
            }

            if (autoDetectIndex > -1)
            {
                var name = GetDeviceName(coreSystem, autoDetectIndex);
                Log.Debug($"Setting auto-detect driver, index {autoDetectIndex} name: {name}");
                result = coreSystem.setDriver(autoDetectIndex);
            }
            else
            {
                var configIndex = ConfigManager.Instance.generalSettings.ASIODriverIndex;
                var name = GetDeviceName(coreSystem, configIndex);
                Log.Debug($"Setting configured driver, index {configIndex} name: {name}");
                result = coreSystem.setDriver(configIndex);
            }
        }
    }

    private string GetDeviceName(FMOD.System coreSystem, int index)
    {
        string name;
        var namelen = 1024;
        Guid guid;
        int systemrate;
        SPEAKERMODE speakermode;
        int speakermodechannels;
        coreSystem.getDriverInfo(index, out name, namelen, out guid, out systemrate, out speakermode,
            out speakermodechannels);
        return name;
    }
}