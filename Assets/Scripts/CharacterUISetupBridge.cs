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
    public static string localCharacterName;    //this is a variable that can be accessed by other scripts 
                                                //to do something with its contents.
                                                //That's why it is static

    public void SetCharacterName(string newName)
    {
        localCharacterName = newName;
    }

}
