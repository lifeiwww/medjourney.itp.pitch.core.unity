using System;
using dreamcube.unity.Core.Scripts.Models.ContentModels;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.API.ContentAPI.Activity
{
    [Serializable]
    public class Intro
    {
        public string en;
        public string zh;
    }

    [Serializable]
    public class Outro
    {
        public string en;
        public string zh;
    }

    [Serializable]
    public class Image
    {
        public string en;
        public string zh;
    }

    [Serializable]
    public class Media
    {
        public Intro Intro;
        public Outro Outro;
        public Image Image;
    }

    [Serializable]
    public class Activity
    {
        public string ID;
        public SharedSchema.Text Text;
        public Media Media;
        public string PrefabFilename;

        public override string ToString()
        {
            var obj = MemberwiseClone() as Activity;
            return JsonUtility.ToJson(obj);
        }
    }

}