using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Script that is part of the UI-Actionbridge Prefab
/// All your Buttons should call this function here, with the Action you want to execute as a Parameter! 
/// </summary>
public class ActionUIBridge : MonoBehaviour
{
    [
        Header("If you want to trigger an Action over UI, ", order = 0), Space(-10, order = 1),
        Header("set your Button to call 'SetPerformedAction' on this Script", order = 2), Space(-10, order = 3),
        Header("and pass your Action Component as Reference", order = 4), Space(15, order = 5),
        Header("You will only need this script once in your Scene!", order = 6), Space(15, order = 7),
    ]


    [Tooltip("you can check this message in your Log to see if everything worked out")]
    public string debugMessage = "An Action wants to be executed!";
    //--------------------------------------
    // this is the function that all Buttons 
    // and other UI Elements can call
    //--------------------------------------
    public void SetPerformedAction(ActionBase thisAction)
    {
        Debug.Log(debugMessage + thisAction.gameObject.name);
        PlayerData.localPlayer.PerformAction(thisAction);
    }


}
