
using LitJson;
using System;
using System.Collections;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityUtil
{
    public class NetWrapper : MonoSingleton<NetWrapper>
    {
        private string[] urls = null;
        private uint index = 0;
        private string token = "";
        public int setBaseUrlList(params string[] _urls) {
            if (urls != null) {
                LogService.LogWarning(string.Format("NetWrapper: already have urls:{0}", urls.Length));
                return -1;
            }
            urls = _urls;
            return _urls.Length;
        }
        public bool setUsingUrl(uint _index)
        {
            if (index >= urls.Length)
            {
                LogService.LogWarning(string.Format("NetWrapper:index out of rang urls:{0}", urls.Length));
                return false;
            }
            index = _index;
            return true;
        }
        public void OpenBrowser(string url)
        {
            LogService.LogWarning(string.Format("NetWrapper: open with browser url={0}", url));
            Application.OpenURL(url);
        }

        public void postData(Action<JsonData> action, string url, JsonData data)
        {
            corList.Add(StartCoroutine(postCoroutine(action, url, data)));
        }
        private IEnumerator postCoroutine(Action<JsonData> action, string url, JsonData data)
        {
            if (url == null)
            {
                token = null;
                yield break;
            }
            UnityWebRequest webRequest = UnityWebRequest.Post(url, data.ToString());
            webRequest.SetRequestHeader("token", token);
            yield return webRequest.SendWebRequest();
            if (webRequest.error != null)
            {
                Debug.LogError(webRequest.error);
            }

            JsonData jsd = LitJson.JsonMapper.ToObject(webRequest.downloadHandler.text);


            token = ((string)jsd["data"]["token"]);
            webRequest.Dispose();
            LogService.LogWarning(string.Format("url={0},res={1}", url, webRequest.downloadHandler.text), LogService.LogType.UNITY_EDITOR);

            if (action.Target == null || action.Method == null)
            {
                yield break;
            }
            else
            {
                action(jsd);
            }
        }
    }
}
