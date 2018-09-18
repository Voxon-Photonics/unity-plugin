using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        if (Voxon.Input.GetKey("Left"))
        {
            pos.x += 0.1f;
        }
        if (Voxon.Input.GetKey("Right"))
        {
            pos.x -= 0.1f;
        }
        if (Voxon.Input.GetKey("Up"))
        {
            pos.z -= 0.1f;
        }
        if (Voxon.Input.GetKey("Down"))
        {
            pos.z += 0.1f;
        }

        Vector3 world_rot = VXProcess.Instance._camera.transform.rotation.eulerAngles;
        if (Voxon.Input.GetKey("RotLeft"))
        {
            world_rot.y += 1f;
        }
        if (Voxon.Input.GetKey("RotRight"))
        {
            world_rot.y -= 1f;
        }
        

        VXProcess.Instance._camera.transform.rotation = Quaternion.Euler(world_rot);

        transform.position = pos;
    }
}
