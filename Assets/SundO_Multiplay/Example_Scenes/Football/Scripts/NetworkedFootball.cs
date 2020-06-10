using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkedFootball : Mirror.NetworkBehaviour
{
    public Rigidbody myRigidbody;

    // Start is called before the first frame update

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();

    }

    //Get the Ball

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Colliding...");
        PlayerData potentialPlayer = collision.gameObject.GetComponent<PlayerData>();
        if (potentialPlayer && potentialPlayer.isLocalPlayer)
        {
            potentialPlayer.CmdGetAuthority(GetComponent<Mirror.NetworkIdentity>());
        }
    }

    


    public override void OnStartAuthority()
    {
        Debug.Log("Player has the Ball!");
        
    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            CmdSetRigidBody(myRigidbody.velocity, myRigidbody.rotation, myRigidbody.position, myRigidbody.angularVelocity);
        }
    }


    [Mirror.Command]
    void CmdSetRigidBody(Vector3 rbVel, Quaternion rbRot, Vector3 rbPos, Vector3 rbAngVel)
    {
        //set the content of this string on the Erver to what we got from the client who triggered this action
        myRigidbody.velocity = rbVel;
        myRigidbody.rotation = rbRot;
        myRigidbody.position = rbPos;
        myRigidbody.angularVelocity = rbAngVel;

        //call PerformAction on all the Computers, and set the text we got to everyone else!

        RpcRBForOthers(myRigidbody.velocity, myRigidbody.rotation, myRigidbody.position, myRigidbody.angularVelocity);


    }

    [Mirror.ClientRpc]
    public void RpcRBForOthers(Vector3 rbVel, Quaternion rbRot, Vector3 rbPos, Vector3 rbAngVel)
    {
        if (!hasAuthority)
        {
            myRigidbody.velocity = rbVel;
            myRigidbody.rotation = rbRot;
            myRigidbody.position = rbPos;
            myRigidbody.angularVelocity = rbAngVel;
        }


    }

    private void OnDestroy()
    {
        Debug.Log("On Destroy is called!");
    }


    public override void OnStopServer()
    {
        //base.OnStopServer();
        Debug.Log("This is called on the Server, when this football is being Despawned...");
        GameObject myNewFootball = Instantiate(this.gameObject);
        Rigidbody newRigidbody = myNewFootball.GetComponent<Rigidbody>();
        newRigidbody.velocity = myRigidbody.velocity;
        newRigidbody.rotation = myRigidbody.rotation;
        newRigidbody.position = myRigidbody.position;
        newRigidbody.angularVelocity = myRigidbody.angularVelocity;
        Mirror.NetworkServer.Spawn(myNewFootball);
        /*
        if (hasAuthority)
        {
            //Spawn a new football on the server, as this one will be removed with the client
            CmdSpawnNewFootball();
            Debug.Log("on the one that has Authority over our football!");    
        }
       */
    }

    [Mirror.Command]
    public void CmdSpawnNewFootball()
    {
        GetComponent<Mirror.NetworkIdentity>().RemoveClientAuthority();
        Debug.Log("This is called on the Server, after the Client has stopped!");
        Mirror.NetworkServer.Spawn(this.gameObject);
        /*
        myRigidbody.velocity = rbVel;
        myRigidbody.rotation = rbRot;
        myRigidbody.position = rbPos;
        myRigidbody.angularVelocity = rbAngVel;
        */
    }

}
