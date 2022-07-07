using System;

namespace dreamcube.unity.Core.Scripts.Models.ContentModels
{
    public class SharedSchema
    {
        [Serializable]
        public class Title
        {
            public string en;
            public string zh;
        }

        [Serializable]
        public class Description
        {
            public string en;
            public string zh;
        }

        [Serializable]
        public class Text
        {
            public Title Title;
            public Description Description;
        }

    }
}
