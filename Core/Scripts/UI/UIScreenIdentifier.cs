using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using dreamcube.unity.Core.Scripts.API;
using UnityEngine;
using UnityEngine.Serialization;

namespace dreamcube.unity.Core.Scripts.UI
{
    // extremely game specific
    public enum ScreenTypeEnum
    {
        Idle_01 = 0,
        Welcome_01,
        ExperienceIntro_01,
        PracticeRoundIntro_01,
        PracticeRoundGameplay_01,
        DrillSelection_01,
        DrillIntroduction_01,
        Exercise_01,
        Leaderboard_01,
        ExperienceOutro_01,
        EndofGame_01,
        Maintenance,
        Offline,
        None,
        ActivityCTA
    }



    public class UIScreenIdentifier : MonoBehaviour
    {
        [SerializeField] public bool DoFade;
        [SerializeField] public float FadeInDelay;
        [SerializeField] public float FadeInSpeed;
        [SerializeField] public float FadeOutSpeed;


        [FormerlySerializedAs("screenTypeName")] [SerializeField] public ScreenTypeEnum screenTypeEnum;

        public void Transition(bool bDirection)
        {
            var canvas = GetComponent<CanvasGroup>();

            if (DoFade && bDirection)
            {
                gameObject.SetActive(bDirection);
                canvas.alpha = 0;

                var transition = DOTween.Sequence()
                    .AppendInterval(FadeInDelay)
                    .Append(canvas.DOFade(1, FadeInSpeed));

            }
            else if (DoFade && !bDirection)
                canvas.DOFade(0, FadeOutSpeed).OnComplete(() => gameObject.SetActive(false));
            else
                gameObject.SetActive(bDirection);
        }
    }
}
