using dreamcube.unity.Core.Scripts.Components.RTLS;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.BallMovement
{
    public class MoveBallWithRTLS : MonoBehaviour
    {
        [SerializeField] bool UseRTLS = true;

        private void Awake()
        {
            UseRTLS = ConfigManager.Instance.generalSettings.UseRTLS;
        }

        void LateUpdate() 
        {
            if (UseRTLS) {

                var pos = RTLSReceiver.Instance.GetPosition();

                // do not update ball position if RTLS server is not up
                if( pos.magnitude > Mathf.Epsilon )
                {
                    transform.position = pos;
                    transform.rotation = Quaternion.identity;

                }
            }
        }
    }
}
