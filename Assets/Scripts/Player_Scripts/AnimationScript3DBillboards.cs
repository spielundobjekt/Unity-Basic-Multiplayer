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

public class AnimationScript3DBillboards : MonoBehaviour
{
	public Vector2 speed = new Vector2(2.2f, 2.2f);

    public int HowManyAnimationPicsperRow = 3;
    public int HowManyAnimationRows = 3;

    //instead of creating individual pictures, 
    //we just adjust our drawing coordinates 
    //of which part of our texture we want to draw 
    //these are only public so that we can look at the in the editor

    public Vector2[] idleAnim;
    public Vector2[] walkLeftRight;             
    public Vector2[] walkTowards;
    public Vector2[] walkAway;
    
    public string charName;
	public bool bReset = false;

    public bool bFlippedAnim = false;   //we check this if the Texture we use for animating has a different facing left/right sequence


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
	public string myDirection = "left";					

    public bool bWalkInCircles = false;

	MeshRenderer mr;
	float animFrame;
    Sprite mySprite;


    Vector3 	oldLocation;

	TextMesh    infoText;

    float   originalScale;

    PlayerData myPlayer;

    
    /// <summary>
    /// Functions from here

    /// </summary>

    void Reset(){
	
		bReset = false;
		Start ();
	}


	// Use this for initialization
	void Start()
	{
        myPlayer = GetComponentInParent<PlayerData>();

        originalScale = Mathf.Abs(transform.localScale.x);
        Debug.Log("Animation Script 3D Billboards at the ready!");
		mr = this.GetComponent<MeshRenderer>();
		infoText = GetComponentInChildren<TextMesh> ();
		//infoText.text=charName;
		pos = transform.position;
        GenerateTextureFromFile ();

        //Let's check if we are the player that belongs to this computer
        //and if we are not (this is someone else's representation), then flip their animation!
        // I do not know why we need to do this, but apparently non-local players are flipped...
        if (!myPlayer.isLocalPlayer)
        {
            bFlippedAnim = !bFlippedAnim;
        }
        //Note: if you want to assign a boolean to its opposite value, no matter how it is set now, then you can do:
        // bValue = !bValue;
        // it's like flipping a switch!

        


    }

    //tbd loading from disk
    public void loadFromDisk()
    {
        /*
        //load image from outside of the resource-file
        byte[] data = File.ReadAllBytes(Application.dataPath+"/character_"+GetComponent<MissionDataContainer>().clientID+".png");
        
        Texture2D charImage = new Texture2D(250, 250, TextureFormat.ARGB32, true);
        charImage.LoadImage(data);
        charImage.name ="mickey";

        charImage.filterMode = FilterMode.Point;

        //Texture2D charImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/M.png", typeof(Texture2D));
        //charImage.filterMode = FilterMode.Point;

        idleAnim = new Sprite[3];
        wdAnim = new Sprite[3];
        wuAnim = new Sprite[3];
        wlAnim = new Sprite[3];

        for (int i = 0; i < 3; i++)
            wdAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 144, 48, -48), new Vector2(0.5f, 0.5f));

        for (int i = 0; i < 3; i++)
            wuAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 96, 48, -48), new Vector2(0.5f, 0.5f));

        for (int i = 0; i < 3; i++)
            wlAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 48, 48, -48), new Vector2(0.5f, 0.5f));


//        mySprite = Sprite.Create(charImage, new Rect(0, 0, 250, 250), new Vector2(0.5f,0.5f), 256);
//        mySprite.name = "mickey";
//        GetComponent<SpriteRenderer>().sprite = mySprite;
        */
    }

    public void GenerateTextureFromFile(){
		//live loading of sprites

		Texture2D charImage = Resources.Load ("Characters/"+charName+"_character") as Texture2D;
		charImage.filterMode = FilterMode.Point;

        
        idleAnim = new Vector2[HowManyAnimationPicsperRow];
		walkTowards = new Vector2[HowManyAnimationPicsperRow];
		walkAway = new Vector2[HowManyAnimationPicsperRow];
		walkLeftRight = new Vector2[HowManyAnimationPicsperRow];

        //here is how we do this: 
        //the image that we loaded as a texture has 3 rows and 3 columns, 
        //in which the image parts lie that we want to flip through

        //we need to generate coordinates for these smaller positions
        //these coordinates are called "offsets" as they offset te position 
        //from which the content of the texture is drawn

        //they are like little coordinates within the texture

        //We presume, the first row contains the left/right animation sequence
        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
            walkLeftRight[i] = new Vector2(i * 1.0f / (float)HowManyAnimationPicsperRow, 0 * 1.0f/(float)HowManyAnimationRows);
        //We presume, the second row contains the walking-away animation sequence
        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
            walkAway[i] = new Vector2(i * 1.0f / (float)HowManyAnimationPicsperRow, 1.0f * 1.0f / (float)HowManyAnimationRows);

        //We presume, the third row contains the walking-towards-us animation sequence
        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
            walkTowards[i] = new Vector2(i* 1.0f/(float)HowManyAnimationPicsperRow, 2.0f * 1.0f / (float)HowManyAnimationRows);


        //we now need to make sure that we do not draw the whole texture, but only a part of it.
        //so we need to set our Texture Scale accordingly
        //we do not care about flipping right now, we can do this later.
        mr.material.mainTextureScale = new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);

        //finally, we will now set our Material in the Meshrenderer to use our Texture

        mr.material.mainTexture = charImage;

	}



	void Update(){

		if (bReset)
			Reset ();

        
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
		if (locDelta.magnitude>0.0f){
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

        //We use a local variable to set up our flipping, depending on the 
        float flipXY = 1.0f;
        if (bFlippedAnim) flipXY = -1.0f;

        //make sure our texture is not flipped before we start changing anything
        mr.material.mainTextureScale = new Vector2(flipXY / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);

        int frame = ((int)animFrame/(int)animSpeed)%3;
		//int amountOfSloMo = 4;

		switch (myAnimState) {
			
		case STATE_IDLE:
			mr.material.mainTextureOffset=walkTowards[1];
			break;

		case STATE_WALK_UP:
                mr.material.mainTextureOffset = walkAway[frame];
			break;
			
		case STATE_WALK_DOWN:
                mr.material.mainTextureOffset = walkTowards[frame];
			break;

            case STATE_WALK_LEFT:

                mr.material.mainTextureOffset = walkLeftRight[frame];
                break;
			
		case STATE_WALK_RIGHT:

                //we don't need to have extra information for walking left and right. 
                //We can simply flip the texture by setting its Texture Scale in x to -flipXY
                // This will basically change the sign of flipXY independently to what it is currently set.
                // so -1 will become 1
                // and 1 will become -1

                //But Beware! our offset are ...
                //...
                //offset! 
                //(haha)
                //this means, we must add one offset step whenever we flip our texture
                mr.material.mainTextureScale = new Vector2(-flipXY / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);
                mr.material.mainTextureOffset = walkLeftRight[frame] + new Vector2(1.0f / (float)HowManyAnimationPicsperRow,0.0f);
                break;
        }

    }


}
