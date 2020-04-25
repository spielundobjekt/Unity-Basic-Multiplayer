using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a Base class that you should inherit from
//to create your own actions.
// To do this, you just write an Action class, like ActionPlayAudio, and instead of Monobehaviour, you write Action

public class Action : Mirror.NetworkBehaviour
{

    //this function gets called automatically, the moment the client has control over the object
    //We get the authority from our PlayerData Script
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Debug.Log("successfully gained Authority over this object!");
        
        CmdReplicateOnOtherClients(GetComponent<Mirror.NetworkIdentity>());
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
    void CmdReplicateOnOtherClients(Mirror.NetworkIdentity objectNetworkID)
    {
        //call PerformAction on all the Computers!
        RpcPerformAction();

        //give Authority back to the Server, because we have done what we needed
        objectNetworkID.RemoveClientAuthority();
    }

    [Mirror.ClientRpc]
    public void RpcPerformAction()
    {
        PerformAction();
    }



}
