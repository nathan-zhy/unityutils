using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        private static readonly object locker = new object();
        private static bool bAppQuitting;
        protected List<Coroutine> corList = new List<Coroutine>();

        public static T Instance
        {
            get
            {
                if (bAppQuitting)
                {
                    instance = null;
                    return instance;
                }

                lock (locker)
                {
                    if (instance == null)
                    {
                        T[] managers = Object.FindObjectsOfType(typeof(T)) as T[];
                        if (managers.Length != 0)
                        {
                            if (managers.Length == 1)
                            {
                                instance = managers[0];
                                instance.gameObject.name = typeof(T).Name;
                                return instance;
                            }
                            else
                            {
                                Debug.LogError("Class " + typeof(T).Name + " exists multiple times in violation of singleton pattern. Destroying all copies");
                                foreach (T manager in managers)
                                {
                                    Destroy(manager.gameObject);
                                }
                            }
                        }

                        var singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)" + typeof(T);
                        singleton.hideFlags = HideFlags.None;
                        DontDestroyOnLoad(singleton);
                    }
                    instance.hideFlags = HideFlags.None;
                    return instance;
                }
            }
        }

        protected virtual void Awake()
        {
            bAppQuitting = false;

        }

        protected virtual void OnDestroy()
        {
            bAppQuitting = true;
            while (corList.Count > 0)
            {
                StopCoroutine(corList[0]);
                corList.RemoveAt(0);
            }
        }
    }
}