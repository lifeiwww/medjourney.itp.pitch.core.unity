using System;
using dreamcube.unity.Core.Scripts.Models.ContentModels;
using UnityEngine;
using UnityEngine.Serialization;

namespace dreamcube.unity.Core.Scripts.API.ContentAPI.Screen
{


    [Serializable]
    public class Text
    {
        public SharedSchema.Title Title;
    }

    [Serializable]
    public class Video
    {
        public string en;
        public string zh;
    }

    [Serializable]
    public class Media
    {
        public Video Video;
    }

    [Serializable]
    public class Screen
    {
        public string ID;
        public Text Text;
        public Media Media;

        public override string ToString()
        {
            var obj = MemberwiseClone() as Screen;
            return JsonUtility.ToJson(obj);
        }
    }


    //[Serializable]
    //public class Title
    //{
    //    public string en;
    //    public string zh;
    //}

    //[Serializable]
    //public class Video
    //{
    //    public string en;
    //    public string zh;
    //}

    //[Serializable]
    //public class Text
    //{
    //    public Title title;
    //}

    //[Serializable]
    //public class Media
    //{
    //    public Video video;
    //}

    //[Serializable]
    //public class Screen
    //{
    //    public string ID;
    //    public Text Text;
    //    public Media Media;
    //}
}