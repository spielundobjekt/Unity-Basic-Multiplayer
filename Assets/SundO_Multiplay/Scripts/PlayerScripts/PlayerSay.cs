using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We use an Action to change the Text floating above our head.
//We are also trying out IEnumerators here.
//

public class PlayerSay : MonoBehaviour
{
    public TMPro.TextMeshProUGUI talkDisplayText;
    public PlayerData myPlayer;

    public void UpdateText (string typedText)
    {
        PlayerData.localPlayer.talk = typedText;
    }

    public void Say()
    {
        PlayerData.localPlayer.Say();
    }
    
}
