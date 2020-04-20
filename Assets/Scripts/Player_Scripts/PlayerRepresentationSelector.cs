using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a Helper class that will create the adequate Representation GameObject for all Players
/// Attention: We presume this script is part of the same GameObject that holds the PlayerData Script
/// </summary>

public class PlayerRepresentationSelector : MonoBehaviour
{
    
    public PlayerData myPlayerData;         //a variable that references our PlayerData Script
                                            //we use this Script to store and retrieve information about the player
                                            //from a whole bunch of scripts

    //Here we connect the Modules that we would like for our Player to have in the Editor!
    //These link to Prefabs
    public GameObject representationPrefab2D;
    public GameObject representationPrefab3DBillboards;


    //Here we keep a reference to the Modules we have created, so that we can destroy them again if we want to...
    public GameObject rep2D;
    public GameObject rep3DBillboard;

    //And in Awake, we create them, depending on the Game State
    //Why not in Start?
    //Well, because I do not have complete control over the order in which the 
    //Start() Function is called in the individual Player-related Scripts
    //So Best make sure things are ready when other other scripts call their Start() function!
    private void Awake()
    {
        //get connected to the PlayerData Script
        myPlayerData = GetComponent<PlayerData>();

        //Let's create both representation modules - so we can switch them on or off later
        //This will actually create GameObjects from the Prefabs we have set up in the Editor
        rep2D = Instantiate(representationPrefab2D, this.transform);
        rep3DBillboard = Instantiate(representationPrefab3DBillboards, this.transform);
        
    }

    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    private void Start()
    {
        //In this case, switching the representation and 
        //initializing the representation are kindof the same thing
        //so we just call this function
        SwitchRepresentation(GameData.instance.clientRepresentation);
    }

    //--------------------------------------
    //We use this function to switch between different representations
    //--------------------------------------
    public void SwitchRepresentation(Representation newRepresentation)
    {
        //Let's have a look at which new representation we want
        switch (newRepresentation)
        {
            //if we want to switch to 2D, we need to first disable our other representations
            case Representation.REP_2D:
                rep3DBillboard.SetActive(false);
                rep2D.SetActive(true);
                //reset Rotation for 2D Representation
                myPlayerData.transform.rotation = Quaternion.identity;
                break;

            case Representation.REP_3D_BILLBOARDS:
                rep3DBillboard.SetActive(true);
                rep2D.SetActive(false);
                break;
        }
    }

}
