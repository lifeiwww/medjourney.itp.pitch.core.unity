using System;
using System.Collections.Generic;
using System.Linq;
using dreamcube.unity.Core.Scripts.API;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Screen;
using dreamcube.unity.Core.Scripts.Stores;
using Serilog;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Screen = dreamcube.unity.Core.Scripts.API.ContentAPI.Screen.Screen;

namespace dreamcube.unity.Core.Scripts.UI
{
    public class UIDualLanguageTitle : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI screenTitleEn;

        [SerializeField]
        private TextMeshProUGUI screenTitleZh;

        [SerializeField] private ScreenTypeEnum associatedScreen = ScreenTypeEnum.None;

        public void Awake()
        {
            // if this is not a subscreen inherit type from screen ID
            var screenIdentifier = GetComponent<UIScreenIdentifier>();
            if (screenIdentifier != null)
                associatedScreen = screenIdentifier.screenTypeEnum;
            SetScreenTitles();
        }

        public void SetScreenTitles()
        {
            var screens = ScreenDataStore.Screens?.Where(x=> x.ID == ScreenNameLookup.GetScreenTypeName(associatedScreen));
            var screen = screens.FirstOrDefault();
            
            if (screen != null )
            {
                screenTitleEn.text = screen.Text.Title.en;
                screenTitleZh.text = screen.Text.Title.zh;
            }
        }
    }
}
