using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class MoveScript3D : MonoBehaviour
{
    
    public float moveSpeed = 1.0f;
    public float lookSpeed = 3.0f;

    public Vector3 currentMovement;     //this variable contains the amount of units we have moved, no matter if we are a local Player or a networked player
    public PlayerData myPlayer;
    public Vector3 oldLocation;
    public Vector2 lookInput;           //here we store the info we get from the mouse, to look around
    public Vector3 movementInput;       //here, we store the information we get from the keyboard and/or gamepad



    void Awake()
    {
    }


    // Use this for initialization
    void Start()
    {
        myPlayer = GetComponentInParent<PlayerData>();
        myPlayer.myCamera = Camera.main;
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        currentMovement = currentMovement * 0.0f;

        if (myPlayer.isLocalPlayer)
        {
            ProcessMovement();
            
        }
        else
        {
            currentMovement = oldLocation - transform.position;
            oldLocation = transform.position;
        }

        UpdatePlayerData();
    }

    //player moves via on-screen-interface
    public void UIMovement()
    {
        currentMovement = moveSpeed * PlayerUIBridge.uiMov;        
    }

    //player moves via keyboard or gamepad
    void ProcessMovement()
    {
        //In 3D, our movement is always relative to where we look! 
        //So we have to calculate our looking direction, and then move accordingly!
        //(This following part of the script is adapted from here: https://www.reddit.com/r/Unity3D/comments/8k7w7v/unity_simple_mouselook/ )
        lookInput.y += Input.GetAxis("Mouse X");
        lookInput.x += -Input.GetAxis("Mouse Y");

        //this next line makes sure that our rotation never goes beyond a certain value (so we cannot fully rotate our head all the way down)
        lookInput.x = Mathf.Clamp(lookInput.x, -15f, 15f);
                
        //and let's calculate all three rotation values for all our three possible rotation axes 
        //let's use a local Variable to store the information we get from the mouse
        Vector3 xyzAngleDegrees = new Vector3(0, 0, 0);
        //(we don't want to rotate around the z-Axis!)
        xyzAngleDegrees = new Vector3(lookInput.x, lookInput.y,0.0f) * 3.0f;

        //finally, store our new look value in the PlayerData Script, so other scripts can access it as well!
        //we can store it as a Quaternion, which is Black Magic (tm), but basically describes a rotation in 3dimensional space in 4 values.
        //thankfully, unity gives us a possibility to transform from 3 values - degrees of rotation around different axis - to this magical data type
        myPlayer.lookRotation = Quaternion.Euler(xyzAngleDegrees);

        // Movement per input direction - Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            //notice how we do not want to move in the y-Axis
            movementInput = new Vector3( moveSpeed * Input.GetAxis("Horizontal"), 0.0f, moveSpeed * Input.GetAxis("Vertical"));
        }
        //Movement via On-Screen-Interface (Web and Cellphone)
        else
        {
            UIMovement();
        }

        //now, before we move our character

        MoveCharacter();             
    }

    void MoveCharacter()
    {
      
        
        //let's rotate our player depending on where we look:
        myPlayer.transform.eulerAngles = new Vector2(0, lookInput.y) * lookSpeed;

        //instead of moving our player in absolute values, we need to move it relative to where she is looking
        //Unity (thankfully) gives us a Vector pointing in the direction we would describe as "forward"
        // we can add a fraction of that vector to our current position to move forward
        currentMovement = transform.forward * Time.deltaTime * movementInput.z;

        //Similarly, Unity gives us a vector that points 90 degrees sideways (to the "right"). 
        //we can add a fraction of this vector to our current position to move from side to side
        currentMovement += transform.right * Time.deltaTime * movementInput.x;

        //make sure that we never loose the ground beneath our feet!
        currentMovement.y = 0;

        //and now, after we did all of the movement, let's set the y-Value again to what it was before
        myPlayer.transform.position += currentMovement;

        
    }

    void UpdatePlayerData()
    {
        //in order to calculate if and how much we have moved, 
        //we need to store our actual movement in space in the movementDirection variable in PlayerData
        myPlayer.movementDirection = currentMovement.normalized;
    }
}
     
