using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Serilog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OdinSerializer.Utilities;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

namespace dreamcube.unity.Core.Scripts.UI
{

    [Flags]
    public enum CounterType
    {
        SIMPLE_COUNTER,
        SCORE_COUNTER
    }

    public class UIRunningCounterManagerBase : MonoBehaviour
    {

        [SerializeField] private List<TextMeshProUGUI> scoreDifferences = new List<TextMeshProUGUI>();
        [SerializeField] private GameObject diffTextElement;
        [SerializeField] private GameObject diffTextHolder;
        [FormerlySerializedAs("totalScore")] [SerializeField] private TextMeshProUGUI runningCount;
        [SerializeField] private TextMeshProUGUI totalText;
        [SerializeField] private Color positive = Color.white;
        [SerializeField] private Color negative = Color.red;
        [SerializeField] private Image positiveScoreGradient;
        [SerializeField] private Image negativeScoreGradient;
        [SerializeField] private bool addPadding;
        [SerializeField] private Vector2 gradientWidthRange = new Vector2(350, 1500); //starts at 350, can go to 1500

        [SerializeField] public CounterType CounterType;

        private readonly float timeForScoreAnimation = 0.5f;
        private int _currentCount;
        private int _currentMax;
        private int _displayedScore;


        private void OnEnable()
        {
            SetRunningText(_currentCount);
            SetTotalText(_currentMax);
        }

        public void SetRunningText(int num)
        {
            if (num == 0 && CounterType == CounterType.SIMPLE_COUNTER )
            {
                //Log.Debug( "Stop!!!");
            }


            if (runningCount.SafeIsUnityNull()) return;
            var countText = $"{num:n0}";
            countText = AddPadding(countText);
            runningCount.text = countText;
            _currentCount = num;
        }

        public void SetTotalText(int total)
        {
            if (totalText.SafeIsUnityNull()) return;

            _currentMax = total;
            var scoreTextString = $"{_currentMax:D2}";
            scoreTextString = AddPadding(scoreTextString);
            totalText.text = scoreTextString;
        }

        private string AddPadding(string scoreTextString)
        {
            if (scoreTextString.Length == 1 && addPadding) scoreTextString = $"0{scoreTextString}";
            //Log.Debug($"{nameof(AddPadding)}: setting number {scoreTextString}");
            return scoreTextString;
        }

        public void ResetCount()
        {
            SetRunningText(0);
            _currentCount = 0;
        }

        public void addToCounter(int total)
        {
            _currentCount += total;
            DoAnimation(total);
        }

        protected virtual void DoAnimation(int diff)
        {
            //Log.Debug($"Adding score {diff}");
            var gradient = diff >= 0 ? positiveScoreGradient : negativeScoreGradient;
            if (gradient!=null)
                AnimateGradient(diff, gradient);

            AnimateDifference(diff);
            AnimateCounter();
        }

        private void AnimateCounter()
        {
            DOTween.To(() => _displayedScore, x => _displayedScore = x, _currentCount, timeForScoreAnimation)
                .OnUpdate(UpdateScoreDisplay);
        }

        private void UpdateScoreDisplay()
        {
            // No digits after the decimal point. Output: 9,876
            if (runningCount.SafeIsUnityNull()) return;
            var text = $"{_displayedScore:n0}";
            if (text.Length == 1) text = $"0{text}";
            runningCount.text = text;
        }

        private void AnimateDifference(int diff)
        {
            if (diffTextHolder == null)
                return;

            var faceColor = diff >= 0 ? positive : negative;

            //animate the difference
            var obj = Instantiate(diffTextElement, diffTextHolder.gameObject.transform);
            var tmPro = obj.GetComponent<TextMeshProUGUI>();
            tmPro.color = faceColor;
            scoreDifferences.Add(tmPro);

            // if the prefab does not contain text
            if (tmPro.SafeIsUnityNull()) return;

            var prefix = diff < 0 ? "" : "+";
            tmPro.text = prefix + diff;
            tmPro.rectTransform.localPosition = Vector3.zero;

            var diffSequence = DOTween.Sequence();
            diffSequence.Append(tmPro.DOFade(1, 0.1f));
            diffSequence.Append(tmPro.DOFade(1, timeForScoreAnimation));
            diffSequence.Append(tmPro.DOFade(0, timeForScoreAnimation));
            diffSequence.Join(tmPro.rectTransform.DOMoveY(runningCount.rectTransform.position.y,
                timeForScoreAnimation));
            diffSequence.AppendCallback(() => RemoveSingleScoreDiff(tmPro));
        }

        private void AnimateGradient(int diff, Image gradient )
        {

            if (gradient.SafeIsUnityNull()) return;

            //animate gradient
            var gradientTargetWidth = (int) gradientWidthRange.y;
            var finalSize = new Vector2(gradientTargetWidth, gradient.rectTransform.sizeDelta.y);

            //start state
            var initialSize =
                new Vector2(gradientWidthRange.x, gradient.rectTransform.sizeDelta.y);

            gradient.rectTransform.sizeDelta = initialSize;

            var gradientSequence = DOTween.Sequence();
            gradientSequence.Append(gradient.DOFade(1, 0.1f))
                .Append(gradient.rectTransform.DOSizeDelta(finalSize, timeForScoreAnimation))
                .AppendInterval(timeForScoreAnimation/2)
                .Append(gradient.DOFade(0, timeForScoreAnimation))
                .Join(gradient.rectTransform.DOSizeDelta(initialSize, timeForScoreAnimation))
                .OnComplete(() => gradient.rectTransform.sizeDelta = initialSize);
        }

        private void RemoveSingleScoreDiff(TextMeshProUGUI diffToTurnOff)
        {
            diffToTurnOff.alpha = 0;
            scoreDifferences.Remove(diffToTurnOff);
            GameObject.Destroy(diffToTurnOff.gameObject);
        }
    }
}