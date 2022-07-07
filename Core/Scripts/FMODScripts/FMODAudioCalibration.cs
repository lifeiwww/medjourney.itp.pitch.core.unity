using System.Diagnostics;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Serilog;
using TMPro;
using UnityEngine;
using EventInstance = FMOD.Studio.EventInstance;


namespace dreamcube.unity.Core.Scripts.FMODScripts
{
    public class FMODAudioCalibration : MonoBehaviour
    {
        private bool isPlayingPinkNoise;

        [EventRef] public string musicEventRearLeft = "";

        [EventRef] public string musicEventRearRight = "";

        [EventRef] public string musicEventPinkNoise = "";

        // test events and their control instances
        [EventRef] public string musicEventFrontLeft = "";

        [EventRef] public string musicEventFrontRight = "";

        [SerializeField] private TextMeshProUGUI textGui;

        // soundscape audio instance
        private EventInstance PinkNoiseInstance;

        private void Awake()
        {
            PinkNoiseInstance = RuntimeManager.CreateInstance(musicEventPinkNoise);
            SetText();
        }

        private void SetText()
        {
            if (!textGui) return;

            var text = "---- audio test shortcuts ----\n\n";
            text += "F1 - FL\n";
            text += "F2 - FR\n";
            text += "F3 - RL\n";
            text += "F4 - RR\n";
            text += "N - Toggle PinkNoise\n";
            textGui.text = text;
        }


        private void OnDestroy()
        {
            PinkNoiseInstance.release();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!ConfigManager.Instance.generalSettings.Debug) return;

            if (Input.GetKeyDown(KeyCode.F1)) RuntimeManager.PlayOneShot(musicEventFrontLeft);

            if (Input.GetKeyDown(KeyCode.F2)) RuntimeManager.PlayOneShot(musicEventFrontRight);

            if (Input.GetKeyDown(KeyCode.F3)) RuntimeManager.PlayOneShot(musicEventRearLeft);

            if (Input.GetKeyDown(KeyCode.F4)) RuntimeManager.PlayOneShot(musicEventRearRight);

            if (Input.GetKeyDown(KeyCode.F5)) RuntimeManager.PlayOneShot(musicEventPinkNoise);

            if (Input.GetKeyDown(KeyCode.N)) TogglePinkNoise();
        }


        private void TogglePinkNoise()
        {
            isPlayingPinkNoise = !isPlayingPinkNoise;
            if (isPlayingPinkNoise)
            {
                Log.Debug("Starting");
                PinkNoiseInstance.start();
            }
            else
            {
                Log.Debug("Stopping");
                PinkNoiseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        private void StopAllPlayerEvents()
        {
            var playerBus = RuntimeManager.GetBus("bus:/player");
            playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}