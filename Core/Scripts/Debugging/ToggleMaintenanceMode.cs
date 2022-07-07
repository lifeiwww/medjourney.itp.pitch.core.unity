using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client;
using dreamcube.unity.Core.Scripts.Util;
using OdinSerializer.Utilities;
using Serilog;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Debugging
{
    public class ToggleMaintenanceMode : MonoBehaviour
    {
        // perhaps use reactive props here? 
        // public ReactiveProperty<bool> isMaintenanceModeTestReactiveProperty = new ReactiveProperty<bool>(false);

        [SerializeField] private GameObject floorPattern;
        [SerializeField] private CanvasGroup WallPattern;

        private bool _currentMode = false;

        private void Awake()
        {
            EventManager.Instance.StartListening(EventStrings.EventOnSignalRMessage, EventHandler);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance.SafeIsUnityNull()) return;

            EventManager.Instance.StopListening(EventStrings.EventOnSignalRMessage, EventHandler);
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.M))
        //    {
        //        ToggleMaintenanceDisplay();
        //    }
        //}

        public void ToggleMaintenanceDisplay()
        {
            Log.Debug($"ToggleMaintenanceMode {WallPattern.gameObject.activeInHierarchy}");
            _currentMode = !_currentMode;
            floorPattern.SetActive(_currentMode);
            WallPattern.gameObject.SetActive(_currentMode);
        }

        public void ActivateMaintenanceDisplay(bool isActive)
        {
            Log.Debug($"ToggleMaintenanceMode {WallPattern.gameObject.activeInHierarchy}");
            _currentMode = isActive;
            floorPattern.SetActive(isActive);
            WallPattern.gameObject.SetActive(isActive);
        }

        private void EventHandler(string theEvent, string msg, GameObject sender, DataModelBase data)
        {
            if (theEvent == EventStrings.EventOnSignalRMessage && msg == SignalRIn.NotifyOnTestMode)
            {
                BoolData action = Extensions.Cast(data, typeof(BoolData));
                ActivateMaintenanceDisplay(action.ABool);
            }
        }
    }
}
