using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dreamcube.unity.Core.Scripts.API;
using dreamcube.unity.Core.Scripts.Stores;
using dreamcube.unity.Core.Scripts.Util;
using TMPro;
using UniRx;
using UnityEngine;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using Serilog;

namespace dreamcube.unity.Core.Scripts.UI
{
    public class UIPlayerNamesListWelcomeMode : MonoBehaviour
    {
        //comment
        [Header("Welcome Mode")]

        [SerializeField]
        private UILanguageController welcomeMessage;

        [SerializeField]
        private TextMeshProUGUI welcomePlayerNamesMiddle;

        [SerializeField]
        private TextMeshProUGUI welcomePlayerNamesLeft;

        [SerializeField]
        private TextMeshProUGUI welcomePlayerNamesRight;

        private int _maxCharsForNameWithMultipleCols = 8;
        private int _maxCharsForNameWithSingleCol = 16;

        private List<string> previousNames = new List<string>();

        private void Awake()
        {
            DreamCubeSessionDataStore.DreamCubeSessionData.Subscribe(x =>
            {
                ReplaceIDText();
                List<string> names = StoreUtil.GetPlayersNames();
                if (!names.SequenceEqual(previousNames)) 
                {
                    SetNames(names);
                    previousNames = names;
                }
            });
        }

        private void ReplaceIDText()
        {
            var screens = ScreenDataStore.Screens.Where(x =>
                x.ID == DreamCubeScreenStates.DC_WELCOME);
            var dreamCubeId = ConfigManager.Instance.generalSettings.DreamCube;
            var screen = screens.FirstOrDefault();
            {
                if (screen != null)
                {
                    var titleEn = ReplaceNumberInText(screen.Text.Title.en, dreamCubeId);
                    var titleZh = ReplaceNumberInText(screen.Text.Title.zh, dreamCubeId);
                    welcomeMessage.SetText(titleEn, titleZh);
                }
            }
        }

        private static string ReplaceNumberInText(string titleText, string ID)
        {
            var foundValue = Regex.Match(titleText, @"\d+").Value;
            //Log.Debug($"found number ---> {foundValue}");
            
            if (ID.Length == 1) ID = "0" + ID;

            var resultString = Regex.Replace(titleText, @"\d+", ID );
            return resultString;
        }

        public void SetNames(List<string> names)
        {
            if (names?.Any() != true) return;

            List<string> allNames = new List<string>();
            string firstCol = "";
            string secondCol = "";

            var maxNameLength = names.Count > 6 ? _maxCharsForNameWithMultipleCols : _maxCharsForNameWithSingleCol;

            // distribute name into two columns if more than 8 players
            for (int i = 0; i < names.Count; i++)
            {
                var shortName = Extensions.Truncate(names[i], maxNameLength);

                if (names.Count > 6)
                {
                    if (i % 2 == 0) firstCol += shortName + "\n";
                    else secondCol += shortName + "\n";
                }
                else firstCol += shortName + "\n";
                
            }


            allNames.Add(firstCol);
            if (secondCol.Any())
                allNames.Add(secondCol);


            if (allNames.Any() == false)
                return;

            if (allNames.Count == 1)
            {
                welcomePlayerNamesMiddle.text = allNames.First();
                welcomePlayerNamesRight.text = "";
                welcomePlayerNamesLeft.text = "";
            }
            else if (allNames.Count > 1)
            {
                welcomePlayerNamesMiddle.text = "";
                welcomePlayerNamesLeft.text = allNames.First();
                welcomePlayerNamesRight.text = allNames.Last();
            }
        }
    }
}
