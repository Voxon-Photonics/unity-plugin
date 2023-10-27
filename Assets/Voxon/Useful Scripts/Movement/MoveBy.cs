using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBy : MonoBehaviour {

    public Vector3 MoveVector;


    void Update() {
        this.transform.Translate(MoveVector * Time.deltaTime);
    }
}
