using UnityEngine;

namespace Voxon
{
    public class VXGameObject : MonoBehaviour
    {

        // Lifespan Variables
        private const float MAX_LIFE_SPAN = 300.0f;
        private bool can_degen = true;
        private bool degen = false;
        private float life_span = MAX_LIFE_SPAN;

        public void Start()
        {
            // We will use this to add our components to draw list

            // VoxieDraw objects cannot degenerate
            if (gameObject.tag == "VoxieDraw")
            {
                can_degen = false;
            }

            // We always want animations to be computed (as otherwise they would only appear when a camera was active)
            if (gameObject.GetComponent<Animator>())
            {
                Animator Anima = transform.root.gameObject.GetComponent<Animator>();
                if(Anima)
                {
                    Anima.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }

                var Animators = transform.root.gameObject.GetComponentsInChildren<Animator>();
                foreach(var A in Animators)
                {
                    A.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }
            }

            foreach (Renderer child in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (child.gameObject.tag == "VoxieHide")
                {
                    continue;
                }
                if(child.gameObject.GetComponent<ParticleSystem>() || child.gameObject.GetComponent<LineRenderer>())
                {
                    continue;
                }
                // Child will add self to VXProcess _components
                if (!child.gameObject.GetComponent<VXComponent>())
                {
                    child.gameObject.AddComponent<VXComponent>();
                }                
            }

            VXProcess._gameobjects.Add(this);
        }

        void OnDestroy()
        {
            foreach (Renderer child in gameObject.GetComponentsInChildren<Renderer>())
            {
                Destroy(child.gameObject.GetComponent<VXComponent>());
            }
        }

        /// <summary>  
        ///  To reduce load on VX1, we want Drawables to be removed a few seconds off screen.
        ///  This won't impact the actual model, just stop it being computed for drawing until it reenters the scene
        ///  </summary>
        private void Update()
        {
            if (life_span <= 0)
            {
                Debug.Log("Destroying " + gameObject.name + " due to degen (out of collider for too long)");
                Destroy(this);
            }
            else if (can_degen && degen)
            {
                life_span--;
            }

        }

        /// <summary>  
        ///  Set Degen on the object; triggered true when drawable leaves capture volume, false when entering
        ///  </summary>
        public void Set_Degen(bool start_degen)
        {
            if (!start_degen)
            {
                life_span = MAX_LIFE_SPAN;
            }
            degen = start_degen;
        }
    }
}
