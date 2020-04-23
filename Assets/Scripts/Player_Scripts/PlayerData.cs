using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class holds information that should be accessible by a number of scripts
//some of those will be attached to this gameobject
//some of them will be children to this gameobject

    //It is often good practice to split the data that multiple scripts access
    //into its own separate script

public class PlayerData : Mirror.NetworkBehaviour
{
    public Vector3 movementDirection;       //here, we will store the Vector this player is moving in
    public Quaternion lookRotation;         //here we store the Rotation of the Player's gaze

    [Mirror.SyncVar]                        //we use this to synchronize variables across server and clients
    public string characterName;            //this is the name of our character Sprite 

    Surroundings mySurroundings;            //helper Object to check for things around us, 
                                            //will be used later

    public enum ClientState {CS_INIT, CS_HASDATA};

    [Mirror.SyncVar]
    public ClientState myState = ClientState.CS_INIT;

    public bool bReadyToLoadSprites = false;

    //We do some things in Awake() because this gets called before the Start() Script of anything else is called.
    //But here's the thing: our networking doesn't work in the Awake Function yet.
    private void Awake()
    {
        Debug.Log("I am alive!");

        //connect to the script that does the check for the surroundings
        mySurroundings = GetComponent<Surroundings>();

        //Let's let the GameData Script know that we exist!
        GameData.instance.players.Add(this);

        //initialize our characterName, so there is something to load even if nothing has been put into the name field
        characterName = "friedrich";

        //GetComponent<Mirror.NetworkIdentity>().au
    }

    //All Networking stuff needs to be called in Start (and all functions onwards)
    private void Start()
    {
        //here we have to distribute our name to all other clients
        //first, only do this for the local Player, because they have access to the name the Player was given by the User
        if (isLocalPlayer)
        {
            //get the name from the input box -
            characterName = CharacterUISetupBridge.localCharacterName;
            
            Debug.Log("CharacterName: " + characterName);

            //set the character name on the server as well...
            CmdSendDataToServer(characterName);
            myState = ClientState.CS_HASDATA;            
        }
        
    }
        
    [Mirror.Command]
    void CmdSendDataToServer(string nameFromClient)
    {
        characterName = nameFromClient;
        myState = ClientState.CS_HASDATA;
    }



    //This gets called when the Network shuts down
    //Or the Player is otherwise removed
    private void OnDestroy()
    {
        //since we keep track of our players in the GameData.instance.players List, we need to keep it tidy
        //so if one Player leaves, we need to remove her reference from that list
        GameData.instance.players.Remove(this);
    }



}
