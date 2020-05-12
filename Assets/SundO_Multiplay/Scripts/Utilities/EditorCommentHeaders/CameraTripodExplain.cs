using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTripodExplain : MonoBehaviour
{
    [
        Header("It is often helpful to have a parent GameObject to the MainCamera. ", order = 0), Space(-10, order = 1),
        Header("This can help with Rotations, or AudioListeners for 3rd person Cameras.", order = 2), Space(15, order = 3),
        Header("If you have  First Person Camera, you probably don't need this.", order = 4), Space(15, order = 5),
    ]
    public bool okay;
}
