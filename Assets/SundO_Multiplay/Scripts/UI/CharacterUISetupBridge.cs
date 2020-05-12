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
                                                     //This shows up in the Unity Editor, and helps to understand the script a bit better
    [
        Header("Call 'SetCharacterName()' on this Script from an InputField to spawn a Player. ", order = 0), Space(-10, order = 1),
        Header("This Script also contains Options for Connecting without the Mirror GUI.", order = 2), Space(15, order = 3),
        Header("If you use this script, you only need it once in your Scene.", order = 4), Space(15, order = 5),
    ]

    [Tooltip("Set this to true, if you want Players to Connect to the Server directly after entering their name. Network Address needs to be set up in NetworkManager!")]
    public bool bDirectConnectToServer = false;     //these settings allow us to set up if we want to use the NetworkManagerGUI 
                                                    //or directly connect to a server that we have set up
    [Tooltip("Set this to true, if you want to test your Scene in the Editor - starts as Host(Server+Client) when you hit the Play Button")]
    public bool bStartEditorAsHost = true;          //for testing, we might want to start the editor as "Host and Client"


    [HideInInspector]
    public string locCharName = "";                 //this is just for us, to get a look at our Character name.
                                                    //we really need to change this to a singleton pattern...

    [Tooltip("This GameObject will be Set Active once we are connected to a Server - useful to manage UI Overkill")]
    public GameObject activePlayerGUI;              //a reference to all the UI things the Player should have
                                                    //because we want that to become visible after we connect


    //This function can get called by finishing TextInput in an Inputfield right now. 
    
    public void SetCharacterName(string newName)
    {
        localCharacterName = newName;
        locCharName = newName;

        //if we want to directly connect to the server, call that Function
        if (bDirectConnectToServer)
        {
            EstablishConnection();
        }

        //if someone has dragged a GameObject into activePlayerGUI, let's hide it!
        if (activePlayerGUI)
        {
            //set our PlayerGUI to visible
            activePlayerGUI.SetActive(true);
        }

        //set our gameobject to inactive
        this.gameObject.SetActive(false);

    }


    //This function can get called by e.g. pressing a button, or after entering a name for your character (see above). 

    public void EstablishConnection()
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

}
