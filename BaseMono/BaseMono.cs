using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityUtil
{
    public class BaseMono : MonoBehaviour
    {
        private List<Coroutine> corList = new List<Coroutine>();
        public void startTimeCoroutine(Func<int, bool> func, uint millisecond, int count = int.MaxValue, Action onStop = null)
        {
            corList.Add(StartCoroutine(timeCoroutine(func, millisecond, count, onStop)));
        }
        private IEnumerator timeCoroutine(Func<int, bool> func, uint millisecond, int count = int.MaxValue, Action onStop = null)
        {
            WaitForSeconds wfs = new WaitForSeconds(millisecond / 1000f);

            while (count-- > 0 && !func(count))
            {
                yield return wfs;
            }
            if (onStop != null) onStop();
        }

        private void OnDisable()
        {
            while (corList.Count > 0)
            {
                StopCoroutine(corList[0]);
                corList.RemoveAt(0);
            }
        }

        private void OnDistory()
        {
            PrefabLoader.Instance.distory(gameObject);
        }
    }
}
