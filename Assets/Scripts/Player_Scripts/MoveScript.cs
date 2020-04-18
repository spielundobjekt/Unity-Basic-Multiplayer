using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class MoveScript : Mirror.NetworkBehaviour
{
    
    public Vector2 speed = new Vector2(1, 1);
    public float tileSpeed = 1.0f;
    public Vector3 mov;
    public PlayerData myPlayer;
    public Vector3 oldLocation;


    void Awake()
    {
    }


    // Use this for initialization
    void Start()
    {
        myPlayer = GetComponent<PlayerData>();
        myPlayer.myCamera = Camera.main;
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        mov = mov * 0.0f;

        if (isLocalPlayer)
        {
            ProcessMovement();
            
        }
        else
        {
            mov = oldLocation - transform.position;
            oldLocation = transform.position;
        }

        UpdatePlayerData();
    }

    //player moves via on-screen-interface
    public void UIMovement()
    {
        mov = speed * PlayerUIBridge.uiMov;        
    }

    //player moves via keyboard or gamepad
    void ProcessMovement()
    {
        // Movement per input direction - Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            mov = new Vector3(
                speed.x * Input.GetAxis("Horizontal"),
                speed.y * Input.GetAxis("Vertical"),
                0.0f);
        }
        //Movement via On-Screen-Interface (Web and Cellphone
        else
        {
            UIMovement();
        }

        MoveCharacter();             
    }

    void MoveCharacter()
    {

        transform.position = transform.position + Time.deltaTime * new Vector3(mov.x, mov.y, 0);
    }

    void UpdatePlayerData()
    {
        myPlayer.movementDirection = mov;
    }
}
     
