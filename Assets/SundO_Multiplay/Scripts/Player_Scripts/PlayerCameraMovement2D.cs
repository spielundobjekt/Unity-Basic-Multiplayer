using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Script moves the Camera for the Player
/// The main Camera (as set in GameData.mainCamera) follows them somewhat lazyly
/// 
/// We presume this script is a Child of the GameObject that holds the PlayerData Script 
/// </summary>
public class PlayerCameraMovement2D : MonoBehaviour
{
    PlayerData myPlayer;    //a connection to the Component that has important Data for the Player
    
    public float cameraLazyness = 200.0f;   //the higher the number, the lazyer the camera is in following the player

    public Vector3 cameraMoveDirection; //the distance, but also the direction of where the camera should move

    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    void Start()
    {
        //first, make sure we can access all the variables of our PlayerData Script
        myPlayer = GetComponentInParent<PlayerData>();


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
        cameraMoveDirection = myPlayer.transform.position - GameData.instance.mainCamera.transform.position;

        //now let's move the camera a little bit in that direction.
        //let's say, depending on the lazyness, we only move a fraction of the actual distance.
        GameData.instance.mainCamera.transform.Translate(1.0f/cameraLazyness * cameraMoveDirection);
    }
}
