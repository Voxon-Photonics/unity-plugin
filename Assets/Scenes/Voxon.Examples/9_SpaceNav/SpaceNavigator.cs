using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class SpaceNavigator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
        var position = VXProcess.Runtime.GetSpaceNavPosition();
        var rotation = VXProcess.Runtime.GetSpaceNavRotation();

        var v3pos = transform.position;
        if (position != null)
        {
            v3pos.x += position[0]/350;
            v3pos.y -= position[2]/350;
            v3pos.z -= position[1]/350;
            transform.position = v3pos;    
        }

        if (rotation != null)
        {
            var v3rot = new Vector3(rotation[0]/35,rotation[2]/35,-rotation[1]/35);
            transform.Rotate(v3rot);    
        }

        if (Voxon.Input.GetSpaceNavButton("LeftButton") && Voxon.Input.GetSpaceNavButton("RightButton"))
        {
            VXProcess.Instance.Camera.transform.position = Vector3.zero;
            VXProcess.Instance.Camera.transform.rotation = Quaternion.identity;
        }
    }
}
