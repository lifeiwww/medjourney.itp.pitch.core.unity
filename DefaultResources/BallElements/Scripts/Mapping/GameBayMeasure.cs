using dreamcube.unity.Core.Scripts.Util;
using UnityEngine;

public class GameBayMeasure : MonoBehaviour
{
    public GameObject ball;
    public GameObject floorMarker, ceilingMarker;
    public GameObject frontMarker, backMarker;
    public GameObject leftMarker, rightMarker;

    public float GetNormalizedVerticalPosition()
    {
        var normalizedPosition = Extensions.mapRange(floorMarker.transform.position.y,
            ceilingMarker.transform.position.y,
            0f,
            1f,
            ball.transform.position.y);

        return normalizedPosition;
    }

    public float GetNormalizedDepthPosition()
    {
        var normalizedPosition = Extensions.mapRange(backMarker.transform.position.z,
            frontMarker.transform.position.z,
            0f,
            1f,
            ball.transform.position.z);


        return normalizedPosition;
    }

    public float GetNormalizedHorizontalPosition()
    {
        var normalizedPosition = Extensions.mapRange(leftMarker.transform.position.x,
            rightMarker.transform.position.x,
            0f,
            1f,
            ball.transform.position.x);

        return normalizedPosition;
    }
}