using System.Collections;
using dreamcube.unity.Core.Scripts.AssetLoading;
using dreamcube.unity.Core.Scripts.Components.RTLS;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace dreamcube.unity.Core.TestsRoot.PlayMode
{
    public class EssentialComponentsTest
    {
        [OneTimeSetUp]
        public void LoadScene()
        {
            // the first scene in the build settings needs to load all the essential 
            // dreamcube components
            if (SceneManager.GetSceneByBuildIndex(0).isLoaded == false)
                SceneManager.LoadScene(0);
        }

        [TearDown]
        public void UnloadScene()
        {
            if (SceneManager.GetSceneByBuildIndex(0).isLoaded)
                SceneManager.UnloadSceneAsync(0);
        }

        [UnityTest]
        public IEnumerator ContainsRootObjects()
        {
            Assert.IsNotNull(Object.FindObjectOfType<RTLSReceiver>());
            Assert.IsNotNull(Object.FindObjectOfType<EventManager>());
            Assert.IsNotNull(Object.FindObjectOfType<ConfigManager>());
            //Assert.IsNotNull(Object.FindObjectOfType<BaseSignalRClient>());
            Assert.IsNotNull(Object.FindObjectOfType<BaseSceneManager>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllBaseScenesLoaded()
        {
            Assert.IsTrue(SceneManager.GetSceneByBuildIndex(0).isLoaded);
            var sceneLoader = Object.FindObjectOfType<BaseSceneManager>();
            Assert.IsNotNull(sceneLoader);
            //yield return new WaitForSeconds(5);
            //foreach (var sceneName in sceneLoader.BaseSceneNames)
            //{
            //    //Log.Debug( $"Loading {sceneName}");
            //    Assert.IsTrue(SceneManager.GetSceneByName(sceneName).isLoaded);
            //}

            yield return null;
        }
    }
}
