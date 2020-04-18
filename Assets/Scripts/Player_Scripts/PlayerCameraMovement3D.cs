using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Script moves the Camera for the Player when they walk around
/// </summary>

public class PlayerCameraMovement3D : MonoBehaviour
{
    PlayerData myPlayer;    //a connection to the Component that has important Data for the Player
    Camera myCamera;        //a variable that we will use to directly manipulate the Main Camera on the local Player's screen

    public float eyeOffset = 0.25f;

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
        //is this player controlled by my local computer?
        // if not (!), then please don't do anything here, because we don't want other players controlling our camera
        //otherwise all players move the camera!
        if (!myPlayer.isLocalPlayer)
        {
            return;
        }

        //In 3D, we want to rotate the camera according to where the player is looking
        //so first we need to get the player transform from our PlayerData Script, and then align the Camera transform with that.
        myCamera.transform.position = myPlayer.transform.position;

        //but! We want to look out of the eyes, and myPlayer.transform.position gives us the location of the players tummy.
        //so we need to offset the camera a bit, to make it seem like we are at eye level
        myCamera.transform.position += new Vector3(0, eyeOffset, 0);

        //Then: use our player's lookRotation to rotate the camera
        //We don't use our player's actual rotaton, because the "body" of our player should always stay upright
        myCamera.transform.rotation = myPlayer.lookRotation;
    }
}
