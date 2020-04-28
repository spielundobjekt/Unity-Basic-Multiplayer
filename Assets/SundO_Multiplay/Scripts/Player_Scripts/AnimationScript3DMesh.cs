using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script will connect some of our Player related Variables to the Unity 3D Animation System
//that is set up mostly using the "Animator" Window in the Editor.

public class AnimationScript3DMesh : MonoBehaviour
{
    public int walkSmoothing = 4;     //this is equivalent to the number of frame we wait for data to accumulate 
    
    PlayerData myPlayer;                        //we assume this one is in our Parent GameObject
    Animator myAnimator;  //We assume this one is in a child GameObject

    List<float> moveMagnitudes;

    //Make connections to all important things we need to read data from, and write data to

    private void Start()
    {
        myPlayer = GetComponentInParent<PlayerData>();
        myAnimator = GetComponentInChildren<Animator>();
        myAnimator.runtimeAnimatorController = Resources.Load("SundO_Multiplay/AnimationControllers/HumanoidMovement") as RuntimeAnimatorController; ;
        moveMagnitudes = new List<float>();
        //fill our buffer with 0s
        for (int i = 0; i < walkSmoothing; i++)
        {
            moveMagnitudes.Add(0.0f);
        }
    }


    //Relay all information from PlayerData to our Animator, so they can choose which Animation to perform
    //also, we should smooth this value over time, as framerate is a bit flaky, which can lead to 
    //this number jumping back and forth between 0 and 1
    private void Update()
    {
        //put in the newest value for movement magnitude
        moveMagnitudes.Add(myPlayer.movementDirection.sqrMagnitude);

        //remove the last value of movement magnitude
        moveMagnitudes.RemoveAt(0);

        //add everything together
        float totalMoveMagnitude = 0.0f;
        foreach (float f in moveMagnitudes)
        {
            totalMoveMagnitude += f;
        }
        totalMoveMagnitude = totalMoveMagnitude / (float)walkSmoothing;

        myAnimator.SetFloat("movementMagnitude", totalMoveMagnitude);
    }

}
