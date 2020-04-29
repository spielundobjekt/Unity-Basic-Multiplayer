using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;   //this thing is new! we need it to access all NavMesh related Components!

//In this script, we use Unity's built in Movement and Path-finding System
//to help us control our character.
//because we use a lot of "Unity Stuff", our code will be much shorter, 
//but many things will need to be set up in the Editor to make this work.
//For more information on unity's system that we use here, see: https://docs.unity3d.com/Manual/Navigation.html

public class MoveScript3DAnimatedMesh : MonoBehaviour
{
    //Our player so far has managed to walk around using our Keyboard and mouse inputs.
    //we will still be using those, but now extend our movement possibilities with some 
    //Unity functionality
    //One of these Unity Concepts is that of a NavMesh (see link above).
    //In order for our NavMesh functionality to work, our Player will need some components added to her Gameobject.
    //and we will need some extra objects to manipulate, where she is going. 

    public PlayerData myPlayer;            //a link to our PlayerData Script - we don't need to set this public btw.
    public GameObject navMeshTarget;        //The Player should always navigate to this target 
                                           //we connect to this Gameobject in the Editor!
                                           
    public NavMeshAgent myNavMeshAgent;           //we need a connection to this, because we want to change its destination

    public float agentAngularSpeed = 1440;      //this is the angular turn rate we want to apply to our NavMeshAgent
    public float moveSpeed = 1;                 //this is our Agent's moveSpeed

    public float lookSpeed = 3.0f;              //a factor for modifying the Rotational speed we get from Input devices

    public Vector3 currentMovement;             //this variable contains the amount of units we have moved, no matter if we are a local Player or a networked player

    public Vector3 oldLocation;                 //we use this to figure out if we have moved during the last frame. 
                                                //this is useful if we do not control this Player from our local inputs,

    public Vector2 lookInput;                   //here we store the info we get from the mouse, to look around
    public Vector3 movementInput;               //here, we store the information we get from the keyboard and/or gamepad



    //--------------------------------------
    //We connect things, as we usually do, 
    //but also create a NavMeshAgent Component for our Player
    //--------------------------------------
    private void Start()
    {
        myPlayer = GetComponentInParent<PlayerData>();

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
    // FixedUpdate is called once per frame
    // it is more deterministic in its execution time and order than the regular "Update"
    // which is why it is often used for movement and Physics related things
    // here we describe in which order we process inputs and the move Objects
    // we do most things exatly the same as the other MoveScripts!
    //--------------------------------------
    private void FixedUpdate()
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
            currentMovement = oldLocation - myPlayer.transform.position;
            oldLocation = myPlayer.transform.position;
        }

        //finally, make sure all relevant information is put into the PlayerData Object 
        //that all other scripts have access to (like the Animation Script).
        UpdatePlayerData();
    }

    //--------------------------------------
    //HERE THINGS HAVE CHANGED!!!!
    //We find out which input type the player has used
    //Only Local Player Processes Movement Inputs via Keyboard, Mouse, or On-Screen UI!
    // there is something different here though! 
    //We set our navMeshTarget to 0,0,0, so that the player only moves if there was an input!
    //--------------------------------------
    void ProcessMovement()
    {
        //Vector3.zero is just a lazy way of writing "new Vector3(0,0,0);"
        navMeshTarget.transform.localPosition = Vector3.zero;

        // Movement per input direction - relevant if input is Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
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
    //HERE THINGS HAVE CHANGED!!!!
    //After we have figured out what the input wants us to do, 
    //we can now start actually moving our player Object
    //in this specific case, we want to move our player using Unity's NavMesh Concept!
    //so our movement is a bit different!
    //--------------------------------------
    void MoveCharacter()
    {
        //we calculate our position we want to move to similarly to our regular 3D movement:

        //instead of moving our player in absolute values, we need to move it relative to where she is looking
        //Unity (thankfully) gives us a Vector pointing in the direction we would describe as "forward"
        // we can add a fraction of that vector to our current position to move forward
        currentMovement = transform.forward * Time.deltaTime * movementInput.z;

        //Similarly, Unity gives us a vector that points 90 degrees sideways (to the "right"). 
        //we can add a fraction of this vector to our current position to move from side to side
        currentMovement += transform.right * Time.deltaTime * movementInput.x;

        //make sure that we never loose the ground beneath our feet!
        currentMovement.y = 0;

        //but now, instead of moving our player directly, let's set use our navMeshTarget 
        //to the position she wants to move to!

        //and now, after we did all of the movement, let's set the y-Value again to what it was before
        navMeshTarget.transform.localPosition += currentMovement;

        //also set our destination
        myNavMeshAgent.destination = navMeshTarget.transform.position;

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
