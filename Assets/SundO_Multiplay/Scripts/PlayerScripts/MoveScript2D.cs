using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


// This Move Script Handles Movement in 2D for Players
// It is derived from MoveScriptBase - please check that one out,
// because it handles the underlying decision structure on when these functions get called

public class MoveScript2D : MoveScriptBase
{

    //--------------------------------------
    //We find out if the player used a device for Input
    //--------------------------------------
    public override bool CheckForDeviceInput()
    {
        // Movement per input direction - Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 /* we add keys that we want to configure here: */ || Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
        //Movement via On-Screen-Interface (Web and Cellphone)
        else
        {
            return false;
        }
    }

    //--------------------------------------
    //Here we process Input for Keyboard and maybe Gamepad
    //--------------------------------------
    public override void ProcessDeviceInput()
    {
        //this is code for jumping using unity's physics system
        //and presuming you have placed rigidbody and collider on the main Player Object
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponentInParent<Rigidbody>().AddForce(new Vector3(0, 4, 0), ForceMode.Impulse);
        }
        */
        currentMovement = new Vector3( moveSpeed * Input.GetAxis("Horizontal"), 0.0f, moveSpeed * Input.GetAxis("Vertical"));
    }


    //--------------------------------------
    //Here we process movement if player moves via on-screen-interface
    //--------------------------------------
    public override void ProcessUIInput()
    {
        //we know that PlayerUIBridge.uiMov has the values we need for UI-Interface Movement
        currentMovement = moveSpeed * PlayerUIMovement.uiMov;
    }

    //--------------------------------------
    //After we have figured out what the input wants us to do, 
    //we can now start actually moving our player Object
    //--------------------------------------
    public override void MoveCharacter()
    {
        //make sure that we don't move the transform of our own GameObject, 
        //but the Transform of the GameObject containing the PlayerData Script
        myPlayer.transform.position = myPlayer.transform.position + Time.deltaTime * currentMovement;
    }

    //--------------------------------------
    //No matter if you are the Locally controlled Player Object, or any other Player Object
    //it is important to update all movement related Data in the PlayerData Script,
    //so all other scripts can access it and work with it
    //--------------------------------------
    public override void UpdatePlayerData()
    {
        
        //There seems to be a bug in the code right now, 
        //that flips the animation of other characters (those that we do not control locally)
        //in order to fix it for now, we will flip the movement vector for those players

        if (myPlayer.isLocalPlayer)
        {
            myPlayer.movementDirection = currentMovement.normalized;
        }
        else
        {
            myPlayer.movementDirection = -currentMovement.normalized;
        }
    }
}
     
