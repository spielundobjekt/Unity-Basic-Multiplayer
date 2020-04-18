﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class MoveScript2D : MonoBehaviour
{
    
    public float speed = 1.0f;
    public float tileSpeed = 1.0f;
    public Vector3 currentMovement;
    public PlayerData myPlayer;
    public Vector3 oldLocation;


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
        currentMovement = speed * PlayerUIBridge.uiMov;        
    }

    //player moves via keyboard or gamepad
    void ProcessMovement()
    {
        // Movement per input direction - Keyboard or Gamepad
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            currentMovement = new Vector3(
                speed * Input.GetAxis("Horizontal"),
                0.0f,
                speed * Input.GetAxis("Vertical"));
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

        transform.position = transform.position + Time.deltaTime * currentMovement;
    }

    void UpdatePlayerData()
    {
        myPlayer.movementDirection = currentMovement.normalized;
    }
}
     
