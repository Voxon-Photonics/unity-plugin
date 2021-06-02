using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class VXCamera : MonoBehaviour
{
	public bool uniformScale = true;
	public int baseScale = 1;
	public Vector3 vectorScale = Vector3.one;

	public bool loadViewFinder = false;
	public Vector3 ViewFinderDimensions = new Vector3(1, 0.4f, 1);

	VxViewFinder view_finder;

	void UpdatePerspective()
	{
		if (uniformScale){
			this.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
		} else {
			this.transform.localScale = vectorScale;
		}
	}

	void ViewFinderCheck()
	{
		if (view_finder == null)
		{
			view_finder = GetComponentInChildren<VxViewFinder>();

			// Handle Corrupted Prefab
			if (view_finder == null)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;

				// This value should be loaded from config data. Defaulting to current standard
				go.transform.localScale = ViewFinderDimensions;

				go.transform.parent = gameObject.transform;
				// Add a view finder
				go.name = "view_finder";
				go.AddComponent<VxViewFinder>();
			}
		}
	}

	void UpdateViewFinder()
	{
		ViewFinderCheck();

		view_finder.SetAspectRatio(ViewFinderDimensions);
	}

	private void Awake()
	{

#if UNITY_EDITOR
		// Load AspectRatio for ViewFinder
		UpdateViewFinder();
		UpdatePerspective();
#endif
	}
	// Start is called before the first frame update
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
		UpdateViewFinder();
		UpdatePerspective();
#endif
	}

	Matrix4x4 GetMatrix()
	{
		ViewFinderCheck();

		return view_finder.transform.worldToLocalMatrix;
	}
}
