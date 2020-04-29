using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script will connect some of our Player related Variables to the Unity 3D Animation System
//that is set up mostly using the "Animator" Window in the Editor.
//Because we use a Unity System, there is much less Code we have to write
//but many more things that are specific to Unity that we need to know.
//For more info on how Unity's Animation System works, see: https://docs.unity3d.com/Manual/AnimationOverview.html

public class AnimationScript3DAnimatedMesh : MonoBehaviour
{
    
    PlayerData myPlayer;                        //we assume this one is in our Parent GameObject
    Animator myAnimator;  //We assume this one is in a child GameObject


    //we also introduce a way of smoothing out variable values over time here.
    //this is sometimes helpful, if our values change every frame, but we don't want 
    //to base our decision making on every single frame, but on an average of multiple frames

    //in this specific case, because of the way we do things over the network, 
    //our "movementDirection" in PlayerData changes frequently, sometimes even though it shouldn't
    //leading to "flickering" animation.
    //this is because the package transmission over the network doesn't arrive in time 
    //for the moveDirection Variable to update (it checks for changes every frame).
    //so it makes more sense to look at the average change for movementDirection over a couple of frames

    //an easy way to do this is with a List in which we store the values over time.
    //If we use lists like this, we often call them "Buffer" - accumulating data over time is called "buffering"
    List<float> valueBuffer;
    
    //we can decide over how many frames we want to average a value
    public int howManyValuesOverTime = 4;     //this is equivalent to the number of frame we wait for data to accumulate 


    //--------------------------------------
    //Make connections to all important things we need to read data from, and write data to
    //is what we usually do in Start()
    //--------------------------------------

    private void Start()
    {
        //initialize our variables with which we get access to other scripts that are important to us
        myPlayer = GetComponentInParent<PlayerData>();      //we will need some variables from PlayerData, namely movementDirection
        myAnimator = GetComponentInChildren<Animator>();    //this is new - it's a Unity Component that takes care of animation for us

        //because we want to use our own, custom made Animationcontroller, 
        //and not the one that maybe came with the Asset, we will load our own
        //from the resources folder
        myAnimator.runtimeAnimatorController = Resources.Load("SundO_Multiplay/AnimationControllers/HumanoidMovement") as RuntimeAnimatorController; ;

        //now we need to set up some things for the averaging of values over time

        //initialize our List, so the computer knows to reserve memory for it
        valueBuffer = new List<float>();

        //Note: not all Buffers are used to average things out! 
        //You might notice the word "buffering" appear in video or audio streaming.
        //people can do whatever they want with a buffer. we just so happen to like to create an average. :-)

        //then fill the initial list with values of 0, 
        //according to the amount of values we want to average over
        for (int i = 0; i < howManyValuesOverTime; i++)
        {
            valueBuffer.Add(0.0f);
        }
    }


    //--------------------------------------
    //Relay all information from PlayerData to our Animator, so they can choose which Animation to perform
    //The way our Animator is set up in the Editor is:
    //(you need to open the Animator window in the editor for this)
    //- there is a Parameter called "movementMagnitude", that controls blending between two animations
    //  You can see how it does this by clicking on the little Arrows connecting the block "Idle" 
    //  and the block "Humanoid_Walk"
    //- we can access this value in our Script, by calling a function on the Animator Component we connected to in Start()
    //- it makes most sense to look at our "movementDirection" value in PlayerData, because we know
    //  that our Move-Script is writing the distance our Player has moved (and her direction) to this Variable
    //- so we try to change our Animator Parameter according to the Magnitude of our Movement

    //Also! We will smooth the movementDirection value over time, as framerate and network transmission 
    //is a bit flaky, which can lead to this number jumping back and forth between 0 and 1,
    //which in turn lead to very jumpy animation.
    //
    //Remember that Update gets called every frame, so all things here also get called every Frame!
    //--------------------------------------

    private void Update()
    {
        //we use a function that we wrote, "GetFloatValueOverTime() to do the averaging for us.
        //the nice thing about having this in a function, is that the code can be used 
        //in other contexts.

        //This is why we called our function "GetFloatValueOverTime"- not because it sounds official
        //but because the function literally averages a float value over a number of frames, 
        //if it is called every frame

        //and the Value we want to average over time is myPlayer.movementDirection.sqrMagnitude,
        //so we pass it to the function
        float totalMoveMagnitude = GetFloatValueOverTime(myPlayer.movementDirection.sqrMagnitude);

        myAnimator.SetFloat("movementMagnitude", totalMoveMagnitude);
    }

    //--------------------------------------
    //Remember that Update gets called every Frame?
    //So this function also gets called every frame!
    //It keeps track of a specific Value
    //--------------------------------------

    //this function gives back a value after it finished.
    //that is why it's not a "void" function, but a "float" function!
    float GetFloatValueOverTime(float myValue)
    {
        //remember which value was passed? It was myPlayer.movementDirection.sqrMagnitude!
        //now in this function, the value that was passed is coming in as "myValue"
        //let's add the newest value that we got from the place that this function was called
        //to our List of Values that we want to average
        valueBuffer.Add(myValue);

        //now that we added a new value, our List is larger than we wanted it to be
        //so we remove the "oldest" value that was put in
        valueBuffer.RemoveAt(0);

        //in a List, the oldest Value is 0, the newest value is the last one in the list!
        //now on to the averaging!

        //initialize a variable which will hold the sum of all the values in our List
        float sumOfAllValues = 0.0f;

        //add everything in our List together
        for (int i=0;i<howManyValuesOverTime;i++)
        {
            sumOfAllValues += valueBuffer[i];
        }
        
        //initialize a variable that holds the average value
        float averageValue = 0.0f;

        //calculate the average
        //Note how we need to cast our howManyValuesOverTime to a float!
        averageValue = sumOfAllValues / (float)howManyValuesOverTime;

        //send it back to whoever wanted to have it
        return averageValue;
    }

}
