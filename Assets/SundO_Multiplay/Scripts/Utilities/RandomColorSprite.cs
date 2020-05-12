using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Helper Script that Randomly changes the color of a Sprite


public class RandomColorSprite : MonoBehaviour
{
    public float changeFrequency = 2.0f;

    public bool bRandomizeChange = true;

    public float minChange = 0.5f;
    public float maxChange = 2.5f;

    float currentElapsedTime = 0.0f;

    public Color lerpedColor;

    SpriteRenderer mySprite;

    public float blinkOffset;
    public bool bRandomBlinkOffset = false;

    public bool bStartedBlinking = false;


    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();

        if (bRandomizeChange)
        {
            changeFrequency = UnityEngine.Random.Range(minChange, maxChange);
        }
    }


    // Update is called once per frame
    void Update()
    {   


        currentElapsedTime += Time.deltaTime;
        if (currentElapsedTime<blinkOffset && !bStartedBlinking)
        {
            return;
        }

        if (currentElapsedTime > blinkOffset && !bStartedBlinking)
        {
            bStartedBlinking = true;
            currentElapsedTime = 0.0f;
        }

        if (currentElapsedTime > changeFrequency)
        {
            lerpedColor = new Color(UnityEngine.Random.Range(0.0f, 0.75f), UnityEngine.Random.Range(0.0f, 0.75f), 0.0f + UnityEngine.Random.Range(0.0f, 0.75f));
            mySprite.color = lerpedColor;
            currentElapsedTime = 0.0f;
        
        }
        
    }
}
