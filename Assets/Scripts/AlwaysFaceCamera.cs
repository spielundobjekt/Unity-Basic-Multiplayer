using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a simple script that sets the transform of the gameobject it is attached to to always face the main Camera

public class AlwaysFaceCamera : MonoBehaviour
{
    Camera myCamera;

    // Start is called before the first frame update
    void Start()
    {
        //Make sure we can access the information of the Main Camera
        myCamera = Camera.main;
    }



    // Update is called once per frame
    void Update()
    {
        //here we go.
        transform.rotation = Quaternion.LookRotation(myCamera.transform.forward, new Vector3(0,1,0));    
    }
}
