using UnityEngine;
using System.Text;
using System.Collections;


[RequireComponent( typeof( GUIText ) )]
class GUITypeWriter : MonoBehaviour
{
    public float delay = 0.03f;
    public StringBuilder currentText;
    public AudioClip sound;
    public float maxWidth = 250.0f;
    public bool skipAhead; // if set to true, all the remaining text will be written to the screen
	public bool skipAheadOnTouch = true; // if the typing text is touched it will fully complete
	public bool keepTextValigned;
	public bool deactivateOnComplete = true; // if true, the game object will deactivate itself when done writing text
	
	// Var to keep track of the number of lines we have
	private int numberOfLines;
	
    private string _text;
    public string text
    {
        get
        {
            return _text;
        }
        set
        {
            guiText.text = string.Empty;
			
			// reset our pixelOffset
			guiText.pixelOffset = Vector2.zero;
			
            // Setup the stringBuilder with a length of the string size
            currentText = new StringBuilder(value.Length);
            _text = value;
        }
    }
	
	
	public void Start()
	{
		// Position the guiText properly so that it can type and expand properly
		guiText.anchor = TextAnchor.UpperLeft;
		guiText.alignment = TextAlignment.Left;
				
		// example usage for a group of strings
		//string[] someTextPages = {"Unity 3D Student is a new way to learn game development with the Unity Game Engine. By following small 'bitesize' Modules of tutorial info, and combining them in our Challenges, you will learn all the skills you need to pickup game development and also get an understanding of how to research further info as you work.", "Poop on you fool", "Unity 3D Student is a new way to learn game development with the Unity Game Engine. By following small 'bitesize' Modules of tutorial info, and combining them in our Challenges, you will learn all the skills you need to pickup game development and also get an understanding of how to research further info as you work."};
		//StartCoroutine( typePagesOfText( someTextPages, 3.0f ) );
	}
	
	
	public void Update()
	{
		if( !skipAheadOnTouch )
			return;
		
		// check each touch to see if it is on the typing text
		foreach( Touch touch in Input.touches )
		{
			if( touch.phase == TouchPhase.Began )
			{
				if( guiText.HitTest( touch.position ) )
					skipAhead = true;
			}
		}
	}
	
	
	public IEnumerator typePagesOfText( string[] textPages, float delayBetweenPages )
	{
		// loop through all the pages
		foreach( string page in textPages )
		{
			// reset skipAhead in case there was some screen mashing going on
			skipAhead = false;
			yield return StartCoroutine( typeText( page ) );
			yield return new WaitForSeconds( delayBetweenPages );
		}
		
		if( deactivateOnComplete )
			this.enabled = false;
	}	


    private IEnumerator typeText( string text )
    {
        this.text = text;
		numberOfLines = 1;
		
        foreach( char letter in _text )
        {
			//Debug.Log( guiText.GetScreenRect().height / numberOfLines );
			
            // Width check.  If the guiText gets too wide we need to backtrack to the previous word boundary and insert a newline
            if( guiText.GetScreenRect().width > maxWidth )
            {
				// Shift the guiText position if we are set to keep the text valigned
				if( keepTextValigned )
				{
					float lineHeight = guiText.GetScreenRect().height / numberOfLines;
					Vector2 offset = guiText.pixelOffset;
					offset.y += lineHeight / 2;
					guiText.pixelOffset = offset;
				}
				
                // Look for the last newline
                int lastIndex = currentText.ToString().LastIndexOf( ' ' );
                currentText.Replace( " ", "\n", lastIndex, 1 );
				numberOfLines++;
            }

            currentText.Append( letter );
            guiText.text = currentText.ToString();
			
            if( sound && !audio.isPlaying )
                audio.PlayOneShot( sound );

            if( !skipAhead )
                yield return new WaitForSeconds( delay );
        }
		
        skipAhead = false;
    }

}