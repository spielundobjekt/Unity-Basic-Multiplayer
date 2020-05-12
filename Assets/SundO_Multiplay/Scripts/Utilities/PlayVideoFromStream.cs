using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
//A helper script that is needed for WebGL Video Playback
/// </summary>

public class PlayVideoFromStream : MonoBehaviour
{
    //a Reference to our VideoPlayer Component - needs to be on the same GameObject
    UnityEngine.Video.VideoPlayer myVideoPlayer;

    //this show up in editor, and is hopefully helpful for people...
    [
        Header("Unfortunately, Unity's built in VideoPlayer can only Play Videos on WebGL with this Script. ", order = 0), Space(-10, order = 1),
        Header("This Script should be in the same GameObject as your VideoPlayer and AudioSource!", order = 2), Space(15, order = 3),
        Header("Note that you need a separate Rendertexture for each Video, if you want multiple Videos to Play at the same time!", order = 4), Space(15, order = 5),
    ]
    //the filename of the video we want to play - note that we need to write the file ending (e.g. .mp4) 
    //as opposed to loading things from the "Resources" Folder, where we need to omit the file ending!
    [Tooltip("Filename of video in StreamingAssets Folder")]
    public string filename;

    //a variable that can hold a video file reference from a web location
    [Tooltip("Leave empty if loading video from file")]
    public string url;

    // We execute our code every time this GameObject is enabled 
    // That way, we can use the ActionSetActive() Script to turn on/off video
    private void OnEnable()
    {
        //get reference to VideoPlayer on same GameObject
        myVideoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();

        //check if we should load a video from the Internet (there's something written in URL) 
        if (string.IsNullOrEmpty(url))
        {
            url = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
        }

        //set the url we want to stream video from - not tested if anything other than a file location
        myVideoPlayer.url = url;

        //check if we want to Output our Audio through an AudioSource Component.
        //Note: this does not work on WebGL!
        if (myVideoPlayer.audioOutputMode == UnityEngine.Video.VideoAudioOutputMode.AudioSource)
        {
            //this uses an audio source directly on the GameObject of the Videoplayer!
            myVideoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());
        }

        //Set the VideoPlayer to Play()
        myVideoPlayer.Play();
    }

}
