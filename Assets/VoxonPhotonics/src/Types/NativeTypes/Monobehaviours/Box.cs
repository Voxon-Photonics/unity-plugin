using UnityEngine;


namespace Voxon
{
    public class Box : MonoBehaviour
    {
        // Editor View (Won't use after initialisation)
        [SerializeField]
        bool parented;
        [SerializeField]
        Vector3[] vPosition = new Vector3[2];
        [SerializeField]
        Color32 colour;
        [SerializeField]
        int fill;
        // Associated Models
        BoxModel boxModel;

        // Associated Views
        BoxGizmoView boxGizmoView;
        BoxView box;
        // Use this for initialization
        void Start()
        {
            boxModel = new BoxModel();
            boxModel.Position = vPosition;
            boxModel.Fill = fill;
            boxModel.SetColor(colour);

            if (parented) boxModel.Parent = gameObject;

            boxModel.Update();

            box = new BoxView(boxModel);
            boxGizmoView = new BoxGizmoView(boxModel);
        }

        private void Update()
        {
            boxModel.Update();
        }

        private void OnDrawGizmos()
        {
            if(boxGizmoView != null)
            {
                boxGizmoView.DrawGizmo();
            }
        }

        
        void OnValidate()
        {
            if(box != null)
            {
                box.Destroy();
            }

            boxModel = null;
            boxModel = new BoxModel();
            boxModel.Position = vPosition;
            boxModel.Fill = fill;
            boxModel.SetColor(colour);

            if (parented) boxModel.Parent = gameObject;

            boxModel.Update();

            box = new BoxView(boxModel);
            boxGizmoView = new BoxGizmoView(boxModel);
        }

        private void OnDisable()
        {
            box.Destroy();
        }

        private void OnDestroy()
        {
            box.Destroy();
        }

    }
}