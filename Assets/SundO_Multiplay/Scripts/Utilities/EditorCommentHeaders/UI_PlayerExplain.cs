using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerExplain : MonoBehaviour
{
    [
        Header("This is the local Player's UI, showing once a Connection has been established. ", order = 0), Space(-10, order = 1),
        Header("Some of itis used for moving the local Player via On-Screen Buttons (see PlayerUIMovement Component).", order = 2), Space(-10, order = 3),
        Header("Some of it is used to manipulate the local Player's Camera (see PlayerUIZoom).", order = 4), Space(15, order = 5),
        Header("You do not have to use this Prefab, if you want to implement your own.", order = 6), Space(15, order = 7),
    ]

    public bool okay;
}
