using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace dreamcube.unity.Core.Scripts.Debugging
{
    [Serializable]
    public struct ConstrainAxis
    {
        public bool X;
        public bool Y;
        public bool Z;
    }

    public class BallToWallProjectionMarker : MonoBehaviour
    {
        [SerializeField] private ConstrainAxis constraints;
        private GameObject _ball;
        
        void Start()
        {
            _ball = GameObject.FindGameObjectWithTag("Ball");
        }

        void LateUpdate()
        {
            var ballPosition = _ball.transform.position;

            if (constraints.X)
                ballPosition.x = gameObject.transform.localPosition.x;
            if (constraints.Z)
                ballPosition.z = gameObject.transform.localPosition.z;

            gameObject.transform.localPosition = ballPosition;
        }
    }
}