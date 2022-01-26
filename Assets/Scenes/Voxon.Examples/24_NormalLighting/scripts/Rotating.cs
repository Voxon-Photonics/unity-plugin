using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    float totalTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;

        Vector3 pos = gameObject.transform.position;

        pos.x = Mathf.Cos(totalTime);
        pos.y = Mathf.Sin(totalTime);

        gameObject.transform.position = pos;
    }
}
