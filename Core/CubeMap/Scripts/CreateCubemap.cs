﻿using OdinSerializer.Utilities;
using UnityEngine;

//attach this script to your camera object
[ExecuteInEditMode]
public class CreateCubemap : MonoBehaviour
{
    public RenderTexture Cubemap;
    private Camera cam;


    private readonly int faceMask =
        (1 << (int) CubemapFace.NegativeX) |
        (1 << (int) CubemapFace.NegativeY) |
        (1 << (int) CubemapFace.NegativeZ) |
        (1 << (int) CubemapFace.PositiveX) |
        (1 << (int) CubemapFace.PositiveZ);


    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (cam.SafeIsUnityNull())
        {
            Debug.Log( $"Could not find cubemap camera");
            return;
        }

        //TODO: getting a warning that GetComponent is expensive?
        cam.RenderToCubemap(Cubemap, faceMask, Camera.MonoOrStereoscopicEye.Mono);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}