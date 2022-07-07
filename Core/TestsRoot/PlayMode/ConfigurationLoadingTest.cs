using System.Collections;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace dreamcube.unity.Core.TestsRoot.PlayMode
{
    public class ConfigurationLoadingTest
    {
        [UnityTest]
        public IEnumerator ConfigurationLoaded()
        {
            var configManager = ConfigManager.Instance;
            Assert.IsNotNull(Object.FindObjectOfType<ConfigManager>());
            yield return null;
            Assert.AreEqual(true, ConfigManager.Instance.configDidLoad);
            yield return null;
        }
    }
}
