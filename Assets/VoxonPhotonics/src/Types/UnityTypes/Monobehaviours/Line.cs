using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Voxon
{
    [RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
    {
        // Editor View (Won't use after initialisation)
        [SerializeField]
        bool parented;
        [SerializeField]
        Color32 colour;

        // Associated Models
        LineModel lineModel;

        // Associated Views
        LineView line;

        LineRenderer lineRenderer;
        // Use this for initialization
        void Start()
        {
            try
            {
                lineRenderer = GetComponent<LineRenderer>();
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("(" + name + ") Failed to load suitable Line", E);
                Destroy(this);
            }

            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            lineModel = new LineModel();
            lineModel.SetColor(colour);
            lineModel.Points = positions;

            if(parented) lineModel.Parent = gameObject;

            lineModel.Update();

            line = new LineView(lineModel);
        }

        // Update is called once per frame
        void Update()
        {
            lineModel.Update();
        }
    }
}