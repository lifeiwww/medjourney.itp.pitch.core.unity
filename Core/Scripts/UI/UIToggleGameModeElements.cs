using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace dreamcube.unity.DefaultResources.Scripts.UI
{
    [Serializable]
    public enum ExerciseMode
    {
        MODE_TIMEOUT, // time limited exercise
        MODE_EFFICIENCY // repetition limited exercise 
    }

    [Serializable]
    public class TypeSpecificElement
    {
        [FormerlySerializedAs("modeType")] public ExerciseMode mode;
        public List<GameObject> elements = new List<GameObject>();
    }

    [ExecuteAlways]
    public class UIToggleGameModeElements : MonoBehaviour
    {
        [SerializeField] private ExerciseMode currentMode = ExerciseMode.MODE_TIMEOUT;
        [SerializeField] private List<TypeSpecificElement> elementSet = new List<TypeSpecificElement>();

        void OnEnable()
        {
            SetMode(currentMode);
        }

        public void SetMode(ExerciseMode mode)
        {
            currentMode = mode;
            foreach (var element in elementSet)
            {
                var activate = element.mode == mode;
                ActivateSet(element,activate);
            }
        }

        private static void ActivateSet(TypeSpecificElement element, bool bActive)
        {
            foreach (var obj in element.elements)
            {
                obj.SetActive(bActive);
            }
        }

        private void OnValidate()
        {
            SetMode(currentMode);
        }
    }
}