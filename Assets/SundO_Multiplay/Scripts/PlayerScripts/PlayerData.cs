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
    public static PlayerData localPlayer;   //here we store our local client player, because we will need it to access it
                                            //every once in a while, if we want to do things to objects

    //these next two variables shouldn't show up in the inspector, 
    //but they still need to be public, so other scripts can access them
    //which is why we use the [HideInInspector] Editor Attribute
    [HideInInspector]
    public Vector3 movementDirection;       //here, we will store the Vector this player is moving in

    [HideInInspector]
    public Quaternion lookRotation;         //here we store the Rotation of the Player's gaze

    [Tooltip("Our Character's name. This should be Set through UI")]
    [Mirror.SyncVar]                        //we use this to synchronize variables across server and clients
    public string characterName;            //this is the name of our character Sprite 

    public enum ClientState {CS_INIT, CS_HASDATA};

    [Tooltip("The current State this client is in - is public for observation purposes.")]
    [Mirror.SyncVar]
    public ClientState myState = ClientState.CS_INIT;

    [Tooltip("The current Action Component (from ActionBase.cs) that is being performed.")]
    public ActionBase currentAction;        //if we want to perform an Action in the world
                                            //(as in: interacting with objects, that others can see)
                                            //this will be the action we are currently performing

    [Tooltip("The string that gets synchronized across the network if someone called the Say() function (see UI-Chatbox).")]
    [Mirror.SyncVar]
    public string talk;                     //the string we use to show what we are talking about


    [Tooltip("A Reference to a Text Element (UI or 3DText) to display Speech in.")]
    public TMPro.TMP_Text speechbubble; //a connection to the speechbubble - 
                                        //because we reference an Object of the TextMeshPro Base Class - TMP_Text - we don't care if it is an UGUI or 3D Mesh Text
    


    //public bool bReadyToLoadSprites = false;

    //We do some things in Awake() because this gets called before the Start() Script of anything else is called.
    //But here's the thing: our networking doesn't work in the Awake Function yet.
    private void Awake()
    {
        Debug.Log("I am alive!");

        //Let's let the GameData Script know that we exist!
        GameData.instance.players.Add(this);
    }

    //All Networking stuff needs to be called in Start (and all functions onwards)
    private void Start()
    {
        //here we have to distribute our name to all other clients
        //first, only do this for the local Player, because they have access to the name the Player was given by the User
        if (isLocalPlayer)
        {
            //set this Object as our Reference for all Scripts accessing PlayerData.localPlayer
            PlayerData.localPlayer = this;
            //get the name from the input box -
            characterName = CharacterUISetupBridge.localCharacterName;
            
            Debug.Log("CharacterName: " + characterName);

            //set the character name on the server as well...
            CmdSendNameToServer(characterName);
            myState = ClientState.CS_HASDATA;            
        }
        

    }
    
    
    [Mirror.Command]
    void CmdSendNameToServer(string nameFromClient)
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

    public void PerformAction(ActionBase myAction)
    {
        //First, get Authority over the GameObject, so we can do things with it
        //For that, we need to get its NetworkIdentity Component
        Mirror.NetworkIdentity actionIdentity = myAction.GetComponent<Mirror.NetworkIdentity>();

        //if we want the action to work for all clients
        if (myAction.bReplicateOnClients)
        {
            //then, we tell the server that we would like to get authority over it
            CmdGetAuthority(actionIdentity);
            //this will in turn call the StartOnAuthority() on this Action automatically!
        }
        else
        {
            //if we don't want o replicate this action on other clients, just call 
            myAction.PerformAction();
        }

    }

    //we need to have this function here, because it handles things about our local Player
    public void Say()
    {
        CmdSendTalkToServer(talk);
    }


    [Mirror.Command]
    public void CmdSendTalkToServer(string clientTalk)
    {
        talk = clientTalk;
        RpcShowTalk(talk);
    }

    [Mirror.ClientRpc]
    public void RpcShowTalk(string myTalk)
    {
        if (speechbubble)
        {
            speechbubble.text = myTalk;
        }
    }


    //this gets called on the Client, 
    //it then calls the Command on the Server
    public void ReleaseAuthority(ActionBase myAction)
    {
        //First, get the NetworkID for the object we want to release Authority over
        Mirror.NetworkIdentity actionIdentity = myAction.GetComponent<Mirror.NetworkIdentity>();

        //then, we tell the server that we would like to get authority over it
        CmdReleaseAuthority(actionIdentity);

    }



    [Mirror.Command]
    public void CmdGetAuthority(Mirror.NetworkIdentity objectIdentity)
    {
        //Let the console know, that we received a command on the server
        Debug.Log("Okay, Server received request to give authority to client to do things with this Object:" + objectIdentity.gameObject.name);

        //remove other people's control over this object
        objectIdentity.RemoveClientAuthority();
        
        //give this client control over this object
        objectIdentity.AssignClientAuthority(base.connectionToClient);
    }

    [Mirror.Command]
    public void CmdReleaseAuthority(Mirror.NetworkIdentity objectIdentity)
    {
        //Let the console know, that we received a command on the server
        Debug.Log("Okay, Server received request to remove authority from client:" + objectIdentity.gameObject.name);
        //give the client control over this object
        objectIdentity.RemoveClientAuthority();
    }

}
