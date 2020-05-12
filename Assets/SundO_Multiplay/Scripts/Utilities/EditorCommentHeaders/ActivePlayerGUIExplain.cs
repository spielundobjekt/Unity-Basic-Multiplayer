using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePlayerGUIExplain : MonoBehaviour
{
    [
        Header("You can put all sorts of GameObjects within this GameObject to reveal them after the Player has connected. ", order = 0), Space(-10, order = 1),
        Header("This can help with too much UI clutter before the Player entered the server.", order = 2), Space(15, order = 3),
        Header("In our examples, we use it to hide the Chat Input Field, the UI-Navigation, and a TextMesh to explain the controls.", order = 4), Space(15, order = 5),
    ]
    public bool okay;
}
