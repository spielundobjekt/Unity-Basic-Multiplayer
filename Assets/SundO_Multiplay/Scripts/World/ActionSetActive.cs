using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an example implementation of an Action
/// If you want to create your own networked Actions, they should look a little bit like this
/// In this example, we are setting a GameObject Active or Inactive
/// </summary>
 
public class ActionSetActive : ActionBase       //make sure you derive from the Action Base Class
{
    
    [Tooltip("The Gameobject we want to set Active or Inactive")]
    public GameObject thingToSetActive;         //the game Object we want to set Active or Inactive

    //this function will be called on all Clients simultaneously, 
    //everything that you write here, you can be sure will happen for all Players
    //if bReplicateOnClients is set to true.
    //if not, then this code will only get called on this individual client!
    //(see ActionBase.cs) 
    public override void PerformAction()
    {
        Debug.Log("Performing Action to Set a GameObject Active!!!");
        
        if (thingToSetActive.activeSelf)
        {
            thingToSetActive.SetActive(false);
        }
        else
        {
            thingToSetActive.SetActive(true);
        }
        
    }

}
