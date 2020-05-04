using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a simple script that updates this GameObject's position 
//and/or rotation to the local Player's position 

public class AlwaysFollowPlayer : MonoBehaviour
{
    public bool bUpdatePosition = true;
    public bool bUpdateRotation = false;
    // Update is called once per frame
    void Update()
    {
        if (!PlayerData.localPlayer)
        {
            return;
        }

        if (bUpdatePosition)
        {
            transform.position = PlayerData.localPlayer.transform.position;
        }
        if (bUpdateRotation)
        {
            transform.rotation = PlayerData.localPlayer.transform.rotation;
        }
    }
}
