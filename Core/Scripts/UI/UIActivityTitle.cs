using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.API;
using dreamcube.unity.Core.Scripts.Stores;
using Serilog;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


namespace dreamcube.unity.Core.Scripts.UI
{
    //[RequireComponent(typeof(TextMeshProUGUI))]
    public class UIActivityTitle : MonoBehaviour
    {
        [FormerlySerializedAs("activityTitleTextBox")]
        [SerializeField]
        private TextMeshProUGUI categoryNameTextBox;

        [SerializeField]
        private TextMeshProUGUI orderNumberTextBox;

        [FormerlySerializedAs("activityTypeTextBox")]
        [SerializeField]
        private TextMeshProUGUI activityTitleTextBoxEn;

        [SerializeField]
        private TextMeshProUGUI activityTitleTextBoxZh;

        [FormerlySerializedAs("languageController")] [SerializeField]
        private UILanguageController activityLanguageController;

        [SerializeField]
        private UILanguageController categoryLanguageController;

        private void Awake()
        {
            GameRoundDataStore.CurrentCategory.Subscribe(x =>
            {
                var categoryTitles = CategoryDataStore.GetCategoryTitleWithID(x);

                if (categoryNameTextBox != null)
                    categoryNameTextBox.text = GameRoundDataStore.CurrentCategory.Value;

                if (categoryLanguageController != null)
                    categoryLanguageController.SetText(categoryTitles.en, categoryTitles.zh);

            });

            GameRoundDataStore.CurrentActivityID.Subscribe(x =>
            {
                var activityID = GameRoundDataStore.CurrentActivityID.Value;
                var titles = ActivityDataStore.GetActivityTitleWithID(activityID);

                if (activityTitleTextBoxEn != null)
                    activityTitleTextBoxEn.text = titles.en;

                if (activityTitleTextBoxZh != null)
                    activityTitleTextBoxZh.text = titles.zh;

                if (activityLanguageController != null)
                    activityLanguageController.SetText(titles.en, titles.zh);
            });
        }

        public void SetCurrentExerciseIndex(int index)
        {
            var paddedNumber = " - " + index.ToString("D2");
            if (orderNumberTextBox != null)
                orderNumberTextBox.text = paddedNumber;
        }
    }
}
