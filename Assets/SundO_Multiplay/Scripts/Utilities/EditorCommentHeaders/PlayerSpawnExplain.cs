using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnExplain : MonoBehaviour
{
    [
        Header("This is a GameObject that is used by the Mirror-Networking Package.", order = 0), Space(-10, order = 1),
        Header("Once you have set up your PlayerPrefab in the NetworkManager Component, this is where the Player will Spawn after Connecting to Server.", order = 2), Space(15, order = 3),
        Header("You need at least one of these in your Scene, but can have multiple.", order = 4), Space(15, order = 5),
    ]
    public bool okay;
}
