using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a Base class that you should inherit from
//to create your own actions.
// To do this, you just write an Action class, like ActionPlayAudio, and instead of Monobehaviour, you write Action

public class Action : Mirror.NetworkBehaviour
{
    //this is a generic variable that can be set if you want to move strings to all clients
    //we use it for ActionSay, but it can be useful for a whole bunch of other actions
    public string textToReplicate;

    //this function gets called automatically, the moment the client has control over the object
    //We get the authority from our PlayerData Script
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Debug.Log("successfully gained Authority over this object!");
        
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
        objectNetworkID.RemoveClientAuthority();
    }

    [Mirror.ClientRpc]
    public void RpcPerformAction(string textFromClient)
    {
        textToReplicate = textFromClient;
        PerformAction();
    }



}
