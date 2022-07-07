using System;
using Serilog;
using UnityEngine;

[Obsolete("Method RayCaster is obsolete.", false)]
public class RayCaster : MonoBehaviour
{
    //private readonly float _radius = 0.11f; // ball size in meters
    //private Camera cam;

    private void Start()
    {
        //cam = GetComponent<Camera>();

        ////TODO: make this less egregious 
        ////should really do this step on an "on level loaded" event"
        ////the raycaster usually loads after the surface does.
        //var surface = FindObjectOfType<Surface>();
        //if (surface.RayCaster == null) surface.RayCaster = this;
    }

    private void Awake()
    {
        Log.Debug("Ray Caster is awake");
    }

    public void CastRay(Vector3 angle)
    {
        CastHittableRay(angle);
        CastGoalVfxRay(angle);
    }

    private void CastGoalVfxRay(Vector3 angle)
    {
        /*
        // Bit shift the index of the layer (14) to get a bit mask
        var layerMask = 1 << 14; //14 is Net VFX

        RaycastHit hit;
        // Does the ray intersect any objects excluding layer mask
        if (Physics.SphereCast(transform.position, _radius, angle, out hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(transform.position, angle * hit.distance, Color.red);
            Debug.DrawRay(transform.position, angle, Color.red);

            var netVfxReceiver = hit.collider.gameObject.GetComponent<PenaltyKickNetVfxReceiver>();
            if (netVfxReceiver) netVfxReceiver.ReceiveRay(hit.point);
        }
        else
        {
            Debug.DrawRay(transform.position, angle * 1000, Color.white);
            //Log.Debug("Did not Hit");
        }
        */
    }

    private void CastHittableRay(Vector3 angle)
    {

        /*
        // Bit shift the index of the layer (8) to get a bit mask
        var layerMask = 1 << 8; //8 is hittable

        RaycastHit hit;
        // Does the ray intersect any objects excluding layer mask
        if (Physics.SphereCast(transform.position, _radius, angle, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, angle, Color.red);
            //Log.Debug("Hit Object: " + hit.collider.gameObject.name);
            var HittableElement = hit.collider.gameObject.GetComponent<BaseHittableElement>();
            if (HittableElement) HittableElement.Hit();
        }
        else
        {
            Debug.DrawRay(transform.position, angle * 1000, Color.white);
            //Log.Debug("Did not Hit");
        }
        */
    }
}