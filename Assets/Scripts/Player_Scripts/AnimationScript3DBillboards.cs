using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq.Expressions;
using System.IO;

/// <summary>
/// In this class, we determine which small part of a larger image (also called SpriteSheet) we should draw, 
/// so that others can see a representation of us in 3D
/// </summary>

public class AnimationScript3DBillboards : MonoBehaviour
{

    //For the 3D Renderer, and to show you different ways in which you can do things,
    //instead of creating individual pictures, 
    //we just adjust our drawing coordinates 
    //of which part of our texture we want to draw 
    //these are only public so that we can look at the in the editor

    public Vector2[] idleAnim;              //idle is what we call the animation that should play if the Player doesn't move
                                            //Video Game people have come up with this term.

    public Vector2[] walkLeftRight;         //since we use the same part of the image for both left and right movement,
                                            //because we flip the Texture to get it to face the other way

    public Vector2[] walkTowards;           //things to show when the Player is facing the camera

    public Vector2[] walkAway;              //things to show when the Player is facing 180 degrees away from the camera


    public int HowManyAnimationPicsperRow = 3;  //this number relates to the amount of animation stills per row in our Sprite Sheet
    public int HowManyAnimationRows = 3;        //this number relates to the amount of rows with animation stills in our Sprite Sheet

    //animation states
    //States are a good way to keep track of information that is persistent for longer than a single frame (a single Update() call in Unity)
    //but still changes over time.
    //there is some info on states on the Wiki:http://hyperdramatik.net/mediawiki/index.php?title=Algorithms#Code_ist_Zustandsabfrage_und_Reaktion
    //You have used states before, but mostly with booleans. 
    //For more complex things (if the state is not just one or the other), 
    //it is helpful to keep track of the state with a simple number, like we do here

    public int myAnimState;             //in this variable, we want to store the current State of our Animation								

    const int STATE_IDLE_TOWARDS = 0;   //these are all variables whose value will never change (which is why they have the word "const" in front)
    const int STATE_IDLE_LEFT = 1;
	const int STATE_IDLE_RIGHT = 2;
	const int STATE_IDLE_AWAY = 3;
    const int STATE_WALK_AWAY = 4;
	const int STATE_WALK_TOWARDS = 5;
	const int STATE_WALK_LEFT = 6;
	const int STATE_WALK_RIGHT = 7;

    //we use this to make the code more readable!
    //because it is sometimes easier to read a line of code like this:
    //
    //if (myAnimState==STATE_WALK_AWAY)
    //
    //than read a line like this:
    //
    //if (myAnimState==4)
    //
    //especially after a couple of days away from the computer

    //we use int variables for this, but there is a different way to do this, that many people use
    //you can see the different way (involving Enums) in the GameData Script

    public float animFrame;             //this variable stores the time that has passed between each call to Update()
                                        //we use it when determining which part of our texture to draw, 
                                        //so that we create a sort of animated flipbook

    public float animSpeed = 10;        //we use this variable to time the switching of our pictures 


    MeshRenderer myMeshRenderer;        //we use this variable to Reference the MeshRenderer Component in our GameObject
                                        //we need that later, to access the texture on the Mesh and change which part of the texture we show


    TextMesh    infoText;               //a variable that references our TextMesh that can (for example) display our character's name
    PlayerData  myPlayer;               //a variable that references the PlayerData Object, in which we store all sorts of Player related information

    //these next two variables are used in the calculations for figuring out which way a Player is facing 
    //corresponding to the camera
    //more info can be found at SetAnimationForCharacter()
    public float visibleDotProductForward;
    public float visibleDotProductRight;

    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    void Start()
	{
        //find our PlayerData Object in the Parent GameObject
        myPlayer = GetComponentInParent<PlayerData>();

        //let the Console know that we exist!
        Debug.Log("Animation Script 3D Billboards at the ready!");

        //find the MeshRenderer Component in this script, because we want to do stuff to it later
        myMeshRenderer = this.GetComponent<MeshRenderer>();

        //find the TextMesh, so we can write things to it.
		infoText = GetComponentInChildren<TextMesh> ();

        //here we load the data from disk to be shown on our Mesh
        //we want to do this in the very beginning, before Update is called
		GenerateTextureFromFile ();

    }


    //--------------------------------------
    // Update() gets called every frame
    // so here you should find a readable list of things that we do every frame
    // :-)
    //--------------------------------------
    void Update()
    {
        //First, we need to figure out which Animation we should play
        //because things might have changed from the last frame
        SetAnimationForCharacter();

        //once we have that figured out, we can call a function that does the actual "playing"
        AnimateState();

    }




    //------------------------------------------------------------------------------------------------------------------------
    // I personally like to first read Start() and Update() so that I can get an idea of what this Script is doing
    // then i can delve deeper and figure out how individual functions work
    //
    // If I have time (and I don't always do), I try to order the function in the same order that they are called in.
    //------------------------------------------------------------------------------------------------------------------------



    //--------------------------------------
    // Load the specific Texture from a .png file in the Resources Folder
    // At some point, we can point this to a location online to download more resources, 
    // or change them without changing the code
    //--------------------------------------
    public void GenerateTextureFromFile(){

        //load Data from the Resources Folder into a Texture (see http://hyperdramatik.net/mediawiki/index.php?title=GlossarCG#Textur )
        Texture2D charImage = Resources.Load("Characters/" + myPlayer.characterName + "_character") as Texture2D;

        //Let's check if loading actually worked
        if (!charImage)
        {
            Debug.Log("Loading failed! Maybe Character does not exist?");
        }

        //this here makes sure that our images are drawn crisp and not mushy
        charImage.filterMode = FilterMode.Point;

        //set up our Arrays so we can put Data in them
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
        myMeshRenderer.material.mainTextureScale = new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);

        //finally, we will now set our Material in the Meshrenderer to use our Texture

        myMeshRenderer.material.mainTexture = charImage;

	}


    //--------------------------------------
    // Figure out which Image of the Animation Sequence to show
    // This is using Vector Maths and is a bit hard, 
    // if you haven't heard of Vector Maths before
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
        Vector3 cameraFacing = GameData.instance.mainCamera.transform.forward;
        cameraFacing.y = 0;
        cameraFacing.Normalize();

        //and in order to determine if they are facing left or right from one another, we need to do the same for the right axis of the camera
        Vector3 cameraRight = GameData.instance.mainCamera.transform.right;
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
	// Display the correct part of our Texture according to our animation state
    // The decision which state we are in is taken in the function SetAnimationForCharacter()
	//--------------------------------------
	void AnimateState(){

        //count up how many frames have passed since we last called this function
        animFrame += Time.deltaTime * 100.0f;


        //make sure our texture is not flipped before we start changing anything
        //We have to do this, because in the previous frame, we might have flipped our texture!
        //so we want to make sure we start from a non-flipped texture and then determine if we need to flip it, 
        //depending on the situation in this specific frame
        myMeshRenderer.material.mainTextureScale = new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);

        //here we calculate which of the three (or whatever HowManyAnimationPicsperRow stands for) pics of our animation we should show
        // if you want to figure out how this works, take paper and pen and write down the outcome (the content of "frame") for different values for animFrame
        int frame = ((int)animFrame/(int)animSpeed)%HowManyAnimationPicsperRow;

        //if you have to go through a lot of different "if" statements, you can also use "switch" instead, 
        //like in this case
        //This is equivalent to if (myAnimState == STATE_IDLE_TOWARDS){ ... } and so on
        switch (myAnimState)
        {

            case STATE_IDLE_TOWARDS:
                myMeshRenderer.material.mainTextureOffset = idleAnim[2];
                break;

            case STATE_IDLE_AWAY:
                myMeshRenderer.material.mainTextureOffset = idleAnim[1];
                break;

            case STATE_IDLE_LEFT:
                myMeshRenderer.material.mainTextureOffset = idleAnim[0];
                break;

            case STATE_IDLE_RIGHT:
                //we don't need to have extra information for standing facing left or right. 
                //We can simply flip the texture by setting its Texture Scale in x to -1
                // But Beware!our offset are ...
                //...
                //offset! 
                //(haha)
                //this means, we must add one offset step whenever we flip our texture
                myMeshRenderer.material.mainTextureScale = new Vector2(-1.0f / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);
                myMeshRenderer.material.mainTextureOffset = idleAnim[0] + new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 0.0f);
                break;

            case STATE_WALK_AWAY:
                myMeshRenderer.material.mainTextureOffset = walkAway[frame];
                break;

            case STATE_WALK_TOWARDS:
                myMeshRenderer.material.mainTextureOffset = walkTowards[frame];
                break;

            case STATE_WALK_LEFT:

                myMeshRenderer.material.mainTextureOffset = walkLeftRight[frame];
                break;

            case STATE_WALK_RIGHT:

                //we don't need to have extra information for walking left and right. 
                //We can simply flip the texture by setting its Texture Scale in x to -1
                //But Beware! our offset are ...
                //...
                //offset! 
                //(haha)
                //this means, we must add one offset step whenever we flip our texture
                myMeshRenderer.material.mainTextureScale = new Vector2(-1.0f / (float)HowManyAnimationPicsperRow, 1.0f / (float)HowManyAnimationRows);
                myMeshRenderer.material.mainTextureOffset = walkLeftRight[frame] + new Vector2(1.0f / (float)HowManyAnimationPicsperRow, 0.0f);
                break;
        }
        
    }


}
