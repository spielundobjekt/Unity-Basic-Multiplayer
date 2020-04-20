using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This is a VERY public enum. The fact that it is written outside of the class means 
//that it is "visible" to all classes!

public enum Representation {REP_2D, REP_3D_BILLBOARDS }

//Enums are similar to the way we substitute variable names for number in AnimationScript2D
//Instead of saying "const int MY_STATE = 0, we use enum MyEnumClassName {ENUM1, ENUM2, etc...}:
//It's pretty much the same, just a different (and faster) way of writing it.
//the all Caps writing is just a thing that people do.

/// <summary>
/// This Class holds information about our game that can be accessed by all other scripts
/// to make decisions based on the game state
/// The idea is, that we shouldn't have functions in this class
/// </summary>


public class GameData : MonoBehaviour
{
    //We do this, so that we can always access all the variables of this script from all other scripts
    //It is called: a Singleton Pattern
    //Patterns are a way in which people program that has proven to be very useful. Like a recipe of something yummy.

    //The reason we use this pattern is - there will only ever be one single Object of this Type in our game.
    //We should never use this Pattern for GameObjects that have multiple Instances running - like PlayerData for example
    public static GameData instance;

    public Representation clientRepresentation; //Here we create an Object from our Enum. 
                                                //The object can have all of the values that we set up in the {} before

    public GameObject mainCamera;               //a link to the Gameobject that our Scripts should treat as the "Main Camera"

    public List<PlayerData> players;            //a list with all PlayerDataObjects in the Game right now


    //Here we set the static variable to the instance of this script
    //This is part of the Pattern described above.
    //This enables us to access all Variables in this class by saying GameData.instance.variableName - anywhere in our code!
    private void Awake()
    {
        instance = this;
    }

}
