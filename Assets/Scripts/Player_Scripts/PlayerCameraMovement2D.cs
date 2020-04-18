using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Script moves the Camera for the Player when they walk around
/// </summary>

public class PlayerCameraMovement2D : MonoBehaviour
{
    PlayerData myPlayer;    //a connection to the Component that has important Data for the Player
    Camera myCamera;        //a variable that we will use to directly manipulate the Main Camera on the local Player's screen

    public float cameraLazyness = 200.0f;   //the higher the number, the lazyer the camera is in following the player

    // Start is called before the first frame update
    void Start()
    {
        //first, make sure we can access all the variables of our PlayerData Script
        myPlayer = GetComponentInParent<PlayerData>();

        //then, make sure we can access the main camera in the scene
        myPlayer.myCamera = Camera.main;
        myCamera = myPlayer.myCamera;

    }

    // Update is called once per frame
    void Update()
    {
        //only move the main camera if the player that moves is controlled by the local computer, 
        //otherwise all players move the camera!
        if (!myPlayer.isLocalPlayer)
        {
            return;
        }

        //calculate camera and player offset - how close is the player to the center of the camera?
        //because we should move the camera in that direction then.
        Vector3 cameraMoveDirection = transform.position - myCamera.transform.position;

        //not interested in moving in the z-Axis, so we pretend there's no difference
        cameraMoveDirection.y = 0;

        //now let's move the camera a little bit in that direction.
        //let's say, depending on the lazyness, we only move a fraction of the actual distance.
        myPlayer.myCamera.transform.Translate(1.0f/cameraLazyness * cameraMoveDirection);
    }
}
