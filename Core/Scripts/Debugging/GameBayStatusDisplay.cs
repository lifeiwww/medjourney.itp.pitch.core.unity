using dreamcube.unity.Core.Scripts.Components.RTLS;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.Debugging;
using TMPro;
using UnityEngine;

namespace Assets.dreamcube.itp.pitch.core.unity.Core.Scripts.Debugging
{
    public class GameBayStatusDisplay : MonoBehaviour
    {
        private float _deltaTime;
        private string _FPSString = "";
        private string Status = "";

        [SerializeField] private TextMeshProUGUI StatusTextMesh;

        private void OnEnable()
        {
            //var ballProjection = FindObjectOfType<BallHit>();
            //if( ballProjection ) ballProjection.ActivateBallProjection(true);
            var ballMarkers = FindObjectsOfType<BallToWallProjectionMarker>(true);
            foreach (var marker in ballMarkers)
                marker.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            //var ballProjection = FindObjectOfType<BallHit>();
            //if (ballProjection) ballProjection.ActivateBallProjection(false);
            var ballMarkers = FindObjectsOfType<BallToWallProjectionMarker>();
            foreach (var marker in ballMarkers)
                marker.gameObject.SetActive(false);

        }

        private void Update()
        {
            GetFrameRate();
            var stats = GenerateStatusString();

            // calculate framerate
            if (stats != Status)
            {
                StatusTextMesh.text = stats;
                Status = stats;
            }
        }

        private void GetFrameRate()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            var sec = _deltaTime * 1000.0f;
            var fps = 1.0f / _deltaTime;
            _FPSString = $"{sec:0:0} ms ({fps:0.} fps)";
        }

        private string GenerateStatusString()
        {
            var tempStatus = "GameBay Status: \n\n";
            tempStatus += $"FPS: {_FPSString}\n";
            tempStatus += $"RTLS LocalIP: {ConfigManager.Instance.generalSettings.RTLSLocalIP}\n";
            tempStatus += $"RTLS RemoteIP: {ConfigManager.Instance.generalSettings.RTLSRemoteIP}\n";
            tempStatus += $"RTLS FPS: {RTLSReceiver.RTLSfps} \n";
            tempStatus += $"RTLS cameras: {RTLSReceiver.NumCameras}\n";
            tempStatus += $"RTLS objects detected: {RTLSReceiver.NumTrackables}\n";

            return tempStatus;
        }
    }
}