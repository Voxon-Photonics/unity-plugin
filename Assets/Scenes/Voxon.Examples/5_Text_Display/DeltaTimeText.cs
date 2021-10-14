using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaTimeText : MonoBehaviour
{
	Voxon.VXTextComponent text;
    // Start is called before the first frame update
    void Start()
    {
		text = GetComponent<Voxon.VXTextComponent>();

		// text.forceUpdatePerFrame = false;
    }

    // Update is called once per frame
    void Update()
    {
		if (text.forceUpdatePerFrame)
		{
			text.text = Time.deltaTime.ToString();
		}
		else
		{
			text.SetString(Time.deltaTime.ToString());
		}
    }
}
