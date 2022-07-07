using System.Collections;
using dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace dreamcube.unity.Core.TestsRoot.PlayMode
{
    public class SignalRConnectivityTest
    {
        [UnityTest]
        public IEnumerator SignalRConnected()
        {
            var signalRInstance = BaseSignalRClient.Instance;
            Assert.IsNotNull(Object.FindObjectOfType<BaseSignalRClient>());
            //yield return new WaitForSeconds(2);
            //Assert.AreEqual("Server Connected", BaseSignalRClient.ServerHealth);
            yield return null;
        }
    }
}
