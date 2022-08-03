using System;
using System.Collections;
using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.General;
using dreamcube.unity.Core.Scripts.Signals.Events;
using UniRx;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.AssetLoading
{
    public class BaseSceneManager : Singleton<BaseSceneManager>
    {
        [SerializeField]
        private List<string> baseSceneNamesList = new List<string>();
        public readonly List<string> BaseSceneNames = new List<string>();
        public static readonly ReactiveProperty<bool> AllScenesLoaded =
            new ReactiveProperty<bool>(false);

        protected virtual void Start()
        {
            ListAllInactiveBaseScenes();
            StartCoroutine(LoadBaseScenes(OnBaseScenesLoaded));
        }

        protected virtual void ListAllInactiveBaseScenes()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                var sceneName =
                    System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility
                        .GetScenePathByBuildIndex(i));

                if (baseSceneNamesList.Contains(sceneName) && !UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    BaseSceneNames.Add(sceneName);
                    Debug.Log($"Adding scene {sceneName} to scenes list");
                }
            }
        }

        IEnumerator LoadBaseScenes( Action callback )
        {
            List<Coroutine> sceneLoadCoroutines = new List<Coroutine>();
            foreach (var sceneName in BaseSceneNames)
            {
                Coroutine coroutine = StartCoroutine(SceneLoader.LoadSceneAsyncNamed(sceneName));
                sceneLoadCoroutines.Add(coroutine);
            }

            foreach (var loadCoroutine in sceneLoadCoroutines)
            {
                yield return loadCoroutine;
            }

            callback?.Invoke();
            Debug.Log($"Base Scenes are loaded");

        }

        protected virtual void OnBaseScenesLoaded()
        {
            AllScenesLoaded.Value = true;
            EventManager.Instance.TriggerEvent(EventStrings.EventOnAllScenesLoaded);
        }
    }
}
