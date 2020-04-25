using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayAudio : Action
{

    public override void PerformAction()
    {
        Debug.Log("Performing Action to Play or Stop Sound!!!");
        AudioSource myAudioSource = GetComponent<AudioSource>();

        if (myAudioSource.isPlaying)
        {
            myAudioSource.Stop();
        }
        else
        {
            myAudioSource.Play();
           
        }
    }

}
