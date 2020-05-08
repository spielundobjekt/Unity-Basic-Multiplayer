using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is an example implementation of an Action
//If you want to create your own networked Actions, they should look a little bit like this

//In this example, we are starting or stopping an audio file

public class ActionSetActive : ActionBase       //make sure you derive from the Action Base Class
{   
    //I am testing Notes in the Inspector...
    [TextArea]
    public string howThisWorks =    "The function PerformAction() in this script will be called on all Clients simultaneously. \n" +
                                    "Everything that you write here, you can be sure will happen for all Clients.\n\n " +
                                    "In this example, we are calling this function when a Button is pressed - \nButton is set up in Child GameObject " +
                                    "and connects to ActionUIBridge via OnClick(), with THIS action as a Parameter.";

    public GameObject thingToSetActive;

    //this function will be called on all Clients simultaneously
    //everything that you write here, you can be sure will happen for all Players
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
