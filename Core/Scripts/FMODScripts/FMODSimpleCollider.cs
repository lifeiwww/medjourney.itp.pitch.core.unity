using FMODUnity;
using UnityEngine;

public class FMODSimpleCollider : MonoBehaviour
{
    // Start is called before the first frame update
    [EventRef] public string collisionEvent;

    private void OnTriggerEnter(Collider other)
    {
        //Log.Debug($"<color=green>>other {other.tag}</color>");
        if (other.CompareTag("Now Line"))
            //Log.Debug("<color=green>>COLLIDE detected</color>");
            RuntimeManager.PlayOneShot(collisionEvent);
    }
}