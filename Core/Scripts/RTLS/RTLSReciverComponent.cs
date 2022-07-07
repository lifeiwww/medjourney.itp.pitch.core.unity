using System.Collections;
using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.Components.RTLS;
using dreamcube.unity.Core.Scripts.General;
using UnityEngine;

public class RTLSReciverComponent : Singleton<RTLSReciverComponent>
{
    // Start is called before the first frame update
    private RTLSReciverService _rtlsReciverService;


    // to display frames per second
    public static float RTLSfps;
    private float _deltaTime;
    private float _lastNewFrameRecievedTime;

    [SerializeField]
    private GameObject indicator;

    void Start()
    {
        _rtlsReciverService = new RTLSReciverService();
    }

    // Update is called once per frame
    void Update()
    {
        // calculate FPS
        if ( _rtlsReciverService!=null && _rtlsReciverService.NewData)
        {
            _lastNewFrameRecievedTime = Time.time;
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            RTLSfps = 1.0f / _deltaTime;
            _rtlsReciverService.NewData = false;
        }

        // reset if no new data is received
        if (Time.time - _lastNewFrameRecievedTime > 5)
        {
            RTLSReciverService.NumCameras=0;
            RTLSfps = 0;
        }
    }

    protected override void OnApplicationQuit()
    {
        _rtlsReciverService.Close();
        base.OnApplicationQuit();
    }
}
