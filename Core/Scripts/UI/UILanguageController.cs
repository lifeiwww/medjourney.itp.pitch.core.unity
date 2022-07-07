using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.API;
using dreamcube.unity.Core.Scripts.API.CMSSchemaPlaceholder;
using dreamcube.unity.Core.Scripts.Stores;
using Serilog;
using TMPro;
using UnityEngine;
using Extensions = dreamcube.unity.Core.Scripts.Util.Extensions;
using UniRx;

namespace dreamcube.unity.Core.Scripts.UI
{
    public class UILanguageController : MonoBehaviour
    {

        [SerializeField] private TMP_FontAsset chineseFontBold;
        [SerializeField] private TMP_FontAsset chineseFontRegular;
        [SerializeField] private TMP_FontAsset englishFontBold;
        [SerializeField] private TMP_FontAsset englishFontRegular;

        [Space(20)]
        [SerializeField] private float lineSpacingEN = 0;
        [SerializeField] private float lineSpacingZH = 0;

        [SerializeField] private bool isBold = true;
        [SerializeField] private bool shouldSwitch = true;

        [SerializeField] private TextMeshProUGUI textBox;
        [SerializeField] private TextMeshPro textBoxNonGui;

        [SerializeField] private string _chinese = "丢失的文本";
        [SerializeField] private string _english = "Missing Text";

        private LANGUAGE _currentLanguageMode = LANGUAGE.LANGUAGE_ZH;


        private void Awake()
        {
            GameRoundDataStore.CurrentLanguage.Subscribe(x =>
            {
                _currentLanguageMode = (LANGUAGE) GameRoundDataStore.CurrentLanguage.Value;
                SwitchLanguage(_currentLanguageMode);
            });

            if (textBox == null) textBox = gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
            LoadFonts();
        }

        private void OnEnable()
        {
            SwitchLanguage(_currentLanguageMode);
        }

        private void LoadFonts()
        {
            if (englishFontBold == null) englishFontBold = Resources.Load<TMP_FontAsset>("BebasNeue-Bold SDF-512-All");
            if (englishFontRegular == null) englishFontRegular = Resources.Load<TMP_FontAsset>("BebasNeue Regular SDF-All");
            if (chineseFontBold == null) chineseFontBold = Resources.Load<TMP_FontAsset>("NotoSansCJKsc-Bold SDF-ALL");
            if (chineseFontRegular == null)
                chineseFontRegular = Resources.Load<TMP_FontAsset>("NotoSansCJKsc-DemiLight SDF-ALL");
        }

        public void SetText(string englishText, string chineseText)
        {
            _chinese = chineseText;
            _english = englishText;
            SwitchLanguage(_currentLanguageMode);
        }

        public void SetText(string txt)
        {
            if (textBox != null) textBox.text = txt;
            if (textBoxNonGui != null) textBoxNonGui.text = txt;
        }

        public void SetControllerText(CMSActivitySchema.LocalizedText data)
        { 
            SetText(data.En, data.Zh);
        }

        public string GetText()
        {
            if (textBox != null) return textBox.text;
            if (textBoxNonGui != null) return textBoxNonGui.text;
            return "TMPRO component not found";
        }

        public void SwitchLanguage(LANGUAGE language)
        {
            if (!shouldSwitch) return;

            //Log.Debug($"Switching language to {language}");
            _currentLanguageMode = language;

            var lineSpacing = _currentLanguageMode == LANGUAGE.LANGUAGE_EN ? lineSpacingEN : lineSpacingZH;
            var text = _currentLanguageMode == LANGUAGE.LANGUAGE_EN ? _english : _chinese;

            // fancy switch statement
            TMP_FontAsset font = _currentLanguageMode switch
            {
                LANGUAGE.LANGUAGE_EN when isBold => englishFontBold,
                LANGUAGE.LANGUAGE_EN => englishFontRegular,
                LANGUAGE.LANGUAGE_ZH when isBold => chineseFontBold,
                _ => chineseFontRegular
            };

            if (textBox != null)
            {
                textBox.lineSpacing = lineSpacing;
                textBox.font = font;
                textBox.text = text;
            }
            else if (textBoxNonGui != null)
            {
                textBoxNonGui.font = font;
                textBoxNonGui.text = text;
            }


        }

        //private void EventHandler(string eventType, string msg, GameObject sender, DataModelBase data)
        //{
        //    IntData languageEnumData = Extensions.Cast(data, typeof(IntData));
        //    if (eventType == EventStrings.EventOnLanguageChange)
        //        SwitchLanguage((LANGUAGE)languageEnumData.AInt);
        //}
    }
}