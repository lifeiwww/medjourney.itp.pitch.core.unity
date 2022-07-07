using System.Collections;
using System.Threading.Tasks;
using Serilog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dreamcube.unity.Core.Scripts.AssetLoading
{
    public static class SceneLoader
    {

        public static IEnumerator LoadSceneAsyncNamed(string sceneName)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded == false)
            {
                AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                Log.Information($"Finished loading scene {sceneName}");
            }
            else
            {
                Log.Warning($"{sceneName} already loaded");
            }
        }


        public static IEnumerator UnloadSceneAsyncNamed(string sceneName)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded == true)
            {
                AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

                // Wait until the asynchronous scene fully unloads
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                Log.Information($"finished unloading scene {sceneName}");
            }
            else
            {
                Log.Warning($"{sceneName} not currently already loaded");
            }
        }


        public static async Task SwitchScenes(string sceneToUnload, string sceneToLoad)
        {
            await UnLoadScene(sceneToUnload);
            await LoadScene(sceneToLoad);
        }

        public static async Task UnLoadScene(string sceneName)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                var UnloadScene = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
                while (UnloadScene.isDone == false) await Task.Delay(30);
                Log.Information($"finished unloading scene {sceneName}");
            }
        }

        public static async Task LoadScene(string sceneName)
        {
            if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                var loadScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (loadScene.isDone == false) await Task.Delay(30);
                Log.Information($"finished loading scene {sceneName}");
            }
        }

    }
}
