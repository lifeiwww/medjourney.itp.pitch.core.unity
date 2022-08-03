using System;
using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.General;
using UnityEngine;
using UnityEngine.Events;

namespace dreamcube.unity.Core.Scripts.Signals.Events
{
    [Serializable]
    public class AppEvent : UnityEvent<string, string, GameObject> { }

    public class EventManager : Singleton<EventManager>, IEventManager
    {
        // private dictionary to hold the events 
        private readonly Dictionary<string, AppEvent> _appEventDictionary = new Dictionary<string, AppEvent>();

        // filter duplicate events 
        private const bool UseFilter = false;
        private const float FilterTime = 250; //ms

        private readonly Dictionary<Tuple<string, string>, DateTime> _processedEvents =
            new Dictionary<Tuple<string, string>, DateTime>();

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            Clean();
            Debug.Log("[APP] Quit");
        }

        private void Clean()
        {
            Debug.Log("Cleaning");
            foreach (var thisEvent in _appEventDictionary.Values)
            {
                thisEvent.RemoveAllListeners();
            }

            _appEventDictionary.Clear();
        }

        public void StartListening(string eventType,
            UnityAction<string, string, GameObject> listener,
            MonoBehaviour mb = null)
        {
            AppEvent thisEvent = null;
            if (Instance._appEventDictionary.TryGetValue(eventType, out thisEvent))
            {
                thisEvent?.AddListener(listener);
            }
            else
            {
                thisEvent = new AppEvent();
                thisEvent.AddListener(listener);
                Instance._appEventDictionary.Add(eventType, thisEvent);
                //Log.Debug("STARTED LISTENING TO: " + eventType.ToString() );
            }
        }

        public void StopListening(string appEventType,
            UnityAction<string, string, GameObject> listener)
        {
            if (!Instance)
                return;

            if (Instance._appEventDictionary.TryGetValue(appEventType, out var thisEvent))
            {
                thisEvent.RemoveListener(listener);
                //Log.Debug("Stopped listening: " + eventName);
            }
        }

        public void TriggerEvent(string appEventType, string msg = "", GameObject obj = null,
             bool trackEvent = false, long value = 0, bool debug = false)
        {
            AppEvent thisEvent = null;
            if (Instance._appEventDictionary.TryGetValue(appEventType, out thisEvent))
            {

                // filter duplicate events
                if (UseFilter && FilterEvents(appEventType, msg, FilterTime))
                    return;

                thisEvent.Invoke(appEventType, msg, obj);
                if (debug)
                {
                    var message = $"TRIGGERING EVENT: {appEventType}";
                    if (obj != null)
                    {
                        message += $"\nCalled by: {obj}";
                    }

                    Debug.Log(message);
                }
                //else
                //{
                //    Log.Information($"[EVENT] {appEventType} {msg}");
                //}
            }
        }

        private bool FilterEvents(string appEventType, string msg, float timeWindow)
        {
            var processed = Instance._processedEvents;

            // events NOT filtered
            if (appEventType == EventStrings.EventOnBadScore ||
                appEventType == EventStrings.EventOnGoodScore
            ) return false;

            var key = new Tuple<string, string>(appEventType, msg);

            if (processed.ContainsKey(key))
            {
                var timeSpan = DateTime.Now - processed[key];
                if (timeSpan.TotalMilliseconds <= timeWindow)
                {
                    Debug.Log($"[FILTERED EVENT] {appEventType} {msg}");
                    return true;
                }
            }

            processed[key] = DateTime.Now;
            return false;
        }
    }
}