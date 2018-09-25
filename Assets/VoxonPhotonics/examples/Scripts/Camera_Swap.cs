using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Swap : MonoBehaviour {

    int camera_index = 0;
    public GameObject [] cameras;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Voxon.Input.GetKeyDown("CameraSwapUp"))
        {
            camera_index = Mathf.Min(++camera_index, cameras.Length - 1);
            VXProcess.Instance.SetCamera(cameras[camera_index]);
            
        }

        if (Voxon.Input.GetKeyDown("CameraSwapDown"))
        {
            camera_index = Mathf.Max(--camera_index, 0);
            VXProcess.Instance.SetCamera(cameras[camera_index]);
            
        }
    }
}
