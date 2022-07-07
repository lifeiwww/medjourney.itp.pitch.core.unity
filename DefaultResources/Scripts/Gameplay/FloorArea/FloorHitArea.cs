using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloorHitArea : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem particles;
    private bool _ballInArea = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            particles.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (_ballInArea == false)
            {
                transform.DOScale(Vector3.one * 0.9f, 0.1f);
            }
            _ballInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            _ballInArea = false;
            transform.DOScale(Vector3.one, 0.1f)
                .OnComplete(() => transform.localScale = Vector3.one );
            particles.Stop();
        }
    }
}
