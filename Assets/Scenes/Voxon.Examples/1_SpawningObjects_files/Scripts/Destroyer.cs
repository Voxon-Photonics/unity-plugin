using UnityEngine;

namespace  Voxon.Examples._1_SpawningObject
{
    /// <summary>
    /// Class to destroy game objects which trigger collisions with the attached game object
    /// </summary>
    public class Destroyer : MonoBehaviour {
        /// <summary>
        /// Event to trigger destruction of colliding entity
        /// </summary>
        /// <param name="collision"></param>
    
        private void OnCollisionEnter(Collision collision)
        {
            Destroy(collision.gameObject);
        }
    }
}

