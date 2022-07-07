using UnityEngine;

namespace App.TTW.Scripts
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float lifetime = 2f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
