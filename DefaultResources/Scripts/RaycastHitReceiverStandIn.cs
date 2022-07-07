using System;
using System.Collections;
using System.Collections.Generic;
using App.TTW.Scripts;
using DG.Tweening;
using UnityEngine;

public class RaycastHitReceiverStandIn : MonoBehaviour
{

    // Assign a prefab to debug impact point 
    public GameObject debugHitMarker;
    public GameObject visualObject;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    //hitPoint is in world space right now
    public virtual void HitByRaycast(Vector3 hitPoint)
    {
        if (debugHitMarker)
        {
            GameObject marker = Instantiate(debugHitMarker, this.transform.parent);
            marker.AddComponent<DestroyAfterSeconds>();
            marker.transform.forward = transform.transform.parent.forward;
            marker.transform.position = hitPoint - marker.transform.forward * 0.01f;
        }

        if( visualObject) visualObject
            .transform.DOLocalRotate(360f * Vector3.right, 1.0f, RotateMode.LocalAxisAdd)
            .OnComplete(()=> visualObject.transform.localRotation=Quaternion.identity);
    }
}