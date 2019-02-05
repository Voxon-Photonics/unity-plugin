using UnityEngine;


namespace Voxon
{
    public class Sphere : MonoBehaviour
    {
        // Editor View (Won't use after initialisation)
        [SerializeField]
        bool parented;
        [SerializeField]
        Vector3[] vPosition = new Vector3[1];
        [SerializeField]
        Color32 colour;
        [SerializeField]
        int fill;
        [SerializeField]
        float radius;
        // Associated Models
        SphereModel sphereModel;

        // Associated Views
        SphereView sphere;
        // Use this for initialization
        void Start()
        {
            sphereModel = new SphereModel();
            sphereModel.Position = vPosition;
            sphereModel.Fill = fill;
            sphereModel.Radius = radius;
            sphereModel.SetColor(colour);

            if (parented) sphereModel.Parent = gameObject;

            sphereModel.Update();

            sphere = new SphereView(sphereModel);
        }

        private void Update()
        {
            sphereModel.Update();
        }

        void OnValidate()
        {
            if (sphere != null)
            {
                sphere.Destroy();
            }

            sphereModel = null;
            sphereModel = new SphereModel();
            sphereModel.Position = vPosition;
            sphereModel.Fill = fill;
            sphereModel.Radius = radius;
            sphereModel.SetColor(colour);

            if (parented) sphereModel.Parent = gameObject;

            sphereModel.Update();

            sphere = new SphereView(sphereModel);
        }

        private void OnDisable()
        {
            sphere.Destroy();
        }

        private void OnDestroy()
        {
            sphere.Destroy();
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = new Color(colour.r, colour.g, colour.b);
            Gizmos.DrawSphere(vPosition[0]+gameObject.transform.position, radius*5);
        }
    }
}