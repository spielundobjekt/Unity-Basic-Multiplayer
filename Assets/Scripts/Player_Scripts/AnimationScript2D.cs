using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq.Expressions;
using System.IO;

/// <summary>
///In this class, we load an image (also called SpriteSheet) and divide it into multiple separate smaller images,
///that we can draw depending on how we move
///so we ourselces and others can see a representation of us in 2D
///
// We presume this script is a Child of the GameObject that holds the PlayerData Script
/// </summary>

public class AnimationScript2D : MonoBehaviour
{
	
    //We extract the Sprites from a texture into arrays, and then display each of them for a couple of frames 
    //we also switch the arrays we use depending on the way we are walking
	public Sprite[] idleAnim;           //idle is what we call the animation that should play if the Player doesn't move
                                        //Video Game people have come up with this term.

    public Sprite[] walkLeftRightAnim;             //since we use the same part of the image for both left and right movement,
                                        //because we flip the Sprite to get it to face the other way
    public Sprite[] walkDownAnim;
	public Sprite[] walkUpAnim;

    public int HowManyAnimationPicsperRow = 3;  //this number relates to the amount of animation stills per row in our Sprite Sheet
    public int HowManyAnimationRows = 3;        //this number relates to the amount of rows with animation stills in our Sprite Sheet


    //animation states
    //States are a good way to keep track of information that is persistent for longer than a single frame (a single Update() call in Unity)
    //but still changes over time.
    //there is some info on states on the Wiki:http://hyperdramatik.net/mediawiki/index.php?title=Algorithms#Code_ist_Zustandsabfrage_und_Reaktion
    //You have used states before, but mostly with booleans. 
    //For more complex things (if the state is not just one or the other), 
    //it is helpful to keep track of the state with a simple number, like we do hereconst int STATE_IDLE = 0;

    public int myAnimState;                     //in this variable, we want to store the current State of our Animation

    const int STATE_IDLE = 0;                   //these are all variables whose value will never change (which is why they have the word "const" in front)
    const int STATE_WALK_UP = 1;
    const int STATE_WALK_DOWN = 2;
	const int STATE_WALK_LEFT = 3;
	const int STATE_WALK_RIGHT = 4;

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

    SpriteRenderer mySpriteRenderer;    //we use this variable to Reference the SpriteRenderer Component in our GameObject
                                        //we need that later, to access the contents of the Sprite we want to showshow

    TextMesh infoText;                  //a variable that references our TextMesh that can (for example) display our character's name
    PlayerData myPlayer;               //a variable that references the PlayerData Object, in which we store all sorts of Player related information


    //--------------------------------------
    // We use Start() to find the references for a lot of our Variables
    // If we do it this way, we don't have to rely on connecting things in the editor that much.
    //--------------------------------------
    void Start()
	{
        //find our PlayerData Object in the Parent GameObject
        myPlayer = GetComponentInParent<PlayerData>();

        //let the Console know that we exist!
        Debug.Log("AnimationScript 2D at the ready!");
        
        //find the SpriteRenderer Component in this script, because we want to do stuff to it later
        mySpriteRenderer = this.GetComponent<SpriteRenderer>();

        //find the TextMesh, so we can write things to it.
        infoText = GetComponentInChildren<TextMesh> ();

        //here we load the data from disk to be shown with our SpriteRenderer
        //we want to do this in the very beginning, before Update is called
        GenerateSpritesFromFile();

        //In order to show our Sprites in 2D view, we must make sure our SpriteRenderer is Facing upwards
        transform.rotation = Quaternion.Euler(new Vector3(90,0,0));

    }

    //--------------------------------------
    // Update() gets called every frame
    // so here you should find a readable list of things that we do every frame
    // :-)
    //--------------------------------------
    void Update(){
        

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
    public void GenerateSpritesFromFile()
    {
        //Let's check if we have a name set up in the UI for our Player
        //also: you should always compare two strings with the .Equals() function
        // and not use ==
        //That's what people tell me. It's safer with .Equals
        if (!CharacterUISetupBridge.localCharacterName.Equals("none"))
        {
            myPlayer.characterName = CharacterUISetupBridge.localCharacterName;
        }

        //load Data from the Resources Folder into a Texture (see http://hyperdramatik.net/mediawiki/index.php?title=GlossarCG#Textur )
        Texture2D charImage = Resources.Load("Characters/" + myPlayer.characterName + "_character") as Texture2D;

        //Let's check if loading actually worked
        if (!charImage)
        {
            Debug.Log("Loading failed! Maybe Character does not exist?");
        }

        //this here makes sure that our images are drawn crisp and not mushy
        charImage.filterMode = FilterMode.Point;

        //Next, we prepare our Arrays so that they can receive Sprites

        //if we don't walk, we will always be showing the same sprite, no matter how we are facing,
        //thus, our idle animation only needs one slot
        idleAnim = new Sprite[1];

        walkLeftRightAnim = new Sprite[HowManyAnimationPicsperRow];
        walkDownAnim = new Sprite[HowManyAnimationPicsperRow];
        walkUpAnim = new Sprite[HowManyAnimationPicsperRow];

        //now we "cut up" our texture and create little smaller images from it, and store them as sprites in our arrays
        //the numbers in the Sprite.Create() function correspond to:
        //the actual amount of pixels we need to cut, the mid-point of our new image, and how many pixels should be displayed per Unity unit
        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
            walkDownAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 144, 48, -48), new Vector2(0.4f, 0.8f), 16.0f);

        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
            walkUpAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 96, 48, -48), new Vector2(0.4f, 0.8f), 16.0f);

        for (int i = 0; i < HowManyAnimationPicsperRow; i++)
            walkLeftRightAnim[i] = Sprite.Create(charImage, new Rect(i * 48, 48, 48, -48), new Vector2(0.4f, 0.8f), 16.0f);

        //we set our idle anim Sprite to the same one that is the middle-walkDownAnim one
        idleAnim[0] = walkDownAnim[1];

    }


    //--------------------------------------
    // Figure out which Image of the Animation Sequence to show
     //--------------------------------------
    public void SetAnimationForCharacter(){

        //make sure our animation state at the beginning of the frame is IDLE - we will change our state if there is a need for that
		myAnimState = STATE_IDLE;

        //transfer our movement Vector from the variable "movementDirection" from the PlayerData Script into a local variable
        //the value of this variable was set by a different script - in our case, the Move2D script
        Vector3 locDelta = myPlayer.movementDirection;

       
		//Next, we check if we are moving, by looking at the magnitude of the movement Vector (it's length)
        //we look at the Square of the magnitude, because every number squared is positive
        //that way we do not have to check if the magnitude is > 0 or < 0
		if (locDelta.sqrMagnitude>0.0f){

            //Next, check if we are moving more left-right-ish or more up-down-ish?
            //we do this by comparing the absolute (which means "always positive") value of our movement's x- and z-parts
            //we do things here if the value of the x part of the vector was higher
			if (Math.Abs (locDelta.x) >= Math.Abs (locDelta.z)) {
				if (locDelta.x > 0.0f) {
					myAnimState = STATE_WALK_RIGHT;
				}
				if (locDelta.x < 0.0) {
					myAnimState = STATE_WALK_LEFT;
				}
			}
            //and here we do things if the value of the z-part was higher
            else
            {
				if (locDelta.z < 0.0) {
					myAnimState = STATE_WALK_DOWN;
                }
				if (locDelta.z > 0.0){
					myAnimState = STATE_WALK_UP;
                }
			}
		}

        //That's it. We have now figured out which state our animation is in, according to the way the Player has moved.
        
	}





    //--------------------------------------
    // Display the correct Sprite according to our animation state
    // The decision which state we are in was taken in the function SetAnimationForCharacter()
    //--------------------------------------
    void AnimateState(){

        //count up how many frames have passed since we last called this function
        animFrame += Time.deltaTime * 100.0f;

        //here we calculate which of the three (or whatever HowManyAnimationPicsperRow stands for) pics of our animation we should show
        // if you want to figure out how this works, take paper and pen and write down the outcome (the content of "frame") for different values for animFrame
        int frame = ((int)animFrame/(int)animSpeed)%3;

        //if you have to go through a lot of different "if" statements, you can also use "switch" instead, 
        //like in this case
        //This is equivalent to if (myAnimState == STATE_IDLE){ ... } and so on
        switch (myAnimState) {
			
		case STATE_IDLE:
			mySpriteRenderer.sprite=idleAnim[0];
			break;

		case STATE_WALK_UP:
			mySpriteRenderer.sprite=walkUpAnim[frame];
			break;
			
		case STATE_WALK_DOWN:
			mySpriteRenderer.sprite=walkDownAnim[frame];
			break;

        case STATE_WALK_RIGHT:
            //we don't need to have extra information for walking left and right. 
            //We can simply flip the Sprite by setting its flipX variable to "true"
            mySpriteRenderer.sprite = walkLeftRightAnim[frame];
            mySpriteRenderer.flipX = true;
            break;


        case STATE_WALK_LEFT:
            //Because we do not know, if we moved Right just a frame ago, 
            //we always have to make sure that, when walking left, the sprite is never flipped.
            mySpriteRenderer.sprite = walkLeftRightAnim [frame];
		    mySpriteRenderer.flipX = false;				
			break;

		}

	}


}
