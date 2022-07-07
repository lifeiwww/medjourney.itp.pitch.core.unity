//using Sirenix.OdinInspector;

using UnityEngine;

namespace dreamcube.unity.Core.Scripts.General
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        #region Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (isApplicationQuiting) //Log.Debug("isApplicationQuiting returning null " + typeof(T).Name);
                    return null;

                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        //Log.Debug("Creating singleton of type " + typeof(T).Name + " " + isApplicationQuiting.ToString() );
                        var obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        ///     The instance.
        /// </summary>
        private static T instance;

        private static bool isApplicationQuiting;

        #endregion

        #region Methods

        /// <summary>
        ///     Use this for initialization.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            //Log.Debug("On application quit " + typeof(T).Name);
            isApplicationQuiting = true;
        }

        #endregion
    }
}