using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;   //this thing is new! we need it to access all NavMesh related Components!

//In this script, we use Unity's built in Movement and Path-finding System
//to help us control our character.
//because we use a lot of "Unity Stuff", our code will be much shorter, 
//but many things will need to be set up in the Editor to make this work.
//For more information on unity's system that we use here, see: https://docs.unity3d.com/Manual/Navigation.html

public class MoveScript3DAnimatedMesh : MoveScriptBase
{
    //Our player so far has managed to walk around using our Keyboard and mouse inputs.
    //we will still be using those, but now extend our movement possibilities with some 
    //Unity functionality
    //One of these Unity Concepts is that of a NavMesh (see link above).
    //In order for our NavMesh functionality to work, our Player will need some components added to her Gameobject.
    //and we will need some extra objects to manipulate, where she is going. 

    public GameObject navMeshTarget;        //The Player should always navigate to this target 
                                           //we connect to this Gameobject in the Editor!
                                           
    public NavMeshAgent myNavMeshAgent;           //we need a connection to this, because we want to change its destination

    public float agentAngularSpeed = 1440;      //this is the angular turn rate we want to apply to our NavMeshAgent
    public float lookSpeed = 3.0f;              //a factor for modifying the Rotational speed we get from Input devices

    public Vector2 lookInput;                   //here we store the info we get from the mouse, to look around
    
    //--------------------------------------
    //We connect things, as we usually do, 
    //but also create a NavMeshAgent Component for our Player
    //--------------------------------------
    public override void SetupMovement()
    {
        //first we do everything that our base class is doing
        base.SetupMovement();

        //instead of linking to an already existing Component, let's create one and link to it afterwards!
        //Remember, we are not creating a component in the PlayerData Script, 
        //but in the Gameobject that holds the Playerdatascript!
        myNavMeshAgent = myPlayer.gameObject.AddComponent<NavMeshAgent>();
        
        //now that we have created a new NavMeshAgent, we should set up some values, 
        //to make it work better in our context
        //Remember: we have created this Component in the same GameObject that PlayerData is in!
        myNavMeshAgent.angularSpeed = agentAngularSpeed;
        myNavMeshAgent.speed = moveSpeed;

    }

    //--------------------------------------
    //HERE THINGS HAVE CHANGED!!!!
    //This is a very good place to reset the location of our navmeshtarget
    //before we calculate its new position for this frame!
    //--------------------------------------
    public override void ProcessMovement()
    {
        //reset the position of our navmeshtarget!
        //Note that we do this for all Players, not just the locally controlled one.
        //Vector3.zero is just a lazy way of writing "new Vector3(0,0,0);"
        navMeshTarget.transform.localPosition = Vector3.zero;

        //then we do all the things that the base class does for movement functionality
        base.ProcessMovement();
    }

    //--------------------------------------
    //We find out which input type the player has used
    //Only Local Player Processes Movement Inputs via Keyboard, Mouse, or On-Screen UI!
    // there is something different here though! 
    //We set our navMeshTarget to 0,0,0, so that the player only moves if there was an input!
    //--------------------------------------
    public override bool CheckForDeviceInput()
    {
        
        // Movement per input direction - relevant if input is Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        return false;
    }

    //--------------------------------------
    //Here we process Input for Mouse, Keyboard and maybe later, Gamepad
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
    //in this specific case, we want to move our player using Unity's NavMesh Concept!
    //so our movement is a bit different!
    //--------------------------------------
    public override void MoveCharacter()
    {

        //In here, we will change the player's y-Rotation to match that one of the lookRotation!
        myPlayer.transform.rotation = Quaternion.Euler(new Vector3(0, lookInput.y * lookSpeed, 0));

        //let's make our currentmovement dependent on the Framerate
        currentMovement = currentMovement * Time.deltaTime;

        //make extra sure that we never loose the ground beneath our feet!
        currentMovement.y = 0;

        //but now, instead of moving our player directly, let's set use our navMeshTarget 
        //to the position she wants to move to!
        navMeshTarget.transform.localPosition = currentMovement;

        //and then, accordingly, also set the destination in our NavMeshAgent
        myNavMeshAgent.destination = navMeshTarget.transform.position;

        //Note: we could do without the Gameobject NavMeshTarget, but it illustrates a bit better how things work
        //and it is useful if we want to debug things       

    }

}
