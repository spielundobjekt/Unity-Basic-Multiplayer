using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the base class for all MoveScripts
// It makes sure our Players are only locally controlled through Keyboard or the On-Screen UI 
// it also keeps track of whether or not the player has moved in the last frame 
// and updates the PlayerData Script accordingly
// 
// The contents of the specific functions should be written in Scripts derived from this script


public class MoveScriptBase : MonoBehaviour
{
    public PlayerData myPlayer;         //a variable that references our PlayerData Script
                                        //we use this Script to store and retrieve information about the player
                                        //from a whole bunch of scripts

    public float moveSpeed = 1.0f;      //a factor for modifying the Movement Value we get from Input devices

    public Vector3 currentMovement;     //this is a Vector3 to use for processing Input in various functions (see MoveScript2D for an example)
                                        //this variable is used in ProcessDeviceInput(), ProcessUIInput(), MoveCharacter() and UpdatePlayerData()
                            

    public Vector3 oldLocation;         //we use this to figure out if we have moved during the last frame. 
                                        //this is useful if we do not control this Player from our local inputs,


    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    void Start()
    {
        SetupMovement();
    }

    //--------------------------------------
    //we create our own Setup function that is called from Start, 
    //so that the overriding becomes clearer in Child Classes
    //--------------------------------------
    public virtual void SetupMovement()
    {
        myPlayer = GetComponentInParent<PlayerData>();
    }

    //--------------------------------------
    // FixedUpdate is called once per frame
    // it is more deterministic in its execution time and order than the regular "Update"
    // which is why it is often used for movement and Physics related things
    // it is a built in Unity Function, and thus we should not override it
    //--------------------------------------
    void FixedUpdate()
    {
        Movement();
    }

    //--------------------------------------
    // This is basically how we want things to move
    // iIt gets called every frame in FixedUpdate()

    // Here we describe in which order we process inputs and the move Objects
    // I presume that this basic structure will be the same for most MoveScripts

    // we have made this its own function, because it is clearer to see how
    // we are overwriting function in this base class

    //For most Implementations of specific movement Scripts, 
    //things will always work the same way, so this function shouldn't get overridden a whole lot
    //--------------------------------------
    virtual public void Movement()
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
        //The movement Information is thus stored in the PlayerData Object
        else
        {
            currentMovement = oldLocation - myPlayer.transform.position;
            oldLocation = myPlayer.transform.position;
        }

        //finally, make sure all relevant information is put into the PlayerData Object 
        //that all other scripts have access to (like the Animation Script).
        //Note how this function gets called no matter if you are locally controlled or not!
        UpdatePlayerData();
    }


    //--------------------------------------
    //We find out which input type the player has used
    //This function gets called only on the local player
    //For most Implementations of specific movement Scripts, 
    //things will always work the same way, so this function shouldn't get overridden a whole lot
    //Only Local Player Processes Movement Inputs via Keyboard, Mouse, or On-Screen UI!
    //--------------------------------------
    public virtual void ProcessMovement()
    {
        // Movement per input direction - Keyboard or Gamepad
        if (CheckForDeviceInput())
        {
            ProcessDeviceInput();
        }
        //Movement via On-Screen-Interface (Web and Cellphone)
        else
        {
            ProcessUIInput();
        }

        //after figuring out the inputs, let's move the character
        //Attention! This also only gets called for the local player!
        MoveCharacter();
    }

    //--------------------------------------
    //We have our own function that checks if there was an input
    //from a device. This could be Keyboard, Mouse, Accelerometer...
    //Individual MoveScripts should overwrite this function depending on which Inputs they would like to grab
    //--------------------------------------
    public virtual bool CheckForDeviceInput()
    {
        //in here, depending on what input device we use, we check if there was input from it
        //if so we return true, so the function ProcessInput() can get called, in which we actually calculate things
        //in case there was no Input, we

        return false;
    }


    //--------------------------------------
    //Here we process Input for Keyboard and maybe Gamepad
    //THis function is empty in the base Class, 
    //you will have to write your own implementation
    //--------------------------------------
    public virtual void ProcessDeviceInput()
    {
        //the way these functions are imleented in the examples 
        //in MoveScript2D, MoveScript3DBillboards and MoveScript3DAnimatedMesh
        //is that you write the outcome of your Input Processing into the variable currentMovement
        //which in turn will be used in MoveCharacter() and UpdatePlayerData()
    }


    //--------------------------------------
    //Here we process movement if player moves via on-screen-interface
    //--------------------------------------
    public virtual void ProcessUIInput()
    {
        //we know that PlayerUIBridge.uiMov has the values we need for UI-Interface Movement
        //so we usually access them here in this function, to set up currentMovement for the local player accordingly
    }

    //--------------------------------------
    //After we have figured out what the input wants us to do, 
    //we can now start actually moving our player Object
    //--------------------------------------
    public virtual void MoveCharacter()
    {
        //after the above functions have processed the movement Inputs for the local Player,
        //and most likely written their outcomes in the Variable currentmovement
        //we usually use that variable here to actually move that GameObject that holds our PlayerData Script!
    }

    //--------------------------------------
    //No matter if you are the Locally controlled Player Object, or any other Player Object
    //it is important to update all movement related Data in the PlayerData Script,
    //so all other scripts can access it and work with it
    //--------------------------------------
    public virtual void UpdatePlayerData()
    {
        //in order to calculate if and how much we have moved, 
        //we need to store our actual movement in space in the movementDirection variable in PlayerData
        myPlayer.movementDirection = currentMovement.normalized;
    }


}
