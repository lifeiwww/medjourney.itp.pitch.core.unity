using System.Collections;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace dreamcube.unity.Core.TestsRoot.PlayMode
{
    public class RTLSConnectivityTest
    {
        [UnityTest]
        public IEnumerator RTLSConnected()
        {
            var configInstance = ConfigManager.Instance;
            var rtlsReciverComponent = RTLSReciverComponent.Instance;
            Assert.IsNotNull(Object.FindObjectOfType<RTLSReciverComponent>());
            if (ConfigManager.Instance.generalSettings.UseRTLS)
            {    
                yield return new WaitForSeconds(2);
                Assert.Greater(0, RTLSReciverComponent.RTLSfps);
            }

            yield return null;
        }
    }
}
