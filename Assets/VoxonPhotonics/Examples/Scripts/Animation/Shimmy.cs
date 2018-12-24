using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Shimmy a script to rotate and/or shimmy an object 
* Matthew Vecchio for VOXON
* v 1.0
* 
*/
public class Shimmy : MonoBehaviour {

    [Tooltip("The speed of rotation 100 is default. Value are 0 - infinity")]
    public float speed = 100; 
    public enum direction { NONE, LEFT, RIGHT, UP, DOWN, FORWARD, BACKWARD,  }

    [Tooltip("The direction you wish the object to rotate")]
    public direction directionSelect = new direction();
    [Tooltip("The 2nd direction you wish the object to rotate")]
    public direction directionSelectSub = new direction();

    [Tooltip("Enable this to have the object reverse its direction after a set point")]
    public bool directionSwitch;

    [Tooltip("The first point to switch the direction of the object. Value is between 0 and 1")]
    public float changeThreshold1 = 0.5f;
    [Tooltip("The second point to switch the direction of the object. Value is between 0 and 1")]
    public float changeThreshold2 = 0.5f;


    // Use this for initialization
    void Start () {
       
    }

    // Update is called once per frame
    void Update() {

        if (directionSwitch)
        {
            switch (directionSelect)
            {

                case direction.RIGHT:
                    if (transform.rotation.x > changeThreshold1)
                    {

                        directionSelect = direction.LEFT;
                    }
                    break;
                case direction.LEFT:
                    if (transform.rotation.x < -changeThreshold2)
                    {

                        directionSelect = direction.RIGHT;
                    }
                    break;
                case direction.UP:
                
                    if (transform.rotation.y > changeThreshold1)
                    {
                       
                        directionSelect = direction.DOWN;
                    }
                    break;
                case direction.DOWN:
                  
                    if (transform.rotation.y  < -changeThreshold2)
                    {
                      
                        directionSelect = direction.UP;
                    }
                    break;
                case direction.FORWARD:
                    if (transform.rotation.z > changeThreshold1)
                    {

                        directionSelect = direction.BACKWARD;
                    }
                    break;
                case direction.BACKWARD:
                    if (transform.rotation.z < -changeThreshold2)
                    {

                        directionSelect = direction.FORWARD;
                    }
                    break;

            }

            switch (directionSelectSub)
            {

                case direction.RIGHT:
                    if (transform.rotation.x > changeThreshold1)
                    {

                        directionSelectSub = direction.LEFT;
                    }
                    break;
                case direction.LEFT:
                    if (transform.rotation.x < -changeThreshold2)
                    {

                        directionSelectSub = direction.RIGHT;
                    }
                    break;
                case direction.UP:

                    if (transform.rotation.y > changeThreshold1)
                    {

                        directionSelectSub = direction.DOWN;
                    }
                    break;
                case direction.DOWN:

                    if (transform.rotation.y < -changeThreshold2)
                    {

                        directionSelectSub = direction.UP;
                    }
                    break;
                case direction.FORWARD:
                    if (transform.rotation.z > changeThreshold1)
                    {

                        directionSelectSub = direction.BACKWARD;
                    }
                    break;
                case direction.BACKWARD:
                    if (transform.rotation.z < -changeThreshold2)
                    {

                        directionSelectSub = direction.FORWARD;
                    }
                    break;

            }
        }


        switch (directionSelect) {

            case direction.RIGHT:
            transform.Rotate(Vector3.right * speed * Time.deltaTime);
                 break;
            case direction.LEFT:
            transform.Rotate(Vector3.left * speed * Time.deltaTime);
                 break;
            case direction.UP:
                transform.Rotate(Vector3.up * speed * Time.deltaTime);
                break;
            case direction.DOWN:
                transform.Rotate(Vector3.down * speed * Time.deltaTime);
                break;
            case direction.FORWARD:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
            case direction.BACKWARD:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

        }

        switch (directionSelectSub)
        {

            case direction.RIGHT:
                transform.Rotate(Vector3.right * speed * Time.deltaTime);
                break;
            case direction.LEFT:
                transform.Rotate(Vector3.left * speed * Time.deltaTime);
                break;
            case direction.UP:
                transform.Rotate(Vector3.up * speed * Time.deltaTime);
                break;
            case direction.DOWN:
                transform.Rotate(Vector3.down * speed * Time.deltaTime);
                break;
            case direction.FORWARD:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
            case direction.BACKWARD:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

        }
    }
}
