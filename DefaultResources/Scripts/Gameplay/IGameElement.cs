using UnityEngine;

namespace App.TTW.Scripts.GameElements
{
    public interface IGameElement
    {
        public GameObject VisualObject { get; set; }
        public GameElementModel GameElementModel { get; set; }

        bool IsActive { get; set; }
        bool CollisionsEnabled { get; set; }

        public void Setup();
        public void Activate();
        public void Deactivate();
    }
}
