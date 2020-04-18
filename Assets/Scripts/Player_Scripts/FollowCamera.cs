using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : Mirror.NetworkBehaviour
{
    PlayerData myPlayer;
    Camera myCamera;

    // Start is called before the first frame update
    void Start()
    {
        myPlayer = GetComponent<PlayerData>();
        myPlayer.myCamera = Camera.main;
        myCamera = myPlayer.myCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //calculate camera and player offset - how close is the player to the center of the camera?

        Vector3 cameraMoveDirection = transform.position - myCamera.transform.position;

        //not interested in moving in the z-Axis, so we pretend there's no difference
        cameraMoveDirection.z = 0;

        myPlayer.myCamera.transform.Translate(0.005f * cameraMoveDirection);
    }
}
