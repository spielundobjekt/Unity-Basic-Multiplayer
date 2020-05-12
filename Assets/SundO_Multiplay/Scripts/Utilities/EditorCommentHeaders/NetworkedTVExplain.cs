using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedTVExplain : MonoBehaviour
{
    [
        Header("This is a networked VideoPlayer. ", order = 0), Space(-10, order = 1),
        Header("(see Child GameObjects, note the Action Component in this GameObject, the Button in the Hierarchy, ", order = 2), Space(-10, order = 3),
        Header("and how it is connected to UI-ActionInterface!)", order = 4), Space(-10, order = 5),
        Header("Note that on WebGL, you must Play videos from the StreamingAssets Folder!", order = 6), Space(15, order = 7),
        Header("The SpriteRenderer used here is just an example, you can replace it with anything you like.", order = 8), Space(15, order = 9),
    ]
    public bool okay;
}
