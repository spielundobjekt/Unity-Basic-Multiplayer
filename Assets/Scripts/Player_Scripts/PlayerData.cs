using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class holds information that should be accessible by a number of scripts
//some of those will be attached to this gameobject
//some of them will be children to this gameobject

    //It is often good practice to split the data that multiple scripts access
    //into its own separate script

public class PlayerData : Mirror.NetworkBehaviour
{
    public Vector3 movementDirection;       //here, we will store the Vector this player is moving in
    public Quaternion lookRotation;         //here we store the Rotation of the Player's gaze

    public string characterName;            //this is the name of our character Sprite 

    Surroundings mySurroundings;            //helper Object to check for things around us, 
                                            //will be used later

    //We do some things in Awake() because this gets called before the Start() Script of anything else is called.
    private void Awake()
    {
        Debug.Log("I am alive!");

        //connect to the script that does the check for the surroundings
        mySurroundings = GetComponent<Surroundings>();

        //Let's let the GameData Script know that we exist!
        GameData.instance.players.Add(this);


        //here we have to distribute our name to all other clients

        //first, only do this for the local Player, because they have access to the name the Player was given by the User
        if (isLocalPlayer)
        {
            //get the name from the input box -
            //but what if there is nothing written in the input box? Well, then we are called friedrich
            if (CharacterUISetupBridge.localCharacterName.Equals("none")){

                characterName = "friedrich";
            }
            else
            {
                characterName = CharacterUISetupBridge.localCharacterName;
            }
            
            //now that we have figured out our name on our specific computer, let's send this information to all the other computers
            RpcMyName(characterName);
        }
    }

    [Mirror.ClientRpc]
    void RpcMyName(string name)
    {
        characterName = name;
    }

    //This gets called when the Network shuts down
    //Or the Player is otherwise removed
    private void OnDestroy()
    {
        //since we keep track of our players in the GameData.instance.players List, we need to keep it tidy
        //so if one Player leaves, we need to remove her reference from that list
        GameData.instance.players.Remove(this);
    }



}
