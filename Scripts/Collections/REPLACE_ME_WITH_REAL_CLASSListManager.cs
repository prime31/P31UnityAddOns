using System;


// Usage:  Copy the contents of this file to a new file then do the following:
//         Run a find/replace with 'REPLACE_ME_WITH_REAL_CLASS' with the classname you want this listManager to manage
//         Access the items the listManager manages with listManager.items ivar
public class REPLACE_ME_WITH_REAL_CLASSListManager
{
	public REPLACE_ME_WITH_REAL_CLASS[] items;


	public REPLACE_ME_WITH_REAL_CLASSListManager()
	{
		items = new REPLACE_ME_WITH_REAL_CLASS[0];
	}
	
	
	public void add( REPLACE_ME_WITH_REAL_CLASS obj )
	{
		int nextIndex = items.Length;
		
		// Expand the array by one to add the item
		REPLACE_ME_WITH_REAL_CLASS[] tempItems = items;
		items = new REPLACE_ME_WITH_REAL_CLASS[items.Length + 1];
		tempItems.CopyTo( items, 0 );

		// Set the item
		items[nextIndex] = obj;
	}


	public void remove( REPLACE_ME_WITH_REAL_CLASS obj )
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
		
		items = newItems as REPLACE_ME_WITH_REAL_CLASS[];
	}


	// Much faster than calling Array.IndexOf
	public int indexOf( REPLACE_ME_WITH_REAL_CLASS obj )
	{
		for( int i = 0, totalItems = items.Length; i < totalItems; i++ )
		{
			if( obj == items[i] )
				return i;
		}
		return -1;
	}

}
