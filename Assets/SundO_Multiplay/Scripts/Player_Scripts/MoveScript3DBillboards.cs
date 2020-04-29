using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Move Script Handles Movement in 3D for Players
/// locally controlled through Mouse and Keyboard or the On-Screen UI 
/// it also keeps track of whether or not the player has moved in the last frame 
/// and updates the PlayerData Script accordingly
/// </summary>

public class MoveScript3DBillboards : MonoBehaviour
{
    public PlayerData myPlayer;         //a variable that references our PlayerData Script
                                        //we use this Script to store and retrieve information about the player
                                        //from a whole bunch of scripts

    public float moveSpeed = 1.0f;      //a factor for modifying the Movement Value we get from Input devices
    public float lookSpeed = 3.0f;      //a factor for modifying the Rotational speed we get from Input devices

    public Vector3 currentMovement;     //this variable contains the amount of units we have moved, no matter if we are a local Player or a networked player
    
    public Vector3 oldLocation;         //we use this to figure out if we have moved during the last frame. 
                                        //this is useful if we do not control this Player from our local inputs,
                                        
    public Vector2 lookInput;           //here we store the info we get from the mouse, to look around
    public Vector3 movementInput;       //here, we store the information we get from the keyboard and/or gamepad



    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    void Start()
    {
        //connect to PlayerData
        myPlayer = GetComponentInParent<PlayerData>();        
    }

    //--------------------------------------
    // FixedUpdate is called once per frame
    // it is more deterministic in its execution time and order than the regular "Update"
    // which is why it is often used for movement and Physics related things
    // here we describe in which order we process inputs and the move Objectss
    //--------------------------------------
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
        }

        currentMovement = oldLocation - myPlayer.transform.position;
        oldLocation = myPlayer.transform.position;

        //finally, make sure all relevant information is put into the PlayerData Object 
        //that all other scripts have access to (like the Animation Script).
        UpdatePlayerData();
    }



    //------------------------------------------------------------------------------------------------------------------------
    // I personally like to first read Start() and Update() so that I can get an idea of what this Script is doing
    // then i can delve deeper and figure out how individual functions work.
    // For your own code, it is good practice to keep Start() and Update() as readable as possible! 
    //
    // If I have time (and I don't always do), I try to order the function in the same order that they are called in.
    //------------------------------------------------------------------------------------------------------------------------




    //--------------------------------------
    //We find out which input type the player has used
    //Only Local Player Processes Movement Inputs via Keyboard, Mouse, or On-Screen UI!
    //--------------------------------------
    void ProcessMovement()
    {


        Debug.Log(transform.forward);

        // Movement per input direction - relevant if input is Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Mouse X") !=0 || Input.GetAxis("Mouse Y") != 0)
        {
            ProcessDeviceInput();
        }
        //Movement via On-Screen-Interface (Web and Cellphone, in case there was no movement from Input devices)
        else
        {
            ProcessUIInput();
        }

        //after figuring out the inputs, let's move the character
        //Attention! This also only gets called for the local player!
        MoveCharacter();             
    }

    //--------------------------------------
    //Here we process Input for Mouse, Keyboard and maybe later, Gamepad
    //--------------------------------------
    void ProcessDeviceInput()
    {
        //In 3D, our movement is always relative to where we look! 

        //So we have to calculate our looking direction, and then move accordingly!
        //(This following part of the script is adapted from here: https://www.reddit.com/r/Unity3D/comments/8k7w7v/unity_simple_mouselook/ )
        lookInput.y += Input.GetAxis("Mouse X");
        lookInput.x += -Input.GetAxis("Mouse Y");

        //this next line makes sure that our rotation never goes beyond a certain value (so we cannot fully rotate our head all the way up or down)
        lookInput.x = Mathf.Clamp(lookInput.x, -15f, 15f);

        //and let's calculate all three rotation values for all our three possible rotation axes 
        //let's use a local Variable to store the information we get from the mouse
        Vector3 xyzAngleDegrees = new Vector3(0, 0, 0);
        //(we don't want to rotate around the z-Axis!)
        xyzAngleDegrees = new Vector3(lookInput.x, lookInput.y, 0.0f) * lookSpeed;

        //finally, store our new look value in the PlayerData Script, so other scripts can access it as well!
        //we can store it as a Quaternion, which is Black Magic (tm), but basically describes a rotation in 3dimensional space in 4 values.
        //thankfully, unity gives us a possibility to transform from 3 values - degrees of rotation around different axis - to this magical data type
        myPlayer.lookRotation = Quaternion.Euler(xyzAngleDegrees);

        //notice how we do not want to move in the y-Axis
        movementInput = new Vector3(moveSpeed * Input.GetAxis("Horizontal"), 0.0f, moveSpeed * Input.GetAxis("Vertical"));
    }


    //--------------------------------------
    //Here we Process Input from the On-Screen UI
    //--------------------------------------
    public void ProcessUIInput()
    {
        //for the UI Movement, we will be doing some old-school like roation and movement

        //we know that PlayerUIBridge.uiMov has the values we need for UI-Interface Movement
        //now we need to interpret them like this:
        //
        //1. we want to rotate left and right when the left and right arrow is pressed
        //2. we want to move "forward" when the up button is pressed and "backwards" when the down button is pressed


        //first, let's do rotation left and right, using similar code to what we have with mouseLook
        lookInput.y += lookSpeed * 0.1f * PlayerUIBridge.uiMov.x;

        Vector3 xyzAngleDegrees = new Vector3(0, 0, 0);
        //(we don't want to rotate around the z-Axis!)
        xyzAngleDegrees = new Vector3(lookInput.x, lookInput.y, 0.0f) * lookSpeed;

        //finally, store our new look value in the PlayerData Script, so other scripts can access it as well!
        //we can store it as a Quaternion, which is Black Magic (tm), but basically describes a rotation in 3dimensional space in 4 values.
        //thankfully, unity gives us a possibility to transform from 3 values - degrees of rotation around different axis - to this magical data type
        myPlayer.lookRotation = Quaternion.Euler(xyzAngleDegrees);

        // then, let's do forward and backward, also similar to what we do in the keyboard movement:
        movementInput = new Vector3(0.0f, 0.0f, moveSpeed * PlayerUIBridge.uiMov.z);

        //remember that the actual movement of the player will be handled after this function
        //so we are pretty much done now.
    }

    //--------------------------------------
    //After we have figured out what the input wants us to do, 
    //we can now start actually moving our player Object
    //--------------------------------------

    //BEWARE! There is a Script on this GameObject called "AlwaysFaceCamera" that will change our 
    //rotation to always be in line with the camera's!
    //This is important to understand why the following code works!
    void MoveCharacter()
    {
        
        //instead of moving our player in absolute values, we need to move it relative to where she is looking
        //Unity (thankfully) gives us a Vector pointing in the direction we would describe as "forward"
        // we can add a fraction of that vector to our current position to move forward
        currentMovement = transform.forward * Time.deltaTime * movementInput.z * moveSpeed;

        //Similarly, Unity gives us a vector that points 90 degrees sideways (to the "right"). 
        //we can add a fraction of this vector to our current position to move from side to side
        currentMovement += transform.right * Time.deltaTime * movementInput.x * moveSpeed;

        //make sure that we never loose the ground beneath our feet!
        currentMovement.y = 0;

        //and now, after we did all of the movement, let's set the y-Value again to what it was before
        myPlayer.transform.position += currentMovement;

    }


    //--------------------------------------
    //No matter if you are the Locally controlled Player Object, or any other Player Object
    //it is important to update all movement related Data in the PlayerData Script,
    //so all other scripts can access it and work with it
    //--------------------------------------
    void UpdatePlayerData()
    {
        //in order to calculate if and how much we have moved, 
        //we need to store our actual movement in space in the movementDirection variable in PlayerData
        myPlayer.movementDirection = currentMovement.normalized;
    }
}
     
