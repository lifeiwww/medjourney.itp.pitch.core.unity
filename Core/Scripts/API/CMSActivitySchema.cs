using System;
using System.Collections.Generic;

namespace dreamcube.unity.Core.Scripts.API.CMSSchemaPlaceholder
{
    public class CMSActivitySchema
    {

        [Serializable]
        public class CMSDataModelBase
        {
        }

        [Serializable]
        public class LocalizedText : CMSDataModelBase
        {
            public string Category;
            public string FieldName;
            public string En;
            public string Zh;
        }

        [Serializable]
        public class ActivityOption : CMSDataModelBase
        {
            public int ID;
            public string ActivityType;
            public LocalizedText Header;
            public LocalizedText Title;
            public string PrefabFileName;
            public string LinearMediaUrl;
        }

        [Serializable]
        public class ActivitySet : CMSDataModelBase
        {
            public int ID;
            public string ActivityType;
            public LocalizedText Title;
            public LocalizedText Description;
            public string ImageUrl;
            public List<ActivityOption> activityOptionSet = new List<ActivityOption>();

        }

        [Serializable]
        public class ActivitySets : CMSDataModelBase
        {
            public List<ActivitySet> AllActivitySets = new List<ActivitySet>();
        }
    }
}
