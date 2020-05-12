using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script calls the Say() function for the local Player, 
//and updates the talk Variable in the local Player
//Its functions get called from an InputField deeper down in its Editor Hierarchy


public class PlayerSay : MonoBehaviour
{
    //These two functions get called from the InputField deeper within the Hierarchy


    //This Function gets called to update our local Player's talk Variable 
    //(this can be useful if we want to implement something like "display '...' when player is typing", i think)
    public void UpdateText (string typedText)
    {
        PlayerData.localPlayer.talk = typedText;
    }

    //This function gets called to transmit over the network to show on all clients
    public void Say()
    {
        PlayerData.localPlayer.Say();
    }
    
}
