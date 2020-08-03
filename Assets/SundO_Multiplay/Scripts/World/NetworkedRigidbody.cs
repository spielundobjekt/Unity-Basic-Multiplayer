using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///This class can be added to a GameObject with Rigidbody to make it synchronize over the Network
///Currently, Synchronization works if Players move or interact with this GameObject. 
///I do not know what happens when other Rigidbodies interact with this.
///
///ATTENTION! IN ORDER FOR THIS TO WORK, YOU MUST DO ADDITIONAL STEPS!!!
///
///You have to make a Prefab out of your Rigidbody Gameobject and add it to the 
///"Registered Spawnable Prefabs" List in the NetworkManager
/// </summary>

public class NetworkedRigidbody : Mirror.NetworkBehaviour
{
    //this shows up in the Unity Editor and can be used to describe Editor instructions
    [
        Header("DO NOT USE THIS COMPONENT, ", order = 0), Space(-10, order = 1),
        Header("Please use the one from the Mirror library.", order = 2), Space(-10, order = 3),
        Header("It has a little blue square next to it in the add-component menu.", order = 4), Space(15, order = 5),
    ]


    private Rigidbody myRigidbody;      //a Reference to the Rigidbody Component within this GameObject

    

    private void Start()
    {
        //get the Reference to our Rigidbody Component
        myRigidbody = GetComponent<Rigidbody>();

    }

    //In order to make this Rigidbody synchronize over the network, the Player that touches it will 
    //get the Authority over this Rigidbody
    private void OnCollisionEnter(Collision collision)
    {
        //see if the touching GameObject contains a PlayerData Component
        PlayerData potentialPlayer = collision.gameObject.GetComponent<PlayerData>();
        
        //if it does, give Authority to the Client Machine that this Player belongs to
        if (potentialPlayer && potentialPlayer.isLocalPlayer)
        {
            potentialPlayer.CmdGetAuthority(GetComponent<Mirror.NetworkIdentity>());
        }
    }

    //In fixedUpdate, we send the properties of the rigidbody to the server
    private void FixedUpdate()
    {
        //we do this only on the Machine that actually has the Authority of this object right now
        if (hasAuthority)
        {
            //here we send all of the Rigidbody Properties to the Server
            CmdSetRigidBody(myRigidbody.velocity, myRigidbody.rotation, myRigidbody.position, myRigidbody.angularVelocity);
        }
    }

    //This function will only execute on the Server, with the variables set by the Client
    [Mirror.Command]
    void CmdSetRigidBody(Vector3 rbVel, Quaternion rbRot, Vector3 rbPos, Vector3 rbAngVel)
    {
        //here, we manually update the Rigidbody properties on the Server with the data from the Client that
        myRigidbody.velocity = rbVel;
        myRigidbody.rotation = rbRot;
        myRigidbody.position = rbPos;
        myRigidbody.angularVelocity = rbAngVel;

        //We call this function on all of the clients, with the now updated Data from the Server
        RpcRBForOthers(myRigidbody.velocity, myRigidbody.rotation, myRigidbody.position, myRigidbody.angularVelocity);


    }

    //This function will only execute on Clients and will be called from the Server
    [Mirror.ClientRpc]
    public void RpcRBForOthers(Vector3 rbVel, Quaternion rbRot, Vector3 rbPos, Vector3 rbAngVel)
    {
        //only do this for the clients that do not currently have Authority over this GameObject
        if (!hasAuthority)
        {
            myRigidbody.velocity = rbVel;
            myRigidbody.rotation = rbRot;
            myRigidbody.position = rbPos;
            myRigidbody.angularVelocity = rbAngVel;
        }


    }

    //If a client leaves the Server, this function is called on the Server.
    //The way mirror is set up is that once a Client has Authority over an Object, 
    //the Object gets destroyed if the Client disconnects
    //We do not want that!
    //Since there is no way (at least none i could find) to change this behaviour, 
    //we have to create a new GameObject of the exact same type as this one, 
    //and give it our Rigidbody properties
    public override void OnStopServer()
    {
        Debug.Log("This is called on the Server, when this football is being Despawned...");
        //First we Instantiate the new GameObject on the Server
        GameObject myNewFootball = Instantiate(this.gameObject);
        
        //then we transfer the last known properties of our Rigidbody over to the Rigidbody of the new GameObject
        Rigidbody newRigidbody = myNewFootball.GetComponent<Rigidbody>();
        newRigidbody.velocity = myRigidbody.velocity;
        newRigidbody.rotation = myRigidbody.rotation;
        newRigidbody.position = myRigidbody.position;
        newRigidbody.angularVelocity = myRigidbody.angularVelocity;

        //Then we Instantiate this GameObject on all Clients with all of these same properties
        Mirror.NetworkServer.Spawn(myNewFootball);

        //after this, we die, but we know that a clone of ourselves will live on!
        //Go Clone! May you achieve everything we couldn't!
    }

}
