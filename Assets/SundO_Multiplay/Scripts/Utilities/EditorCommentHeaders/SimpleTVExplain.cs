using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTVExplain : MonoBehaviour
{
    [
        Header("This is a simple VideoPlayer. ", order = 0), Space(-10, order = 1),
        Header("(see Child GameObjects, and note the RenderTexture Object in VideoPlayer and RawImage!)", order = 2), Space(-10, order = 3),
        Header("Note that on WebGL, you must Play videos from the StreamingAssets Folder!", order = 4), Space(15, order = 5),
        Header("The SpriteRenderer used here is just an example, you can replace it with anything you like.", order = 6), Space(15, order = 7),
    ]
    public bool okay;
}
