using UnityEngine;
using UnityEngine.Events;

namespace dreamcube.unity.Core.Scripts.Signals.Events
{
    public interface IEventManager
    {
        public void StartListening(string appEventType,
            UnityAction<string, string, GameObject> listener,
            MonoBehaviour mb = null);

        public void StopListening(string appEventType,
            UnityAction<string, string, GameObject> listener);

        public void TriggerEvent(string appEventType, string msg = "", GameObject obj = null,
              bool trackEvent = false, long value = 0, bool debug = false);
    }
}

