using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareStreamingAudio : MonoBehaviour
{
    AudioSource myAudio;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        myAudio.clip.LoadAudioData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
