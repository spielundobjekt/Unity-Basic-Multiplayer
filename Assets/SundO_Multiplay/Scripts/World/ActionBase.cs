using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a Base class that you should inherit from
/// to create your own actions.
/// To do this, you just write an Action class, like ActionPlayAudio, and instead of Monobehaviour, you write Action
/// </summary>
public class ActionBase : Mirror.NetworkBehaviour
{
    //this shows up in the Unity Editor and can be used to describe Editor instructions
    [
        Header("If you want to trigger this Action over UI, ", order =0),Space(-10, order =1),
        Header("set your Button to call 'SetPerformedAction' in the UI-ActionInterface Gameobject", order = 2), Space(-10, order =3),
        Header("and pass this Component as a Reference", order = 4), Space(15, order = 5),
    ]

    //we use this to determine if we want our Action to execute on all Client devices
    //or just on our local machine.
    //setting this to false can be useful for triggering videos or audio individually
    [Tooltip("Turn this on, if you want your Action to be performed on all connected Clients")]
    public bool bReplicateOnClients = true;
    //for example:  - bReplicateOnClients = true - a video will start playing on all networked computers
    //              - bReplicateOnClients = false - a video will start playing only on the computer of the local player


    //this is a generic variable that can be set if you want to move strings to all clients
    //there is no use case for this in our examples as of yet
    [Tooltip("This can be left blank unless you know what you are doing!")]
    public string textToReplicate;

    //this function gets called automatically, the moment the client has control over the object
    //We get the authority from our PlayerData Script
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        
        CmdReplicateOnOtherClients(GetComponent<Mirror.NetworkIdentity>(), textToReplicate);
        
    }

    
    
    
    //this is set up, so you can fill in whatever it is that you want to do
    //as long as you inherit from this base class
    //and you do not have to worry about network things
    public virtual void PerformAction()
    {

    }

    //These two functions are neededto tell the server 
    //that all other connected computers should also do the thing!

    [Mirror.Command]
    void CmdReplicateOnOtherClients(Mirror.NetworkIdentity objectNetworkID, string textSentToServer)
    {
        //set the content of this string on the Erver to what we got from the client who triggered this action
        textToReplicate = textSentToServer;

        //call PerformAction on all the Computers, and set the text we got to everyone else!
        
        RpcPerformAction(textToReplicate);


        //give Authority back to the Server, because we have done what we needed
        Debug.Log("Removing Authority over Object after we called PerformAction() on Clients!");
        objectNetworkID.RemoveClientAuthority();
    }

    [Mirror.ClientRpc]
    public void RpcPerformAction(string textFromClient)
    {
        textToReplicate = textFromClient;
        PerformAction();       
    }


}
