using System.IO;
using dreamcube.unity.Core.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class LoadTexture : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private string imageURL;

    [SerializeField] private Material mat;

    // Start is called before the first frame update
    private void Start()
    {
        imageURL = Path.Combine(Application.streamingAssetsPath, imageURL);
        var imageAsset = new TextAsset(imageURL);
        var tex = Extensions.LoadTexture(imageURL);
        if (image != null)
            image.texture = tex;

        if (mat != null)
            mat.mainTexture = tex;
    }
}