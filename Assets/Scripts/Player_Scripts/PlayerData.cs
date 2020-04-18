using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Mirror.NetworkBehaviour
{
    public Vector3 movementDirection;
    Surroundings mySurroundings;
    public Camera myCamera;
    public Quaternion lookRotation;

    //We do some things in Awake() because this gets called before the Start() Script of anything else is called.
    private void Awake()
    {
        Debug.Log("I am alive!");

        mySurroundings = GetComponent<Surroundings>();
    }


}
