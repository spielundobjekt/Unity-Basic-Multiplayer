using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surroundings : MonoBehaviour
{
    public List<TileData> thingsAroundMe;

    
    //add objects to surroundings
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        //check location of triggered object
        TileData newTile = collision.gameObject.GetComponent<TileData>();
        if (newTile)
        {
            thingsAroundMe.Add(newTile);
        }

    }


    //remove object from surroundings
    private void OnTriggerExit2D(Collider2D other)
    {
        TileData removeTile = other.GetComponent<TileData>();
        thingsAroundMe.Remove(removeTile);
    }

}
