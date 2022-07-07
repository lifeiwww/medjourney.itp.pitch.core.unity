using System.Collections.Generic;
using UnityEngine;

public class AutoKick : MonoBehaviour
{
    public List<Transform> aimPoints;
    private Rigidbody body;
    public float ForceMultiplier = 5f;
    public float timeBetweenKicks = 3f;

    private float timeUntilNextKick = 3f;

    private void Start()
    {
        timeUntilNextKick = timeBetweenKicks;
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timeUntilNextKick -= Time.deltaTime;

        if (timeUntilNextKick < 0)
        {
            Kick();
            timeUntilNextKick = timeBetweenKicks;
        }
    }

    private void Kick()
    {
        //choose aim point
        if (aimPoints.Count > 0)
        {
            var randomIndex = Random.Range(0, aimPoints.Count);
            var endPoint = aimPoints[randomIndex].position;
            var ballPos = gameObject.transform.position;
            var shotDirection = endPoint - ballPos;

            var force = shotDirection.normalized * ForceMultiplier;
            body.AddForce(force, ForceMode.Impulse);
        }
    }
}