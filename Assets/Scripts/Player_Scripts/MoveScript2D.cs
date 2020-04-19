using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//The Move Script Handles Movement in 2D for Players
//locally controlled through Keyboard or the On-Screen UI 
//it also keeps track of whether or not the player has moved in the last frame 
//and updates the PlayerData Script accordingly

public class MoveScript2D : MonoBehaviour
{
    public PlayerData myPlayer;         //a variable that references our PlayerData Script
                                        //we use this Script to store and retrieve information about the player
                                        //from a whole bunch of scripts

    public float moveSpeed = 1.0f;      //a factor for modifying the Movement Value we get from Input devices

    public Vector3 currentMovement;     //this variable contains the amount of units we have moved, no matter if we are a local Player or a networked player

    public Vector3 oldLocation;         //we use this to figure out if we have moved during the last frame. 
                                        //this is useful if we do not control this Player from our local inputs,


    // Use this for initialization
    void Start()
    {
        myPlayer = GetComponentInParent<PlayerData>();
    }

    // FixedUpdate is called once per frame
    // it is more deterministic in its execution time and order than the regular "Update"
    // which is why it is often used for movement and Physics related things
    // here we describe in which order we process inputs and the move Objectss
    void FixedUpdate()
    {
        //first, we make sure that we are not moving currently
        currentMovement = currentMovement * 0.0f;

        //Only Process Movement from Input for the local Player, 
        //otherwise all player Objects would move!
        if (myPlayer.isLocalPlayer)
        {
            ProcessMovement();
            
        }
        //All other Player Objects still need to animate! 
        //So they need to figure out if they have changed their position from the last frame to this frame 
        else
        {
            currentMovement = oldLocation - transform.position;
            oldLocation = transform.position;
        }

        //finally, make sure all relevant information is put into the PlayerData Object 
        //that all other scripts have access to (like the Animation Script).
        UpdatePlayerData();
    }

    

    //player moves via keyboard or gamepad
    void ProcessMovement()
    {
        // Movement per input direction - Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            ProcessDeviceInput();
        }
        //Movement via On-Screen-Interface (Web and Cellphone
        else
        {
            ProcessUIInput();
        }

        MoveCharacter();             
    }

    //Here we process Input for Keyboard and maybe Gamepad
    void ProcessDeviceInput()
    {
        currentMovement = new Vector3( moveSpeed * Input.GetAxis("Horizontal"), 0.0f, moveSpeed * Input.GetAxis("Vertical"));
    }


    //player moves via on-screen-interface
    public void ProcessUIInput()
    {
        currentMovement = moveSpeed * PlayerUIBridge.uiMov;
    }

    //After we have figured out what the input wants us to do, 
    //we can now start actually moving our player Object
    void MoveCharacter()
    {
        //make sure that we don't move the transform of our own GameObject, 
        //but the Transform of the GameObject containing the PlayerData Script
        myPlayer.transform.position = myPlayer.transform.position + Time.deltaTime * currentMovement;
    }

    //No matter if you are the Locally controlled Player Object, or any other Player Object
    //it is important to update all movement related Data in the PlayerData Script,
    //so all other scripts can access it and work with it
    void UpdatePlayerData()
    {
        //in order to calculate if and how much we have moved, 
        //we need to store our actual movement in space in the movementDirection variable in PlayerData
        myPlayer.movementDirection = currentMovement.normalized;
    }
}
     
