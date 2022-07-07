using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


public class ProgressBarBlock : MonoBehaviour
{
    [SerializeField] private Color backgroundColorOff;
    [SerializeField] private Color backgroundColorOn;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Color strokeColorOff;
    [SerializeField] private Color strokeColorOn;
    [SerializeField] private Image strokeImage;

    [SerializeField] private float switchSpeed = 0.2f;

    private void Start()
    {
        Assert.IsNotNull(backgroundImage, "backgroundImage is null");
        Assert.IsNotNull(strokeImage, "strokeImage is null");
    }

    private void OnEnable()
    {
        backgroundImage.color = backgroundColorOff;
        strokeImage.color = strokeColorOff;
    }

    public void Activate(bool goActive)
    {
        Color backColor = goActive ? backgroundColorOn : backgroundColorOff;
        Color strokeColor = goActive ? strokeColorOn : strokeColorOff;

        backgroundImage.DOColor(backColor, switchSpeed);
        strokeImage.DOColor(strokeColor, switchSpeed);
    }
}
