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

public class MoveScript3DBillboards : MoveScriptBase
{

    public float lookSpeed = 3.0f;      //a factor for modifying the Rotational speed we get from Input devices
                                    
    public Vector2 lookInput;           //here we store the info we get from the mouse, to look around



    //--------------------------------------
    //Here we check for Input from Keyboard (and maybe Gamepad)
    //and mouse!
    //--------------------------------------
    public override bool CheckForDeviceInput()
    {
       if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //--------------------------------------
    //Here we process Input for Mouse and Keyboard 
    //--------------------------------------
    public override void ProcessDeviceInput()
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
        currentMovement = new Vector3(moveSpeed * Input.GetAxis("Horizontal"), 0.0f, moveSpeed * Input.GetAxis("Vertical"));
    }


    //--------------------------------------
    //Here we Process Input from the On-Screen UI
    //--------------------------------------
    public override void ProcessUIInput()
    {
        //for the UI Movement, we will be doing some old-school like roation and movement

        //we know that PlayerUIBridge.uiMov has the values we need for UI-Interface Movement
        //now we need to interpret them like this:
        //
        //1. we want to rotate left and right when the left and right arrow is pressed
        //2. we want to move "forward" when the up button is pressed and "backwards" when the down button is pressed


        //first, let's do rotation left and right, using similar code to what we have with mouseLook
        lookInput.y += lookSpeed * 0.1f * PlayerUIMovement.uiMov.x;

        Vector3 xyzAngleDegrees = new Vector3(0, 0, 0);
        //(we don't want to rotate around the z-Axis!)
        xyzAngleDegrees = new Vector3(lookInput.x, lookInput.y, 0.0f) * lookSpeed;

        //finally, store our new look value in the PlayerData Script, so other scripts can access it as well!
        //we can store it as a Quaternion, which is Black Magic (tm), but basically describes a rotation in 3dimensional space in 4 values.
        //thankfully, unity gives us a possibility to transform from 3 values - degrees of rotation around different axis - to this magical data type
        myPlayer.lookRotation = Quaternion.Euler(xyzAngleDegrees);

        // then, let's do forward and backward, also similar to what we do in the keyboard movement:
        currentMovement = new Vector3(0.0f, 0.0f, moveSpeed * PlayerUIMovement.uiMov.z);

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
    public override void MoveCharacter()
    {

        //instead of moving our player in absolute values, we need to move it relative to where she is looking
        //Unity (thankfully) gives us a Vector pointing in the direction we would describe as "forward"
        //we can add a fraction of that vector to our current position to move forward
        //Similarly, Unity gives us a vector that points 90 degrees sideways (to the "right"). 
        //we can add a fraction of this vector to our current position to move from side to side
        Vector3 combinedMovement = transform.forward * Time.deltaTime * currentMovement.z * moveSpeed;

        //note how we ADD the other Axis movement to our combinedMovement Vector
        combinedMovement += transform.right * Time.deltaTime * currentMovement.x * moveSpeed;
        
        //after all of these calculations are finished, let's put our updated value back into the currentMovement Variable
        currentMovement = combinedMovement;

        //really really make sure that we never loose the ground beneath our feet!
        currentMovement.y = 0;

        //and now, after we did all of the movement, let's set the y-Value again to what it was before
        myPlayer.transform.position += currentMovement;

    }

}
     
