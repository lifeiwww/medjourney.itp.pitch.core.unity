using System.Collections;
using UnityEngine;

public class UIRenderTextureClear : MonoBehaviour
{
    [SerializeField] private RenderTexture UIRenderTexture;

    private void Start()
    {
        if (!Application.isEditor) StartCoroutine(ClearTexture());
    }

    private IEnumerator ClearTexture()
    {
        while (true)
        {
            ClearOutRenderTexture(UIRenderTexture);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        var rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
}