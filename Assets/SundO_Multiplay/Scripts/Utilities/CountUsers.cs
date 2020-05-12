using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A small script that counts the number of users on this server

public class CountUsers : MonoBehaviour
{
    public TMPro.TextMeshProUGUI myUIText;


    // Start is called before the first frame update
    void Start()
    {
        myUIText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameData.instance.players.Count > 0)
        {

            myUIText.text = GameData.instance.players.Count.ToString();
        }
        else
        {
            myUIText.text = "...";
        }
    }
}
