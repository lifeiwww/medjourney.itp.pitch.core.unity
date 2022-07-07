using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace dreamcube.unity.DefaultResources.Scripts.UI
{

    public class UIToggleOverlayElements : MonoBehaviour
    {
        [SerializeField] private List<GameObject> elementSet = new List<GameObject>();

        void OnEnable()
        {
            SetOverlayState(-1);
        }

        public void SetOverlayState(int overlayIndex )
        {
            for (var i=0; i<elementSet.Count; i++)
            {
                var activate = i == overlayIndex;
                elementSet[i].SetActive(activate);
            }
        }
    }
}