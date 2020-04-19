using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Class holds information about our game that can be accessed by all other scripts
//to make decisions based on the game state

public enum Representation {REP_2D, REP_3D_BILLBOARDS }

public class GameData : MonoBehaviour
{
    //We do this, so that we can always access all the variables of this script from all other scripts
    //It is called: a Singleton Pattern
    //Patterns are a way in which people program that has proven to be very useful. Like a recipe of something yummy.

    //The reason we use this pattern is - there will only ever be one single Object of this Type in our game.
    //We should never use this Pattern for GameObjects that have multiple Instances running - like PlayerData for example
    public static GameData instance;

    public Representation clientRepresentation;

    public GameObject mainCamera;

    public List<PlayerData> players;


    //Here we set the static variable to the instance of this script
    private void Awake()
    {
        instance = this;
    }

}
