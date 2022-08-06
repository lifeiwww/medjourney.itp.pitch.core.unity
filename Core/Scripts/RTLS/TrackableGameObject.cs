using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using TMPro;

namespace dreamcube.unity.Core.Scripts.Components.RTLS
{

    public class TrackableGameObject : MonoBehaviour
    {
        [SerializeField] public string Id;
        [SerializeField] public int Age;
        [SerializeField] public TextMeshProUGUI IdTextMesh;

        [ReadOnly]
        public Vector3 Position;

        void Update()
        {
            gameObject.transform.localPosition = Position;
        }
    }

}

