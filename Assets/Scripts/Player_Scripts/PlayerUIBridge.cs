using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIBridge : MonoBehaviour
{
    public static Vector3 uiMov;

    private void FixedUpdate()
    {
        //uiMov = 0.0f * uiMov;
    }

    public void PressMoveArrowLeftRight(float x)
    {
        uiMov.x = x;
        Debug.Log("pressing!");
    }

    public void PressMoveArrowUpDown(float y)
    {
        uiMov.y = y;
    }

    public void BtnRelease()
    {
        uiMov = 0 * uiMov;
    }


}
