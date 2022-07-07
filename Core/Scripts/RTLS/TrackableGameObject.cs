using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using TMPro;

namespace dreamcube.unity.Core.Scripts.Components.RTLS
{

    public class TrackableGameObject : MonoBehaviour
    {
        [SerializeField] public int Id;
        [SerializeField] public int Age;
        [SerializeField] public TextMeshPro IdTextMesh;

        [ReadOnly]
        public Vector3 Position;

        private void Awake()
        {
            if (IdTextMesh == null)
                gameObject.AddComponent<TextMeshPro>();
        }

        void Update()
        {
            gameObject.transform.localPosition = Position;
        }
    }

}

