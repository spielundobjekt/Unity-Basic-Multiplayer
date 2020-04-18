using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Mirror.NetworkBehaviour
{
    public Vector3 movementDirection;
    Surroundings mySurroundings;
    public Camera myCamera;


    private void Start()
    {
        mySurroundings = GetComponent<Surroundings>();
    }

}
