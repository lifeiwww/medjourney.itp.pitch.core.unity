using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRaycasterStandIn : MonoBehaviour
{
    public GameObject raycastMarker;
    private float _radius = 0.11f; //ball size in meters

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            //draw debug marker
            if (raycastMarker)
            {
                GameObject marker = GameObject.Instantiate(raycastMarker, this.transform);
                marker.transform.position = other.GetContact(0).point;
                marker.transform.forward = other.GetContact(0).normal;
            }

            //just assuming there's at least one contact point
            CastRay(other.GetContact(0));
        }
    }

    private void CastRay(ContactPoint contactPoint)
    {
        int layerMask = 1 << 7; //Hittable layermask is 13

        //start the raycast from a little behind the hit point to forgive the placement a bit
        Vector3 startPoint = contactPoint.point - (contactPoint.normal * 0.2f);
        //Vector3 startPoint = contactPoint.point;

        RaycastHit hit;
        if (Physics.SphereCast(startPoint, _radius, contactPoint.normal, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(startPoint, contactPoint.normal, Color.green, 1f);
            var hittable = hit.collider.gameObject.GetComponent<RaycastHitReceiverStandIn>();
            if (hittable != null)
            {
                hittable.HitByRaycast(hit.point);
            }
        }
        else
        {
            //didn't hit anything
            Debug.DrawRay(startPoint, contactPoint.normal, Color.white, 1f);
        }
    }
}
