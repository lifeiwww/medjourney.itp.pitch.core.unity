using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Util
{
    //"Serialize and Deserialize Json and Json Array in Unity" 
    // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity

    public static class JsonHelper
    {
        public static List<T> FromJson<T>(string json)
        {
            json = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(List<T> list)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = list;


            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(List<T> list, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = list;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public List<T> Items;
        }
    }
}