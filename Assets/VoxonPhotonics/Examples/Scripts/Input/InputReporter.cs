using UnityEngine;

public class InputReporter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        int but = (int)InputController.GetButton("Jump", 1);
        for (int i = 0; i < 4; i++)
        {
            if (VXProcess.runtime.GetButtonDown(but, i))
            {
                VXProcess.Instance.add_log_line("Player " + i);
            }
        }
	    
	}
}
