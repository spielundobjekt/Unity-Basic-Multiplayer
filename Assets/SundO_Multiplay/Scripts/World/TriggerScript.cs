using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script executes different Actions depending on Collision states of the Player with this Object
/// The main difference between the functions of this script is, if a Collider is marked with IsTrigger, 
/// the Collision is ignored (the other object can go right through)
/// </summary>

public class TriggerScript : MonoBehaviour
{
    //drop your Actions in any of these variables

    [
    Header("These Variables can be filled with Action-Scripts that get executed ", order = 0), Space(-10, order = 1),
    Header("at specific times depending on the local Player's relation to this Collider.", order = 2), Space(15, order = 3),
    ]

    [Tooltip("Drop your Action in here that you want to execute on Trigger Enter (if your Collider has IsTrigger selected!)!")]
    public ActionBase actionOnTriggerEnter;

    [Tooltip("Drop your Action in here that you want to execute on Trigger Exit (if your Collider has IsTrigger selected!)!")]
    public ActionBase actionOnTriggerExit;

    [Tooltip("Drop your Action in here that you want to execute when a Player touches this object (if your Collider has IsTrigger NOT selected!)!")]
    public ActionBase actionOnCollisionEnter;

    [Tooltip("Drop your Action in here that you want to execute when a Player stops touching this object (if your Collider has IsTrigger NOTselected!)!")]
    public ActionBase actionOnCollisionExit;


    //--------------------------------------
    // These next two functions will only get called if the Collider 
    // of the Gameobject that has this script as its component has the
    // IsTrigger property set to true!
    //--------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        //tell us, firend, who entered?
        Debug.Log("Entered Trigger: "+other.gameObject.name);

        //let's execute the action on the computer of the person that entered the Collider!
        if (other.gameObject.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer)
        {
            //let's make sure there is an Action set up!
            if (actionOnTriggerEnter)
            {
                PlayerData.localPlayer.PerformAction(actionOnTriggerEnter);
            }
            else
            {
                Debug.LogWarning("No Action dropped in actionOnTriggerEnter!");
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //tell us, firend, who left?
        Debug.Log("Exited Trigger: "+other.gameObject.name);

        //let's execute the action on the computer of the person that exited the Collider!
        if (other.gameObject.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer)
        {
            if (actionOnTriggerExit)
            {
                PlayerData.localPlayer.PerformAction(actionOnTriggerExit);
            }
            else
            {
                Debug.LogWarning("No Action dropped in actionOnTriggerExit!");
            }
        }
    }


    //--------------------------------------
    // These next two functions will only get called if the Collider 
    // of the Gameobject that has this script as its component has the
    // IsTrigger property set to false!
    //--------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        //tell us, firend, who entered?
        Debug.Log("Entered Collision: "+collision.gameObject.name);

        //let's execute the action on the computer of the person that started touching the Collider!
        if (collision.gameObject.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer)
        {
            if (actionOnCollisionEnter)
            {
                PlayerData.localPlayer.PerformAction(actionOnCollisionEnter);
            }
            else
            {
                Debug.LogWarning("No Action dropped in actionOnCollisionEnter!");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //tell us, firend, who exited?
        Debug.Log("Exited Collision: "+collision.gameObject.name);

        //let's execute the action on the computer of the person that stopped touching the Collider!
        if (collision.gameObject.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer)
        {
            if (actionOnCollisionExit)
            {
                PlayerData.localPlayer.PerformAction(actionOnCollisionExit);
            }
            else
            {
                Debug.LogWarning("No Action dropped in actionOnCollisionExit!");
            }
        }
    }


}
