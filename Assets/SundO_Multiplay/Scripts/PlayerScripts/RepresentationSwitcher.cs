using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a Helper class whose functions get called from the RepresentationSelector UI
//it manages the switching between 2D and 3D Representations

public class RepresentationSwitcher : MonoBehaviour
{
    public GameObject room2D;               //a reference to a GameObject that contains all walls for the 2D room
    public GameObject room3DBillboards;     //a reference to a GameObject that contains all walls for the 3D room

    public GameObject camera2D;             //a reference to a GameObject that is set to be the Main Camera for the 2D Representation
    public GameObject camera3DBillboards;   //a reference to a GameObject that is set to be the Main Camera for the 3D Representation

    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    private void Start()
    {
        //here we cast an enum Representation to an integer number 
        //"casting" is basically similar to "converting"
        //it basically gives back the number of the enum in the enum list as an integer
        SwitchRepresentationModeAll((int)GameData.instance.clientRepresentation);
    }

    //--------------------------------------
    // We have to "turn off" a bunch of GameObjects 
    // and then "turn on" a bunch of others, 
    // depending on the representation we choose
    //
    // This is a function that can get called by a UI BUtton
    //--------------------------------------

    public void SwitchRepresentationModeAll(int newRepresentation)
    {
        //Update our new Representation on the GameData Script for everyone to see
        //here we cast an integer number to an enum Representation
        //"casting" is basically similar to "converting"
        //if the integer number has a representation in the list of enums, it becomes that enum
        GameData.instance.clientRepresentation = (Representation)newRepresentation;

        //depending on the new Representation, we do different things
        switch (GameData.instance.clientRepresentation)
        {

            case Representation.REP_2D:
                //Also make sure that we update the main camera in the GameData Script
                //so our other scripts that will do things with it do the right thing
                GameData.instance.cameraTripod = camera2D;

                //Then switch the rooms on or off
                room3DBillboards.SetActive(false);
                room2D.SetActive(true);
                break;

            case Representation.REP_3D_BILLBOARDS:
                //Also make sure that we update the main camera in the GameData Script
                //so our other scripts that will do things with it do the right thing
                GameData.instance.cameraTripod = camera3DBillboards;

                //Then switch the rooms on or off
                room3DBillboards.SetActive(true);
                room2D.SetActive(false);
                break;
        }

        //Finally, update all the PlayerScripts to use the new Representation
        foreach (PlayerData pd in GameData.instance.players)
        {
            pd.GetComponent<PlayerRepresentationSelector>().SwitchRepresentation(GameData.instance.clientRepresentation);
        }
    }

}
