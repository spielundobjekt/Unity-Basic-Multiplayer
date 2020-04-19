using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class holds information that should be accessible by a number of scripts
//some of those will be attached to this gameobject
//some of them will be children to this gameobject

    //It is often good practice to split the data that multiple scripts access
    //into its own separate script

public class PlayerData : Mirror.NetworkBehaviour
{
    public Vector3 movementDirection;
    Surroundings mySurroundings;
    public Quaternion lookRotation;

    //We do some things in Awake() because this gets called before the Start() Script of anything else is called.
    private void Awake()
    {
        Debug.Log("I am alive!");

        mySurroundings = GetComponent<Surroundings>();

        //Let's let the GameData Script know that we exist!
        GameData.instance.players.Add(this);
    }



}
