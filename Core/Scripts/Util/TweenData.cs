using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class TweenData
{
    public enum TweenType
    {
        Move,
        Rotate,
        Scale
    }

    [SerializeField] private TweenType tweenType;
    [SerializeField] private RectTransform animatedObjectTransform;
    [SerializeField] private float duration;
    [SerializeField] private Ease easing;
    [SerializeField] public float interval;
    [SerializeField] public bool join;


    [SerializeField] private Vector3 positionOffset = new Vector3();
    [SerializeField] private Vector3 targetRotation = new Vector3();
    [SerializeField] private float targetScale;

    public Tween GetTween()
    {
        switch (tweenType)
        {
            case TweenType.Move:
                Vector3 targetPosition = animatedObjectTransform.transform.position + positionOffset;
                return animatedObjectTransform.DOMove(targetPosition, duration).SetEase(easing);

            case TweenType.Rotate:
                return animatedObjectTransform.DORotate(targetRotation, duration).SetEase(easing);

            case TweenType.Scale:
                return animatedObjectTransform.DOScale(targetScale, duration).SetEase(easing);

            default:
                return null;
        }
    }

}
