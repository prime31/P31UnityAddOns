using UnityEngine;
using System;


// Helper class that automatically resizes a standard array.  Useful for GUITouchableSprites
// because we only have the overhead of resizing during initialization
public class GUITouchableSpriteListManager
{
	public GUITouchableSprite[] items;


	public GUITouchableSpriteListManager()
	{
		items = new GUITouchableSprite[0];
	}
	
	
	public void add( GUITouchableSprite obj )
	{
		int nextIndex = items.Length;
		
		// Expand the array by one to add the item
		GUITouchableSprite[] tempItems = items;
		items = new GUITouchableSprite[items.Length + 1];
		tempItems.CopyTo( items, 0 );

		// Set the item
		items[nextIndex] = obj;
	}


	public void remove( GUITouchableSprite obj )
	{
		int index = this.indexOf( obj );
		
		if( index >= 0 )
			this.remove( index );
	}


	public void remove( int index )
	{
		Array newItems = Array.CreateInstance( items.GetType().GetElementType(), items.Length - 1 );
		Array.Copy( items, 0, newItems, 0, index );
		Array.Copy( items, index + 1, newItems, index, items.Length - index - 1 );
		
		items = newItems as GUITouchableSprite[];
	}


	// Much faster than calling Array.IndexOf
	public int indexOf( GUITouchableSprite obj )
	{
		for( int i = 0, totalItems = items.Length; i < totalItems; i++ )
		{
			if( obj == items[i] )
				return i;
		}
		return -1;
	}

}