using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Script allows for Zooming in and out of the MainCamera
/// It is part of the PlayerUIBridge Prefab
/// </summary>
public class PlayerUIZoom : MonoBehaviour
{
    private bool bZoomedIn = false;            //we use this variable to remember if we are zoomed in or out                                                

    [Tooltip("Set this to true if you want the Camera to rotate clockwise on Zoom In")]
    public bool bRotateWhenZooming = false;    //we use this variable to say if we want to rotate our camera when zoomed in


    //the new values for our camera that describe a zoomed in or zoomed out state
    //Note: these numbers differ significantly depending on what type of projection 
    //our Camera uses - orthographic (numbers from 1 to 10) or perspective (numbers from 20 to 120)
    [Tooltip("Camera's Size (Orthographic) or FOV (Perspective) when zoomed In")]
    public float zoomInFactor;
    [Tooltip("Camera's Size (Orthographic) or FOV (Perspective) when zoomed Out")]
    public float zoomOutFactor;

    //the angle our camera-tripod should rotate to when zooming
    [Tooltip("How many degrees we rotate clockwise when zooming (not tested for negative values)")]
    public float zoomRotationAngle = 30.0f;


    //we use these two boolean values to synchronize zooming and rotating
    //our zoom is only finished (and our variable bZoomedIn flipped) if both of these are true
    bool bZoomFinished = false;
    bool bSwivelFinished = false;


    //this is a simple way to zoom in on what's happening
    public void CameraZoom()
    {
        //first check if we want to zoom in or zoom out, depending on the state of our boolean bZoomedIn
        if (!bZoomedIn)
        {
            //get a reference to the ACTUAL Camera in the scene
            Camera myCamera = Camera.main;

            //depending on the projection type of our camera, we need to change different values
            if (myCamera.orthographic)
            {
                myCamera.orthographicSize = zoomInFactor;
            }
            else
            {
                myCamera.fieldOfView = zoomInFactor;
            }
            bZoomedIn = !bZoomedIn;
        }
        else
        {
            Camera myCamera = Camera.main;
            if (myCamera.orthographic)
            {
                myCamera.orthographicSize = zoomOutFactor;
            }
            else
            {
                myCamera.fieldOfView = zoomOutFactor;
            }
            bZoomedIn = !bZoomedIn;
        }
    }

    //this is a somewhat fancy way of zooming in and out
    public void FancyZoom()
    {
        //In this script, we want to zoom smoothly over time, 
        //we want to do this with an IEnumerator, as described here: http://hyperdramatik.net/mediawiki/index.php?title=Algorithms#Was_ist_ein_IEnumerator_und_wann_benutze_ich_ihn
        //Let's check what projection Type our camera has:
        Camera myCamera = Camera.main;
        if (myCamera.orthographic)
        {
            //set our boolean to make everyone know we are currently zooming
            bZoomFinished = false;

            //start the Zooming IEnumerator
            StartCoroutine(FancyZoomOverTimeOrthographic());

            //Check if we want to also swivel the camera 45 degrees while zooming
            if (bRotateWhenZooming)
            {
                //set our boolean to make everyone know we are currently swivelling
                bSwivelFinished = false;
                //start the Swiveling IEnumerator
                StartCoroutine(SwivelCameraOverTime());
            }
            else        //if we do not want to rotate the camera while zooming, we have to pretend that we did that already
            {
                bSwivelFinished = true;
            }
            //start an IEnumerator that checks for both of the others if they are finished, 
            //so we can update our bZoomedIn boolean
            StartCoroutine(WaitForAllThingsToFinish());
        }

    }

    //A function that slowly zooms in over Multiple Frames
    IEnumerator FancyZoomOverTimeOrthographic()
    {

        //first, let's get a reference to our ACTUAL main Camera
        Camera myCamera = Camera.main;

        //then check if we want to zoom in or out
        //depending on the state of the zoom right now, we want to add or subtract zoom steps over time
        //so we create this little helper float variable and set it to be either positive or negative, depending on
        //if we want to zoom in or out

        float inOut = 0.0f;

        if (bZoomedIn)
        {
            inOut = 1.0f;
        }
        else
        {
            inOut = -1.0f;
        }
        //we do this, so we can now use this variable in our code and multiply the value we add to the actual camera zoom.
        //if this variable is positive, our values get ADDED to that.
        //if it is negative, our values get SUBTRACTED from that


        //then run this code until we have successfully zoomed all the way in or out
        //in a while loop, check if we have already zoomed in or out enough, depending on our bZoomedIn variable
        //Note: this is a somewhat complex boolean thing. I am trying to run this code for two cases.
        //Also note: i am a bit lazy.
        while ((myCamera.orthographicSize > zoomInFactor && !bZoomedIn) || (myCamera.orthographicSize < zoomOutFactor && bZoomedIn))
        {
            //if we haven't, then this code in here gets executed!
            //so let's zoom a tiny amount (depending on the user's framerate
            myCamera.orthographicSize += inOut * Time.deltaTime * 5.0f;
            //and then let's wait for one Frame before evaluating our while loop again
            yield return new WaitForEndOfFrame();
        }


        //to make sure that we don't move in or out too much, given unknown framerate on people's devices,
        //set the final zoom value to the one we specified in zoomInFactor

        if (!bZoomedIn)
        {
            myCamera.orthographicSize = zoomInFactor;
        }
        else
        {
            myCamera.orthographicSize = zoomOutFactor;
        }

        bZoomFinished = true;
    }

    ///A function that slowly rotates the camera tripod over Multiple Frames
    IEnumerator SwivelCameraOverTime()
    {
        //we don't want to rotate the camera, but instead rotate the tripod that the camera is attached to
        //so we need to get a reference to it
        GameObject tripod = GameData.instance.cameraTripod;

        float inOut = 0.0f;

        if (bZoomedIn)
        {
            inOut = -1.0f;
        }
        else
        {
            inOut = 1.0f;
        }

        //take the same amount of time as the zooming we do in FanzyZoomOverTime
        while ((tripod.transform.eulerAngles.y < zoomRotationAngle && !bZoomedIn) || (tripod.transform.eulerAngles.y > 0.0f && tripod.transform.eulerAngles.y < 180.0f && bZoomedIn))
        {
            //if we haven't, then this code in here gets executed!
            //so let's zoom a tiny amount (depending on the user's framerate
            tripod.transform.Rotate(0, inOut * Time.deltaTime * 25.0f, 0);
            //and then let's wait for one Frame before evaluating our while loop again
            yield return new WaitForEndOfFrame();
        }

        if (bZoomedIn)
        {
            tripod.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            tripod.transform.eulerAngles = new Vector3(0, zoomRotationAngle, 0);
        }

        bSwivelFinished = true;
    }

    //since we are performing two things at the same time, and don't know when either of those two things are finished, 
    //we check for two boolean variables that will be set when each IEnumerator has finished.
    IEnumerator WaitForAllThingsToFinish()
    {
        //wait until everything finishes
        while (!(bZoomFinished && bSwivelFinished))
        {
            yield return new WaitForEndOfFrame();
        }
        //then flip the state of our camera
        bZoomedIn = !bZoomedIn;

    }

}
