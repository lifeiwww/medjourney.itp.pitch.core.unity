using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.Util;
using OdinSerializer.Utilities;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Debugging
{
    public class ToggleMaintenanceMode : MonoBehaviour
    {
        // perhaps use reactive props here? 
        // public ReactiveProperty<bool> isMaintenanceModeTestReactiveProperty = new ReactiveProperty<bool>(false);

        [SerializeField] private GameObject floorPattern;
        [SerializeField] private CanvasGroup WallPattern;

        private bool _currentMode = true;

        private void Awake()
        {
            ToggleMaintenanceDisplay();
        }

        private void OnDestroy()
        {
            if (EventManager.Instance.SafeIsUnityNull()) return;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
                ToggleMaintenanceDisplay();
        }

        public void ToggleMaintenanceDisplay()
        {
            Debug.Log($"ToggleMaintenanceMode {WallPattern.gameObject.activeInHierarchy}");
            _currentMode = !_currentMode;
            floorPattern.SetActive(_currentMode);
            WallPattern.gameObject.SetActive(_currentMode);
        }

        public void ActivateMaintenanceDisplay(bool isActive)
        {
            Debug.Log($"ToggleMaintenanceMode {WallPattern.gameObject.activeInHierarchy}");
            _currentMode = isActive;
            floorPattern.SetActive(isActive);
            WallPattern.gameObject.SetActive(isActive);
        }
    }
}
