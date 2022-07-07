using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using UnityEngine;

namespace manutd
{
    public class HeartbeatComponent : MonoBehaviour
    {
        private bool _heartbeatEnabled;
        private HeartbeatService _heartbeatService;

        private void Awake()
        {
            var ip = ConfigManager.Instance.generalSettings.HeartbeatIP;
            var port = ConfigManager.Instance.generalSettings.HeartbeatPort;
            _heartbeatEnabled = ConfigManager.Instance.generalSettings.UseHeartbeat;
            if (_heartbeatEnabled) _heartbeatService = new HeartbeatService(ip, port);
        }

        private void Start()
        {
            if (_heartbeatEnabled) _heartbeatService?.Start();
        }

        private void OnApplicationQuit()
        {
            if (_heartbeatEnabled ) _heartbeatService?.Stop();
        }
    }
}