using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client;
using manutd;
using Serilog;
using UnityEngine;

public class RTLSDataSimulator : MonoBehaviour
{
    public static void SendFakeCameraStatus( bool isStartup)
    {
        var gameBayIDIsNumber = int.TryParse(ConfigManager.Instance.generalSettings.DreamCube, out var gameBayID);
        var camDataList = new List<CameraData>();
        int sampleSize = Random.Range(4, 8);
        for (int i = 0; i < sampleSize; i++)
        {
            var camData = new CameraData
            {
                CameraAlignmentStatus = (Random.Range(0, 2) == 1),
                CameraID = $"{i}",
                CameraSerialNumber = $"newSerial12354988s6{i}"
            };
            camDataList.Add(camData);
        }

        var trackingData = new TrackingSystemData
        {
            GameBayID = gameBayID,
            Cameras = camDataList
        };

        BaseSignalRClient client = FindObjectOfType<BaseSignalRClient>();
        if (client != null && gameBayIDIsNumber)
        {
            Log.Information($"Sending camera data {trackingData} ");
            Task.Run(() => { client.UpdateCameraStatus(trackingData, isStartup); });
        }
        else
            Log.Error("No pitch signalR client found");
    }
}
