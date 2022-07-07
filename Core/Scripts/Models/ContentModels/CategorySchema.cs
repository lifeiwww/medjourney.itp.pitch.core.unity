using System;
using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.Models.ContentModels;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.API.ContentAPI.Category
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
    public class Category
    {
        public string ID;
        public SharedSchema.Text Text;
        public Media Media;
        public List<string> ActivityList;

        public override string ToString()
        {
            var obj = MemberwiseClone() as Category;
            return JsonUtility.ToJson(obj);
        }
    }
}