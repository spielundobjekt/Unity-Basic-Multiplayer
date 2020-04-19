using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class takes input from UI Buttons
//All functions in this class are called from the Buttons on the Canvas Object 
//set up in the Unity Editor


public class PlayerUIBridge : MonoBehaviour
{
    public static Vector3 uiMov;    //this is a variable that can be accessed by other scripts 
                                    //to do something with its contents.
    
    public void PressMoveArrowLeftRight(float x)
    {
        uiMov.x = x;
        Debug.Log("pressing!");
    }

    public void PressMoveArrowUpDown(float z)
    {
        uiMov.z = z;
    }

    public void BtnRelease()
    {
        uiMov = 0 * uiMov;
    }


}
