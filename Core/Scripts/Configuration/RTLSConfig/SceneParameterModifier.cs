using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using dreamcube.unity.Core.Scripts.Components.RTLS;
using UnityEngine;
using dreamcube.unity.Core.Scripts.Util;
public class SceneSettings
{
    public float SceneWallsDepth = 3.805f;
    public float SceneWallsHeight = 2.38f;
    public float SceneWallsWidth = 3.2f;

    public float SiteWallsDepth = 3.805f;
    public float SiteWallsHeight = 2.38f;
    public float SiteWallsWidth = 3.2f;
    public float TopCameraDepthOffset = 0;
    public float TopCameraSizeMultiplier = 1;

    public override string ToString()
    {
        // hide secret when printing
        var obj = MemberwiseClone() as SceneSettings;
        return JsonUtility.ToJson(obj);
    }
}

public class SceneParameterModifier : MonoBehaviour
{
    [SerializeField] private Camera topCamera;

    //[SerializeField] private GameObject CameraMask = null;
    [SerializeField] private SceneSettings _cameraSettings = new SceneSettings();

    [SerializeField] private RTLSReceiver _rtlsReceiver;

    // we want to load these settings before log loading, so going to store some info during load
    public List<string> logs = new List<string>();

    protected void Awake()
    {
        var pathToUse = Common.SCENE_SETTINGS_MODIFIER_FILE;

        if (!File.Exists(Path.Combine(Application.streamingAssetsPath, pathToUse)))
        {
            logs.Add("No top camera configurations, using defaults");
            var settings = JsonUtility.ToJson(_cameraSettings);
            LoadSettings(settings);

            // save file with default settings
            var savePath = Path.Combine(Application.streamingAssetsPath, pathToUse);
            logs.Add($"Saving local configuration file to {savePath}");
            File.WriteAllText(savePath, settings);
        }
        else
        {
            logs.Add($"Loading top camera condiguration from {pathToUse}");
            LoadSettings(Extensions.LoadStringFromFile(pathToUse));
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Stored logs from ParameterModifier:");
        foreach (var x in logs) Debug.Log($"-{x}");
        _rtlsReceiver = GameObject.FindObjectOfType<RTLSReceiver>();
        ApplySettings();
    }

    public void ApplySettings()
    {
        // apply the settings
        topCamera.orthographicSize *= _cameraSettings.TopCameraSizeMultiplier;
        gameObject.transform.localPosition = gameObject.transform.localPosition +
                                             new Vector3(0, 0, _cameraSettings.TopCameraDepthOffset);
        //CameraMask.transform.localPosition = CameraMask.transform.localPosition + new Vector3(_cameraSettings.TopCameraMaskOffset, 0, 0);

        if (_rtlsReceiver == null) return;

        if (_cameraSettings.SiteWallsDepth < float.Epsilon || _cameraSettings.SiteWallsHeight < float.Epsilon ||
            _cameraSettings.SiteWallsWidth < float.Epsilon) return;

        var scale = new Vector3();
        scale.x = _cameraSettings.SceneWallsWidth / _cameraSettings.SiteWallsWidth;
        scale.y = _cameraSettings.SceneWallsHeight / _cameraSettings.SiteWallsHeight;
        scale.z = _cameraSettings.SceneWallsDepth / _cameraSettings.SiteWallsDepth;
        _rtlsReceiver.scale = scale;
    }

    public void ReloadSettings()
    {
        LoadSettings(Extensions.LoadStringFromFile(Common.SCENE_SETTINGS_MODIFIER_FILE));
        ApplySettings();
    }

    private void LoadSettings(string dataString)
    {
        _cameraSettings = JsonUtility.FromJson<SceneSettings>(dataString);
    }
}