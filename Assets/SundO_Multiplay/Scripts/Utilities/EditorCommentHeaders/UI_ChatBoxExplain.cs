using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChatBoxExplain : MonoBehaviour
{
    [
         Header("This is a separate UI Element for the local Player that serves as a Chat Input (see Hierarchy). ", order = 0), Space(-10, order = 1),
         Header("It is set up to always follow the local Plazer (see AlwaysFollowPlayer Component)", order = 2), Space(-10, order = 3),
         Header("The InputField lower in the Hierarchy calls functions in the PlayerSay Component.", order = 4), Space(15, order = 5),
         Header("You can try playing with the Canvas Settings or dis/enable AlwaysFollowPlayer ", order = 6), Space(15, order = 7),
     ]

    public bool okay;
}
