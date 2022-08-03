using UnityEngine;

namespace App.Scripts.Display
{
    public class DisplaysManager : MonoBehaviour
    {
        [SerializeField] private int maxScreens = 4;

        void Start()
        {
            int displayWidth = 1920;
            int displayHeight = 1200;
            int displayOffsetX = 0;
            int displayOffsetY = 0;

            Debug.Log("Activating displays");
            Debug.Log($"{UnityEngine.Display.displays.Length} displays found");

            for (var i = 0; i < UnityEngine.Display.displays.Length; i++)
            {
                // reactivating the primary display produces occasional misalignment of camera raster
                if (i > 0 && i < maxScreens) UnityEngine.Display.displays[i].Activate();
                UnityEngine.Display.displays[i].SetParams(displayWidth, displayHeight, displayOffsetX, displayOffsetY);
            }

            Debug.Log("Display configuration complete");
        }
    }
}