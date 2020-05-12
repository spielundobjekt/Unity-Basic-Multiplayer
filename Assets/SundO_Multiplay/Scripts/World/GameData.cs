using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class holds information about our game that can be accessed by all other scripts
/// to make decisions based on the game state
/// The idea is, that we shouldn't have functions in this class
/// </summary>

//This is a VERY public enum. The fact that it is written outside of the class means 
//that it is "visible" to all classes!

public enum Representation {REP_2D, REP_3D_BILLBOARDS }

//Enums are similar to the way we substitute variable names for number in AnimationScript2D
//Instead of saying "const int MY_STATE = 0, we use enum MyEnumClassName {ENUM1, ENUM2, etc...}:
//It's pretty much the same, just a different (and faster) way of writing it.
//the all Caps writing is just a thing that people do.


public class GameData : MonoBehaviour
{
    //We do this, so that we can always access all the variables of this script from all other scripts
    //It is called: a Singleton Pattern
    //Patterns are a way in which people program that has proven to be very useful. Like a recipe of something yummy.

    //The reason we use this pattern is - there will only ever be one single Object of this Type in our game.
    //We should never use this Pattern for GameObjects that have multiple Instances running - like PlayerData for example
    public static GameData instance;

    //this shows up in editor and id hopefully helpful to people...
    [
        Header("*DO NOT DELETE* unless you know what you are doing! ", order = 0), Space(-10, order = 1),
        Header("This Object should be in your Scene only once!", order = 2), Space(-10, order = 3),
        Header("It holds Data on the Players currently on the Server", order = 4), Space(-10, order = 5),
        Header("Usually has the NetworkManager Component in the same GameObject", order = 6), Space(15, order = 7),
    ]
    public Representation clientRepresentation; //Here we create an Object from our Enum. 
                                                //The object can have all of the values that we set up in the {} before

    [Tooltip("A MainCamera Parent GameObject (we usually affectionately call it Tripod). If your Camera does not have Parent GameObject, then put in your MainCamera")]
    public GameObject cameraTripod;             //a link to the Gameobject that our Scripts should treat as a Reference Point of the Main Camera
                                                //this is useful for certain 2D or Isometric Representations

    [Tooltip("Don't add anything - will be populated from Script!")]
    public List<PlayerData> players;            //a list with all PlayerDataObjects in the Game right now

    [HideInInspector]
    public Mirror.NetworkManager netManager;    //a reference to the Network Component on the same GameObject

    [Tooltip("Select this to show Mirror Network GUI after Connection (if it was disabled before)")]
    public bool bShowGUIOnConnect = true;       //a bool that lets us decide to show the Network GUI on the top left after we connected

    //Here we set the static variable to the instance of this script
    //This is part of the Pattern described above.
    //This enables us to access all Variables in this class by saying GameData.instance.variableName - anywhere in our code!
    private void Awake()
    {
        instance = this;
        netManager = GetComponent<Mirror.NetworkManager>();
    }

    public void StartHostAndClient()
    {
        netManager.StartHost();

        if (bShowGUIOnConnect)
        {
            GetComponent<Mirror.NetworkManagerHUD>().showGUI = true;
        }
    }

    public void StartClient()
    {
        netManager.StartClient();

        if (bShowGUIOnConnect)
        {
            GetComponent<Mirror.NetworkManagerHUD>().showGUI = true;
        }
    }


}
