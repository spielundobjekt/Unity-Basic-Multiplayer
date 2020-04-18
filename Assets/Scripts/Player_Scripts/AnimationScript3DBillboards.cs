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
	const int STATE_IDLE_TOWARDS = 0;
	const int STATE_IDLE_LEFT = 1;
	const int STATE_IDLE_RIGHT = 2;
	const int STATE_IDLE_AWAY = 3;
    const int STATE_WALK_AWAY = 4;
	const int STATE_WALK_TOWARDS = 5;
	const int STATE_WALK_LEFT = 6;
	const int STATE_WALK_RIGHT = 7;
	
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
    Camera myCamera;


    public float visibleDotProductForward;
    public float visibleDotProductRight;

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
        myCamera = Camera.main;

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

    void Update()
    {

        if (bReset)
            Reset();


        SetAnimationForCharacter();
        animFrame += Time.deltaTime * 100.0f;
        AnimateState();

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


    //--------------------------------------
    // Load the specific Texture from a .png file in the Resources Folder
    // At some point, we can point this to a location online to download more resources, or change them without changing the code
    //--------------------------------------
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

        //We also need to have multiple Idle animations now - one for each position we could be facing
        //These will be the center image in the animation sequence
        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
        {
            idleAnim[i] = new Vector2(1.0f * 1.0f / (float)HowManyAnimationPicsperRow, i * 1.0f / (float)HowManyAnimationRows);
        }


        //we now need to make sure that we do not draw the whole texture, but only a part of it.
        //so we need to set our Texture Scale accordingly
        //we do not care about flipping right now, we can do this later.
        mr.material.mainTextureScale = new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);

        //finally, we will now set our Material in the Meshrenderer to use our Texture

        mr.material.mainTexture = charImage;

	}


    //--------------------------------------
    // Figure out which Image of the Animation Sequence to show
    //--------------------------------------
    public void SetAnimationForCharacter(){

        //make sure our basic animation state is IDLE_TOWARDS - we will change our state if there is a need for that
		myAnimState = STATE_IDLE_TOWARDS;

        //First, let's find out if our Player is actually moving, or just standing around.
        bool bPlayerMoves = false;

        //We do this by looking at the Square Magnitude of MoveDirection (Square, because it is always positive that way!) in PlayerData
        if (myPlayer.movementDirection.sqrMagnitude > 0.5f)
        {
            bPlayerMoves = true;
        }

        //Second, let's find out which way this player is facing, relative to the main camera
        //We do this by comparing the Facing of our Player and the Facing of the Camera.
        //We will use Dot Product for this. It's Math! It's fun!

        Vector3 playerFacing = myPlayer.transform.forward;
        //make sure we ignore looking up or down
        playerFacing.y = 0;
        //and then make sure that our Vector is exactly 1 unit long - this is important for the Maths!
        playerFacing.Normalize();

        //do the same for the main camera
        Vector3 cameraFacing = myCamera.transform.forward;
        cameraFacing.y = 0;
        cameraFacing.Normalize();

        //and in order to determine if they are facing left or right from one another, we need to do the same for the right axis of the camera
        Vector3 cameraRight = myCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();


        //Compare the two vectors using the Dot Product: 
        //(ignore the first part of the website, start reading from "Why cos()")
        //https://www.mathsisfun.com/algebra/vectors-dot-product.html 
        //
        // A Dot Product works a bit like this. Imagine the Two Vectors in 3Dimensional Space as 2 Arrows.
        //In your Mind, move the Arrows so that the starting points of the arrows are touching each other, but leave their rotations as they were.
        //They should now stand at an angle to each other. 
        //The dot product gives you the angle between these two vectors, 
        //or the projection of one of them on the other.
        float dotProductForward = Vector3.Dot(cameraFacing, playerFacing);
        float dotProductRight = Vector3.Dot(cameraRight, playerFacing);

        //For Debugging Purposes:
        //Show me the Dot Products!
        visibleDotProductForward = dotProductForward;
        visibleDotProductRight = dotProductRight;


        //Now we can look at the two Dot Products, and depending on how large it is, they are facing in different directions:
        //If the first Dot Product comparing the two forward Vectors is less than 0, 
        //then the two vectors face each other (the projection of one Vector on another is negative)
        //If it is less than -0.5, we can safely assume that the Image we should show as an animation is "Towards"
        if (dotProductForward < -0.5f)
        {
            if (bPlayerMoves)
            {
                myAnimState = STATE_WALK_TOWARDS;
            }
            else
            {
                myAnimState = STATE_IDLE_TOWARDS;
            }
        }
        //Similarly, if our Dot Product of the two facing vectors is bigger than 0.5, the player is facing away from the camera
        else if (dotProductForward > 0.5f)
        {
            if (bPlayerMoves)
            {
                myAnimState = STATE_WALK_AWAY;
            }
            else
            {
                myAnimState = STATE_IDLE_AWAY;
            }
        }

        //Now, in between these two, we cannot know if the player is facing left or right, 
        //because the projection of the dot product will be the same for both cases.
        //that is why we did our second dot product, and that one can tell us if we are facing left or right.
        else if (dotProductRight > 0.0f)
        {
            if (bPlayerMoves)
            {
                myAnimState = STATE_WALK_RIGHT;
            }
            else
            {
                myAnimState = STATE_IDLE_RIGHT;
            }
        }
        else
        {
            if (bPlayerMoves)
            {
                myAnimState = STATE_WALK_LEFT;
            }
            else
            {
                myAnimState = STATE_IDLE_LEFT;
            }
        }

        //and that's it!
	}
	


	
       	
	//--------------------------------------
	// Display the right part of our animation according to our animation state
    // The decision which state we are in is taken in the function SetAnimationForCharacter()
	//--------------------------------------
	void AnimateState(){

        //We use a local variable to set up our flipping, depending on the 
        float flipXY = 1.0f;
        //if (bFlippedAnim) flipXY = -1.0f;

        //make sure our texture is not flipped before we start changing anything
        mr.material.mainTextureScale = new Vector2(flipXY / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);


        int frame = ((int)animFrame/(int)animSpeed)%3;

        switch (myAnimState)
        {

            case STATE_IDLE_TOWARDS:
                mr.material.mainTextureOffset = idleAnim[2];
                break;

            case STATE_IDLE_AWAY:
                mr.material.mainTextureOffset = idleAnim[1];
                break;

            case STATE_IDLE_LEFT:
                mr.material.mainTextureOffset = idleAnim[0];
                break;

            case STATE_IDLE_RIGHT:
                //we don't need to have extra information for standing facing left or right. 
                //We can simply flip the texture by setting its Texture Scale in x to -flipXY
                // This will basically change the sign of flipXY independently to what it is currently set.
                // so -1 will become 1
                // and 1 will become -1
                // But Beware!our offset are ...
                //...
                //offset! 
                //(haha)
                //this means, we must add one offset step whenever we flip our texture
                mr.material.mainTextureScale = new Vector2(-flipXY / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);
                mr.material.mainTextureOffset = idleAnim[0] + new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 0.0f);
                break;

            case STATE_WALK_AWAY:
                mr.material.mainTextureOffset = walkAway[frame];
                break;

            case STATE_WALK_TOWARDS:
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
                mr.material.mainTextureOffset = walkLeftRight[frame] + new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 0.0f);
                break;
        }
        
    }


}
