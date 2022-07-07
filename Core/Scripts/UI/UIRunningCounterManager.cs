using System.Collections.Generic;
using DG.Tweening;
using Serilog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OdinSerializer.Utilities;
using UnityEngine.Serialization;

namespace dreamcube.unity.Core.Scripts.UI
{
    public class UIRunningCounterManager : MonoBehaviour
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


        private readonly float timeForScoreAnimation = 0.5f;
        private int _currentScore;
        private int _displayedScore;

        private void OnEnable()
        {
            ResetScore();
            SetTotalText(0);
        }

        public void SetRunningText(int num)
        {
            if (runningCount.SafeIsUnityNull()) return;
            var countText = $"{num:n0}";
            countText = AddPadding(countText);
            runningCount.text = countText;
        }

        public void SetTotalText(int total)
        {
            if (totalText.SafeIsUnityNull()) return;

            var scoreTextString = $"{total:D2}";
            scoreTextString = AddPadding(scoreTextString);
            totalText.text = scoreTextString;
        }

        private string AddPadding(string scoreTextString)
        {
            if (scoreTextString.Length == 1 && addPadding) scoreTextString = $"0{scoreTextString}";
            Log.Debug($"{nameof(AddPadding)}: setting number {scoreTextString}");
            return scoreTextString;
        }

        public void ResetScore()
        {
            SetRunningText(0);
            _currentScore = 0;

            foreach (var diff in scoreDifferences)
            {
                scoreDifferences.Remove(diff);
                Destroy(diff.gameObject);
            }
        }

        public void addToScore(int total)
        {
            _currentScore += total;
            DoAnimation(total);
        }

        protected virtual void DoAnimation(int diff)
        {
            Log.Debug($"Adding score {diff}");
            var gradient = diff >= 0 ? positiveScoreGradient : negativeScoreGradient;
            AnimateGradient(diff, gradient);
            AnimateDifference(diff);
            AnimateCounter();
        }

        private void AnimateCounter()
        {
            DOTween.To(() => _displayedScore, x => _displayedScore = x, _currentScore, timeForScoreAnimation)
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