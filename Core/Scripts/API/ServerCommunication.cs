using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamcube.unity.Core.Scripts.General;
using Serilog;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static System.String;

namespace dreamcube.unity.Core.Scripts.API
{
    /// <summary>
    /// https://www.patrykgalach.com/2019/04/18/how-to-call-rest-api-in-unity/
    /// This class is responsible for handling REST API requests to remote server.
    /// To extend this class you just need to add new API methods.
    /// </summary>
    public class ServerCommunication : Singleton<ServerCommunication>
    {
        #region [Server Communication]

        public void SendRequest(string url, string type, UnityAction<string, string> CallbackOnSuccess, UnityAction<string,string> callbackOnFail)
        {
            StartCoroutine(RequestCoroutine(url, type, CallbackOnSuccess, callbackOnFail));
        }

        private IEnumerator RequestCoroutine(string url, string type, UnityAction<string, string> CallbackOnSuccess, UnityAction<string,string> CallbackOnFail)
        {
            if (!IsNullOrEmpty(url))
            {
                var www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ProtocolError || IsNullOrEmpty(www.downloadHandler.text))
                    CallbackOnFail?.Invoke(
                        $"{www.error} | {www.method} | {www.uri} | of type {type} |{www.downloadHandler.text}", type);
                else
                    CallbackOnSuccess?.Invoke(www.downloadHandler.text, type);
            }
            else
            {
                Log.Warning($"URI is empty, aborting request");
                yield return null;
            }
        }

        #region ignore

        ///// <summary>
        ///// This method finishes request process as we have received answer from server.
        ///// </summary>
        //private void ParseResponse<T>(string dataString, UnityAction<T> callbackOnSuccess, UnityAction<string> callbackOnFail)
        //{
        //    try
        //    {
        //        Log.Debug($"Parsing : {dataString}");
        //        var parsedData = JsonUtility.FromJson<T>(dataString);
        //        callbackOnSuccess?.Invoke(parsedData);
        //    }
        //    catch ( Exception ex)
        //    {
        //        callbackOnFail?.Invoke($"ParseResponse failed {typeof(T)} : {ex.Message}");
        //    }

        //}

        #endregion
        /// <summary>
        /// Post request to rest API
        /// </summary>
        public void PostRequest<T>(string url, T data, UnityAction<string> CallbackOnSuccess, UnityAction<string> CallbackOnFail)
        {
            if (IsNullOrEmpty(url))
            {
                Log.Debug($"PostRequest URL is empty {url}");
            }


            StartCoroutine(PostCoroutine(url, data, CallbackOnSuccess, CallbackOnFail));
        }

        private IEnumerator PostCoroutine<T>(string url, T data, UnityAction<string> CallbackOnSuccess, UnityAction<string> CallbackOnFail)
        {
            var jsonData = JsonUtility.ToJson(data);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
            var www = UnityWebRequest.Put(url, byteArray);
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            Log.Debug($"Posting data: {jsonData}");

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
                CallbackOnFail?.Invoke($"{www.error} | {www.method} | {url}");
            else
                CallbackOnSuccess?.Invoke(www.result.ToString());
        }
        #endregion
    }
}