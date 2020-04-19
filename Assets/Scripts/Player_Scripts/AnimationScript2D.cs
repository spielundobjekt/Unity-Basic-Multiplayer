using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq.Expressions;
using System.IO;
//#if UNITY_EDITOR 
//using UnityEditor;
//#endif

public class AnimationScript2D : MonoBehaviour
{
	public Vector2 speed = new Vector2(2.2f, 2.2f);

    //Here we extract Sprites from a texture, and then flip through them 
    //to create our animation
	public Sprite[] idleAnim;
	public Sprite[] wlAnim;				//only have one side-walking anim because we flip it!
	public Sprite[] wdAnim;
	public Sprite[] wuAnim;
    public Sprite death;

	public string charName;
	public bool bReset = false;

    public bool bDead;

	// 2 - Store the movement
		
	[HideInInspector]
	public Vector3 pos;
	public Vector3 mov;

	//animation states - the values in the animator conditions
	const int STATE_IDLE = 0;
	const int STATE_WALK_UP = 1;
	const int STATE_WALK_DOWN = 2;
	const int STATE_WALK_LEFT = 3;
	const int STATE_WALK_RIGHT = 4;
	const int STATE_BATTLE = 5;
	const int STATE_SUPER_BATTLE = 6;

	public int myAnimState;								

	public float animSpeed=10;
	public bool bFlippedAnim = false;
	public string myDirection = "left";					

    public bool bWalkInCircles = false;

	SpriteRenderer sr;
	float animFrame;
    Sprite mySprite;


    Vector3 	oldLocation;

	TextMesh    infoText;

     PlayerData myPlayer;

    
    /// <summary>
    /// Functions from here

    /// </summary>



	// Use this for initialization
	void Start()
	{
        myPlayer = GetComponentInParent<PlayerData>();

        Debug.Log("AnimationScript 2D at the ready!");
		sr = this.GetComponent<SpriteRenderer>();
		infoText = GetComponentInChildren<TextMesh> ();
		//infoText.text=charName;
		pos = transform.position;
        GenerateSprites ();
        //loadFromDisk();

        
        
	}

    

    public void GenerateSprites(){
		//live loading of sprites

		Texture2D charImage = Resources.Load ("Characters/"+charName+"_character") as Texture2D;
		charImage.filterMode = FilterMode.Point;

        //since we will always be showing the same sprite, no matter how we are facing,
        //our idle animation only needs one slot
        idleAnim = new Sprite[1];

		wdAnim = new Sprite[3];
		wuAnim = new Sprite[3];
		wlAnim = new Sprite[3];

        for (int i = 0; i < 3; i++)
            wdAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 144, 48, -48), new Vector2(0.4f, 0.8f),16.0f);
        
		for (int i=0;i<3;i++)
			wuAnim[i]= Sprite.Create (charImage, new Rect(i*48,96,48,-48), new Vector2 (0.4f,0.8f), 16.0f);

		for (int i=0;i<3;i++)
			wlAnim[i]= Sprite.Create (charImage, new Rect(i*48,48,48,-48), new Vector2 (0.4f,0.8f), 16.0f);

        

	}



	void Update(){

        setAnimationForCharacter();
        animFrame += Time.deltaTime * 100.0f;
        animateState();
        
    }


	
	public void setAnimationForCharacter(){

        //make sure our animation state is IDLE - we will change our state if there is a need for that
		myAnimState = STATE_IDLE;

        //transfer our movement into a local variable
        //we get the movement information from the variable PlayerData Script, 
        //which we access thorugh our variable myPlayer
        
        Vector3 locDelta = myPlayer.movementDirection;

       
		//are we moving?
		if (locDelta.sqrMagnitude>0.0f){
			//are we moving more left-right-ish or more up-down-ish?
			if (Math.Abs (locDelta.x) >= Math.Abs (locDelta.z)) {
				if (locDelta.x > 0.0f) {
					myAnimState = STATE_WALK_RIGHT;
					myDirection = "right";
				}
				if (locDelta.x < 0.0) {
					myAnimState = STATE_WALK_LEFT;
					myDirection = "left";
				}
			} else {
				if (locDelta.z < 0.0) {
					myAnimState = STATE_WALK_DOWN;
                    myDirection = "down";
                }
				if (locDelta.z > 0.0){
					myAnimState = STATE_WALK_UP;
                    myDirection = "up";
                }
			}
		}

        oldLocation = transform.position;
	}
	


	
       	
	//--------------------------------------
	// Change the players animation state
	//--------------------------------------
	void animateState(){

		int frame = ((int)animFrame/(int)animSpeed)%3;
		//int amountOfSloMo = 4;

		switch (myAnimState) {
			
		case STATE_IDLE:
			sr.sprite=wdAnim[1];
			break;

		case STATE_WALK_UP:
			sr.sprite=wuAnim[frame];
			break;
			
		case STATE_WALK_DOWN:
			sr.sprite=wdAnim[frame];
			break;
			
		case STATE_WALK_LEFT:
			sr.sprite = wlAnim [frame];
			if (bFlippedAnim) {
                sr.flipX = true;				
			} else {
				sr.flipX = false;				
			}
			break;

		case STATE_WALK_RIGHT:
			sr.sprite = wlAnim [frame];
			if (bFlippedAnim) {
                sr.flipX = false;
            } else {
                sr.flipX = true;
            }
			break;
		}

	}


}
