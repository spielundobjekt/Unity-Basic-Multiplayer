using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a Helper class that will create the adequate Representation GameObject
//for all Players

//We presume this script is part of the same GameObject, that holds the PlayerData Script

public class PlayerRepresentationSelector : MonoBehaviour
{
    public PlayerData myPlayerData;
    
    //Here we set up the Modules that we would like for our Player to have
    //These link to Prefabs
    public GameObject representationPrefab2D;
    public GameObject representationPrefab3DBillboards;


    //Here we keep a reference to the Modules we have created, so that we can destroy them again if we want to...
    public GameObject rep2D;
    public GameObject rep3DBillboard;

    //And in Awake, we create them, depending on the Game State
    //Why not in Start?
    //Well, because I do not have complete control over the order in which the 
    //Start Function is called in the individual Player-related Scripts
    //So Best make sure things are ready when other scripts call their Start() function
    private void Awake()
    {
        //get connected to the PlayerData Script
        myPlayerData = GetComponent<PlayerData>();

        //Let's create both representation modules - so we can switcxh them on or off later
        rep2D = Instantiate(representationPrefab2D, this.transform);
        rep3DBillboard = Instantiate(representationPrefab3DBillboards, this.transform);
        
    }

    private void Start()
    {
        SwitchRepresentation(GameData.instance.clientRepresentation);
    }

    //We use this function to switch between different representations
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
