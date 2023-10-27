using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBy : MonoBehaviour {

    public Vector3 AnglesRotation;


    void Update() {
        this.transform.localEulerAngles += AnglesRotation * Time.deltaTime;
    }
}
