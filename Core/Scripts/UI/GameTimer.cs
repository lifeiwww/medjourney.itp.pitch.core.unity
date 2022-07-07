using System;
using manutd;
using Serilog;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{

    [SerializeField] private Animator gameClockAnimator;
    [SerializeField] private TextMeshProUGUI gameTimer;
    [SerializeField] private float warningTime = 10;
    [SerializeField] private bool countBackwards = true;
    [SerializeField] private string timeFormat = "{0:N2}";

    private bool _alarmStarted;
    private bool _timeStarted;
    private float _currentTime;

    public static float TimerDuration { get; set; }


    public void Update()
    {
        if (_timeStarted) UpdateTime();
    }

    public void StartTimer()
    {
        _timeStarted = true;
        SetStandardStageClock();
        InvokeRepeating(nameof(UpdateTime), 0, 1);
    }

    public void SetTime(float duration)
    {
        TimerDuration = duration;
        _currentTime = countBackwards ? duration : 0;
        gameTimer.text = FormatTime(_currentTime);
    }

    public void UpdateTime()
    {
        if (countBackwards)
            _currentTime -= Time.deltaTime;
        else
            _currentTime += Time.deltaTime;


        if (_currentTime <= warningTime && countBackwards ||
            _currentTime >= (TimerDuration - warningTime) && !countBackwards)
        {
            if (!_alarmStarted)
                SetWarningStageClock();
        }


        if (_currentTime <= 0 && countBackwards ||
            _currentTime >= TimerDuration && !countBackwards)
            ResetTimer();

        gameTimer.text = FormatTime(_currentTime);

    }

    private void ResetTimer()
    {
        _timeStarted = false;
        _alarmStarted = false;
        _currentTime = countBackwards ? 0 : TimerDuration;
        SetStandardStageClock();
        CancelInvoke(nameof(UpdateTime));
    }

    private string FormatTime(float currentTime)
    {
        var time = TimeSpan.FromSeconds(currentTime);
        var seconds = string.Format(timeFormat, time.TotalSeconds);
        return seconds;
    }

    public void SetStandardStageClock()
    {
        _alarmStarted = false;
        if (gameClockAnimator)
            gameClockAnimator.SetBool("isWarning", false);
    }

    public void SetWarningStageClock()
    {
        _alarmStarted = true;
        if (gameClockAnimator)
            gameClockAnimator.SetBool("isWarning", true);
    }
}