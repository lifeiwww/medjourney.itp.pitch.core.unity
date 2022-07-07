using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Serilog;

public class TweenSequencer : MonoBehaviour
{
    public List<TweenData> tweenDataList = new List<TweenData>();

    private Sequence _sequence;
    private bool _isTriggered;

    private void Awake()
    {
        _sequence = DOTween.Sequence();
        InitializeSequence();
    }


    private void Update()
    {
        //if( Input.GetKeyDown(KeyCode.P))
        //    StartSequence();
    }

    private void InitializeSequence()
    {
        // stop any active animation
        _sequence.Pause();

        // allow the sequence to be reusable
        _sequence.SetAutoKill(false);

        foreach (var tweenData in tweenDataList)
        {
            if (tweenData.interval > 0)
                _sequence.PrependInterval(tweenData.interval);

            if (tweenData.join)
                _sequence.Join(tweenData.GetTween());
            else
                _sequence.Append(tweenData.GetTween()).OnComplete(() => TweenComplete() );
        }
    }

    private void TweenComplete()
    {
        Log.Debug($"sequence complete {gameObject.name}");
    }

    public void StartSequence()
    {
        Log.Debug($"starting sequence on {gameObject.name}");

        if( !_isTriggered )
            _sequence.PlayForward();
        else
            _sequence.PlayBackwards();

        _isTriggered = !_isTriggered;
    }
}

