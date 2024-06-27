
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace UnityUtil
{
    class PrefabIns:BaseMono
    {
        [ReadOnly]
        [HideInInspector]
        public string prefabName;
    }

    public class PrefabLoader : MonoSingleton<PrefabLoader>
    {
        Dictionary<string, ConcurrentQueue<GameObject>> unusePrefabs = new Dictionary<string, ConcurrentQueue<GameObject>>();
        Dictionary<string, List<GameObject>> usePrefabs = new Dictionary<string, List<GameObject>>();

        public void generate(string loadkey, Transform parent)
        {
            generate(loadkey, parent,Vector3.one);
        }
        public void generate(string loadkey, Transform parent, Vector3 position) 
        {
            GameObject objtmp = null;
            if (unusePrefabs.ContainsKey(loadkey)) 
            {
                unusePrefabs[loadkey].TryDequeue(out objtmp);
            }
            if (objtmp == null)
            {
                objtmp = Instantiate((GameObject)Resources.Load(loadkey));
            }
            if (objtmp.GetComponent<PrefabIns>() == null)
            {
                PrefabIns comp = objtmp.AddComponent<PrefabIns>();
                comp.prefabName = loadkey;
            }

            objtmp.transform.parent = parent;
            objtmp.transform.localPosition = position;
            objtmp.transform.localScale = Vector3.one;
            objtmp.transform.localEulerAngles = Vector3.zero;
            objtmp.SetActive(true);

            checkDic(loadkey);
            usePrefabs[loadkey].Add(objtmp);
        }
        public void distory(GameObject disobj) 
        {
            PrefabIns[] ins = disobj.GetComponentsInChildren<PrefabIns>(true);
            foreach (PrefabIns intmp in ins) 
            {
                GameObject objtmp = intmp.gameObject;
                objtmp.transform.parent = transform;
                objtmp.SetActive(false);
                checkDic(intmp.prefabName);

                usePrefabs[intmp.prefabName].Remove(objtmp);
                unusePrefabs[intmp.prefabName].Enqueue(objtmp);
            }
        }

        void checkDic(string loadkey)
        {
            if (!unusePrefabs.ContainsKey(loadkey))
            {
                unusePrefabs.Add(loadkey, new ConcurrentQueue<GameObject>());
            }
            if (!usePrefabs.ContainsKey(loadkey))
            {
                usePrefabs.Add(loadkey, new List<GameObject>());
            }
        }
    }
}
