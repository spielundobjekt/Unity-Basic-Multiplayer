using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUIBridge : MonoBehaviour
{

    //--------------------------------------
    // this is the function that all Buttons 
    // and other UI Elements can call
    //--------------------------------------
    public void SetPerformedAction(Action thisAction)
    {
        Debug.Log("An Action wants to be executed!");
        PlayerData.localPlayer.PerformAction(thisAction);
    }


}
