using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnCollision : MonoBehaviour
{
    public float fadeTime = 3.0f;
    public Color flashColor;

    Material myMat;



    // Start is called before the first frame update
    void Start()
    {
        myMat = GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with Goal!");
        if (other.CompareTag("Football"))
        {
            Debug.Log("Gooooooooooooaaaaaaaalllll");
            StartCoroutine(FlashFade());
        }
    }


    IEnumerator FlashFade()
    {
        float currentTime = 0.0f;
        
        
        while (currentTime < fadeTime)
        {
            myMat.SetColor("_EmissionColor", flashColor * (fadeTime - currentTime));
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        
    }

}
