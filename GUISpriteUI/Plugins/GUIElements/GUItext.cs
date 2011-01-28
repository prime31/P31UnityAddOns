using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GUIFonts
{	
	public int charID;
	public int posX;
	public int posY;
	public int w;
	public int h;
	public int offsetx;
	public int offsety;
	public int xadvance;
}


public class GUItext : MonoBehaviour  {

 	public GUIFonts[] arrayFonts;
	public List<GUISprite[]> textsprites = new List<GUISprite[]>();
	List<int> stringLength = new List<int>();
	
	//init the array first
	//we hold here 256 chars
	void Start()
	{
		arrayFonts = new GUIFonts[256];
		for (int i = 0; i< arrayFonts.Length; i++)
		{
			arrayFonts[i] = new GUIFonts();
		}
	}
	
	
	//parse the .fnt file with the font definition
	public void LoadConfigfile(string fileName)
	{
		string localizedStringsFile = Application.dataPath;
		localizedStringsFile = localizedStringsFile.Substring(0, localizedStringsFile.Length ) + "/StreamingAssets/" + fileName + ".fnt"; 

		// create reader & open file
		StreamReader sr = new StreamReader(new FileStream(localizedStringsFile, FileMode.Open, FileAccess.Read));
		string input = null;
	
		int idNum = 0;
		
		while ((input = sr.ReadLine()) != null)
		{
			//first split line into "space" chars
       		string[] words = input.Split(' ');
			foreach (string word in words)
        	{
				//then split line into "=" sign to get the values for each component
				string[] wordsSplit = word.Split('=');
				foreach (string word1 in wordsSplit)
       	 		{
					if (string.Equals(word1, "id"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						idNum = System.Int32.Parse(tmp);
						arrayFonts[idNum].charID = new int();
						arrayFonts[idNum].charID = idNum;
					}
					if (string.Equals(word1, "x"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].posX = new int();
						arrayFonts[idNum].posX = System.Int32.Parse(tmp);
					}
					if (string.Equals(word1, "y"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].posY = new int();
						arrayFonts[idNum].posY = System.Int32.Parse(tmp);
					}
					if (string.Equals(word1, "width"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].w = new int();
						arrayFonts[idNum].w = System.Int32.Parse(tmp);
					}
					if (string.Equals(word1, "height"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].h = new int();
						arrayFonts[idNum].h = System.Int32.Parse(tmp);
					}
					if (string.Equals(word1, "xoffset"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].offsetx = new int();
						arrayFonts[idNum].offsetx = System.Int32.Parse(tmp);
					}
					if (string.Equals(word1, "yoffset"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].offsety = new int();
						arrayFonts[idNum].offsety = System.Int32.Parse(tmp);
					}
					if (string.Equals(word1, "xadvance"))
					{
						string tmp = wordsSplit[1].Substring(0, wordsSplit[1].Length);
						arrayFonts[idNum].xadvance = new int();
						arrayFonts[idNum].xadvance = System.Int32.Parse(tmp);
					}
	
				}
			}
		}
		
		
	}
	 
	
	//draw text on screen, create each quad and send it to the guispritemanage
	public void DrawText (string text, Vector2 textposition, float textscale, int depth) 
	{		
		float dx = textposition.x;
		float dy = 0;
		float widetext;
		float offsetY;
		
		int fontlineskip = 0;
			
		int charID = 0;
		int counter = 0;
		
		GUISprite[] sprite = null;
		
		int length = text.Length;
		sprite = new GUISprite[length];
		//add the current lenght to the array
		//we use this later to delete the strings from the screen
		stringLength.Add(length);
		
		
		System.CharEnumerator charEnum = text.GetEnumerator();
	    while (charEnum.MoveNext())
	    {
			charID = System.Convert.ToInt32(text[counter]); 
			
			//"10" is the new line char
			if (charID == 10) {
			//calculate the size to center text on Y axis, based on its scale
			// 77 is the "M" char usually big enough to get a proper spaced
			//lineskip, use any other char if you want
			fontlineskip += (int)(arrayFonts[77].h * textscale);
			dx =  textposition.x;
			}
			else {
				//calculate the size to center text on Y axis, based on its scale
				offsetY = arrayFonts[charID].offsety * textscale;
				dy =  textposition.y + offsetY + fontlineskip;
			}

			//add quads for each char
			sprite[counter] = new GUISprite( new Rect( dx, dy, arrayFonts[charID].w * textscale, arrayFonts[charID].h * textscale), depth, new UVRect( arrayFonts[charID].posX,arrayFonts[charID].posY, arrayFonts[charID].w, arrayFonts[charID].h, GUISpriteUI.instance.textureSize ), false );
			GUISpriteUI.instance.addSprite( sprite[counter] );
	
			//calculate the size to advance, based on its scale
			widetext = (arrayFonts[charID].xadvance * textscale);
		
			//advance the position to draw the next letter
			dx +=  widetext + arrayFonts[charID].offsetx;

			counter++;	
		}
		
		//add all sprites at once to the array, we use this later to delete the strings
		textsprites.Add ( sprite);
		

	}
	
	public void DeleteText(int indextext)
	{
		int length = stringLength[indextext];

		GUISprite[] sprite = null;
		sprite = new GUISprite[length];
		sprite = textsprites[indextext];
		
		for (int i = 0; i < length; i++)
		{
			GUISpriteUI.instance.removeElement( sprite[i]);
		}

	}
	
	//empty all the arrays
	public void RemoveAllText()
	{
		textsprites.Clear();
		stringLength.Clear();
	}

}




//usage 
//vec1 = new Vector2( 100 , 230 );
//GUItext mtext = new GUItext();
//mtext.LoadConfigfile("testfont");
//mtext.DrawText("hello", vec1, 2, textureSize);