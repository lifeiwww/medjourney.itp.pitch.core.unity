using dreamcube.unity.Core.Scripts.AssetLoading;
using dreamcube.unity.Core.Scripts.Util;
using UnityEngine;
public class SimpleSceneSwitcher : MonoBehaviour
{
    public void ShowCubemap()
    {
        _ = SceneLoader.SwitchScenes("ClusterRender", "CubemapRender");
    }

    public void ShowCluster()
    {
        _ = SceneLoader.SwitchScenes("CubemapRender", "ClusterRender");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
