using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes input from UI Buttons
/// All functions in this class are called from the Buttons on the Canvas Object 
/// set up in the Unity Editor
/// </summary>

public class CharacterUISetupBridge : MonoBehaviour
{
    public static string localCharacterName = "";    //this is a variable that can be accessed by other scripts 
                                                     //to do something with its contents.
                                                     //That's why it is static

    public bool bDirectConnectToServer = false;     //these settings allow us to set up if we want to use the NetworkManagerGUI 
                                                    //or directly connect to a server that we have set up
    public bool bStartEditorAsHost = true;          //for testing, we might want to start the editor as "Host and Client"

    public string locCharName = "";                 //this is just for us, to get a look at our Character name.
                                                    //we really need to change this to a singleton pattern...

    public GameObject activePlayerGUI;              //a reference to all the UI things the Player should have
                                                    //because we want that to become visible after we connect

    public void SetCharacterName(string newName)
    {
        localCharacterName = newName;
        locCharName = newName;

        if (bDirectConnectToServer)
        {
            //only start as host and client, if we run in the Unity Editor
            if (bStartEditorAsHost && Application.isEditor)
            {
                GameData.instance.StartHostAndClient();
            }
            else
            {
                GameData.instance.StartClient();
            }
        }

        //set our PlayerGUI to visible
        activePlayerGUI.SetActive(true);

        //set our gameobject to inactive
        this.gameObject.SetActive(false);

    }

}
