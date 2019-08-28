using UnityEngine;

namespace Voxon.Examples.Input
{
    public class MovePit : MonoBehaviour {
        private bool _hidden;

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
            if (Voxon.Input.GetKey("Up") || Voxon.Input.GetButton("Jump"))
            {
                pos.z -= 0.1f;
            }
            if (Voxon.Input.GetKey("Down"))
            {
                pos.z += 0.1f;
            }

            Vector3 worldRot = VXProcess.Instance.EulerAngles;
            if (Voxon.Input.GetKey("RotLeft"))
            {
                worldRot.y += 1f;
            }
            if (Voxon.Input.GetKey("RotRight"))
            {
                worldRot.y -= 1f;
            }
        
            if(Voxon.Input.GetKeyDown("Hide") && !_hidden)
            {
                VXProcess.add_log_line("Hiding: " + _hidden.ToString());
                tag = "VoxieHide";
                _hidden = !_hidden;
            }
            else if(Voxon.Input.GetKeyDown("Hide"))
            {
                tag = "Untagged";
                _hidden = !_hidden;
            }

            VXProcess.Instance.EulerAngles = worldRot;

            transform.position = pos;
        }
    }
}
