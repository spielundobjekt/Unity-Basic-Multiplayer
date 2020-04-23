using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes input from UI Buttons
/// All functions in this class are called from the Buttons on the Canvas Object 
/// set up in the Unity Editor
/// </summary>


public class PlayerUIBridge : MonoBehaviour
{
    public static Vector3 uiMov;    //this is a variable that can be accessed by other scripts 
                                    //to do something with its contents.
                                    //That's why it is static

    //a different way to implement this, instead of doing static variables, is to do a singleton pattern, 
    //as shown in the GameData script

    //--------------------------------------
    //this function will be called from an UI Button set up in the editor
    //--------------------------------------
    public void PressMoveArrowLeftRight(float x)
    {
        uiMov.x = x;
    }

    //--------------------------------------
    //this function will be called from an UI Button set up in the editor
    //--------------------------------------
    public void PressMoveArrowUpDown(float z)
    {
        uiMov.z = z;
    }

    //--------------------------------------
    //this function will be called from an UI Button set up in the editor
    //it is used to reset the uiMov variable back to 0, so we don't keep walking 
    //even though we released the ui Button
    //--------------------------------------
    public void BtnRelease()
    {
        uiMov = 0 * uiMov;
    }


}
