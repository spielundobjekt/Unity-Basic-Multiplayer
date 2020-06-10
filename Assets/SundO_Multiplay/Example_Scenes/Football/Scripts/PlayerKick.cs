using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    public float baseKickStrength;
    public float baseKickLift;
    public float kickMultiplier;

    private float kickStrength;
    private float kickLift;

    Rigidbody football;
    PlayerData myPlayer;
    bool bOnBall = false;

    // Start is called before the first frame update
    void Start()
    {
        myPlayer = GetComponent<PlayerData>();
        football = GameObject.FindWithTag("Football").GetComponent<Rigidbody>();
        kickStrength = baseKickStrength;
        kickLift = baseKickLift;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (football && collision.rigidbody == football)
        {
            bOnBall = true;
            Debug.Log("Got the Ball!");
            kickStrength = baseKickStrength;
            kickLift = baseKickLift;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (football && collision.rigidbody == football)
        {
            bOnBall = false;
            Debug.Log("Lost the Ball!");
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        //kick higher the longer we hold spacebar
        if (Input.GetKey(KeyCode.Space) && bOnBall)
        {
            kickLift += kickMultiplier * Time.deltaTime;

            kickStrength += kickMultiplier * Time.deltaTime;

            kickLift = Mathf.Min(kickLift, .85f);
            kickStrength = Mathf.Min(kickStrength, baseKickStrength * 10.0f);
        }   

        if (Input.GetKeyUp(KeyCode.Space) && bOnBall)
        {
            Debug.Log("Kicking with Strength " + kickStrength);
            football.AddForce((myPlayer.movementDirection + new Vector3(0, kickLift, 0) )* kickStrength );
            kickStrength = baseKickStrength;
            kickLift = baseKickLift;
        }


    }
}
