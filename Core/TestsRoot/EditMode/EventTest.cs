using System.Collections;
using dreamcube.unity.Core.Scripts.Signals.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace dreamcube.unity.Core.TestsRoot.EditMode
{
    public class EditorEventTest
    {
        private bool _eventReceived;

        [OneTimeSetUp]
        public void Setup()
        {
            var manager = EventManager.Instance;
            EventManager.Instance.StartListening(EventStrings.EventOnSignalRMessage, EventHandler);
        }

        [UnityTest]
        public IEnumerator EditorEventReceived()
        {
            Assert.IsNotNull(Object.FindObjectOfType<EventManager>());
            EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage);
            yield return null;
            Assert.IsTrue(_eventReceived);
        }

        private void EventHandler(string theEvent, string msg, GameObject sender)
        {
            if (theEvent == EventStrings.EventOnSignalRMessage)
                _eventReceived = true;
        }

    }
}
