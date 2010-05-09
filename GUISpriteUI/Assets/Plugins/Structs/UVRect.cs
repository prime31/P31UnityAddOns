using UnityEngine;


public struct UVRect
{
	public Vector2 lowerLeftUV;
	public Vector2 uvDimensions;
	

	// Convenience function to return a UVRect of all zeros
	public static UVRect zero
	{
		get { return new UVRect(); }
	}

	
	// Automatically converts coordinates to UV space as specified by textureSize
	public UVRect( int x, int y, int width, int height, Vector2 textureSize )
	{
		lowerLeftUV = new Vector2( x / textureSize.x, 1.0f - ( ( y + height ) / textureSize.y ) );
		uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
	}


#region Operator overloads
	
	public static bool operator ==( UVRect lhs, UVRect rhs )
	{
		return ( lhs.lowerLeftUV == rhs.lowerLeftUV && lhs.uvDimensions == rhs.uvDimensions );
	}


	public static bool operator !=( UVRect lhs, UVRect rhs )
	{
		return ( lhs.lowerLeftUV != rhs.lowerLeftUV || lhs.uvDimensions != rhs.uvDimensions );
	}


	public override bool Equals( object obj )
	{
		if( ( obj is UVRect ) && this == (UVRect)obj )
			return true;
		
		return false;
	}


	public override int GetHashCode()
	{
		return lowerLeftUV.GetHashCode() ^ uvDimensions.GetHashCode();
	}

#endregion;

}
