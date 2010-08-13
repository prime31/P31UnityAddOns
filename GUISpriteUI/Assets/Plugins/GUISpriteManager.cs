//-----------------------------------------------------------------
// Holds a single mesh object which is composed of an arbitrary
// number of quads that all use the same material, allowing
// multiple, independently moving objects to be drawn on-screen
// while using only a single draw call.
//-----------------------------------------------------------------

using System;
using UnityEngine;


public class GUISpriteManager : MonoBehaviour 
{

	#region ivars
	
	// Which way to wind polygons?
	public enum WINDING_ORDER
	{
	    CCW,        // Counter-clockwise
	    CW      		// Clockwise
	};
	
	public Material material;            // The material to use for the sprites
	public int maxSpriteCount = 10;        // How many sprites to allocate space for
	public WINDING_ORDER winding = WINDING_ORDER.CCW; // Which way to wind polygons
	public bool autoUpdateBounds = false;   // Automatically recalculate the bounds of the mesh when vertices change?
	
	protected bool meshIsDirty = false; // Flag that gets set if any of the following flags are set.  No updates will happen unless this is true
	protected bool vertsChanged = false;    // Have changes been made to the vertices of the mesh since the last frame?
	protected bool uvsChanged = false;    // Have changes been made to the UVs of the mesh since the last frame?
	protected bool colorsChanged = false;   // Have the colors changed?
	protected bool vertCountChanged = false;// Has the number of vertices changed?
	protected bool updateBounds = false;    // Update the mesh bounds?
	
	protected GUISprite[] sprites;    // Array of all sprites (the offset of the vertices corresponding to each sprite should be found simply by taking the sprite's index * 4 (4 verts per sprite).
	
	protected MeshFilter meshFilter;
	protected MeshRenderer meshRenderer;
	protected Mesh mesh;                    // Reference to our mesh (contained in the MeshFilter)
	public Vector2 textureSize = Vector2.zero;
	
	protected Vector3[] vertices;  // The vertices of our mesh
	protected int[] triIndices;    // Indices into the vertex array
	protected Vector2[] UVs;       // UV coordinates
	protected Color[] colors;      // Color values

	#endregion;
	

	#region Unity MonoBehaviour Functions
	
    virtual protected void Awake()
    {
        gameObject.AddComponent( "MeshFilter" );
        gameObject.AddComponent( "MeshRenderer" );

        meshFilter = (MeshFilter)GetComponent( typeof( MeshFilter ) );
        meshRenderer = (MeshRenderer)GetComponent( typeof( MeshRenderer ) );

        meshRenderer.renderer.material = material;
        mesh = meshFilter.mesh;

        // Create our vert, UV, color and sprite arrays
		createArrays( maxSpriteCount );

        // Move the object to the origin so the objects drawn will not be offset from the objects they are intended to represent.
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
		
		// Cache our texture size
		Texture t = material.GetTexture( "_MainTex" );
		textureSize = new Vector2( t.width, t.height );
    }

	
	/* This gets called in the GUISpriteUI Update() method for consolidation
	 * If you are not using the GUISpriteUI subclass, uncomment this method
	void LateUpdate()
	{
		if( meshIsDirty )
		{
			meshIsDirty = false;
			updateMeshProperties();
		}
	}
	*/
	
	
	// Performs any changes if the verts, UVs, colors or bounds changed
	protected void updateMeshProperties()
	{
        // Were changes made to the mesh since last time?
        if( vertCountChanged )
        {
            vertCountChanged = false;
            colorsChanged = false;
            vertsChanged = false;
            uvsChanged = false;
            updateBounds = false;

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = UVs;
            mesh.colors = colors;
            mesh.triangles = triIndices;
        }
        else
        {
            if( vertsChanged )
            {
                vertsChanged = false;

                if( autoUpdateBounds )
                    updateBounds = true;

                mesh.vertices = vertices;
            }

            if( updateBounds )
            {
                mesh.RecalculateBounds();
                updateBounds = false;
            }

            if( colorsChanged )
            {
                colorsChanged = false;
                mesh.colors = colors;
            }

            if( uvsChanged )
            {
                uvsChanged = false;
                mesh.uv = UVs;
            }
        } // end else
	}
	
	#endregion;
	

	// Initializes all required arrays
    protected void createArrays( int count )
    {
        // Create the sprite array
        sprites = new GUISprite[count];

        // Vertices:
        vertices = new Vector3[count * 4];
        
        // UVs:
        UVs = new Vector2[count * 4];

        // Colors:
        colors = new Color[count * 4];

        // Triangle indices:
        triIndices = new int[count * 6];

        // Inform existing sprites of the new vertex and UV buffers:
        //for( int i = 0; i < firstNewElement; ++i )
        //    sprites[i].setBuffers( vertices, UVs );

        // Setup the newly-added sprites and Add them to the list of available 
        // sprite blocks. Also initialize the triangle indices while we're at it:
        for( int i = 0; i < sprites.Length; ++i )
        {
            // Init triangle indices:
            if( winding == WINDING_ORDER.CCW )
            {   // Counter-clockwise winding
                triIndices[i * 6 + 0] = i * 4 + 0;  //    0_ 2            0 ___ 3
                triIndices[i * 6 + 1] = i * 4 + 1;  //  | /      Verts:  |   /|
                triIndices[i * 6 + 2] = i * 4 + 3;  // 1|/                1|/__|2

                triIndices[i * 6 + 3] = i * 4 + 3;  //      3
                triIndices[i * 6 + 4] = i * 4 + 1;  //   /|
                triIndices[i * 6 + 5] = i * 4 + 2;  // 4/_|5
            }
            else
            {   // Clockwise winding
                triIndices[i * 6 + 0] = i * 4 + 0;  //    0_ 1            0 ___ 3
                triIndices[i * 6 + 1] = i * 4 + 3;  //  | /      Verts:  |   /|
                triIndices[i * 6 + 2] = i * 4 + 1;  // 2|/                1|/__|2

                triIndices[i * 6 + 3] = i * 4 + 3;  //      3
                triIndices[i * 6 + 4] = i * 4 + 2;  //   /|
                triIndices[i * 6 + 5] = i * 4 + 1;  // 5/_|4
            }
        }

        vertsChanged = true;
        uvsChanged = true;
        colorsChanged = true;
        vertCountChanged = true;
		meshIsDirty = true;
    }

	#region Add/Remove sprite functions
	
	// Shortcut for adding a new sprite
    public GUISprite addSprite( Rect frame, UVRect uvFrame, int depth )
    {
        // Create and initialize the new sprite
		GUISprite newSprite = new GUISprite( frame, depth, uvFrame );
		addSprite( newSprite );
		
		return newSprite;
    }


    // Adds a sprite to the manager
    public void addSprite( GUISprite sprite )
    {
        // Initialize the new sprite and update the UVs		
		int i = 0;
	
		// Find the first available sprite index
		for( ; i < sprites.Length; i++ )
		{
			if( sprites[i] == null )
				break;
		}
		
        // Assign and setup the sprite
		sprites[i] = sprite;
		
        sprite.index = i;
        sprite.manager = this;

        sprite.setBuffers( vertices, UVs );

		// Setup indices of the sprites vertices, UV entries and color values
		sprite.vertexIndices.initializeVertsWithIndex( i );
		sprite.initializeSize();
		
		sprite.color = Color.white;
		
        // Set our flags:
        vertsChanged = true;
        uvsChanged = true;
		meshIsDirty = true;
    }


    public void removeSprite( GUISprite sprite )
    {
		vertices[sprite.vertexIndices.mv.one] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.two] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.three] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.four] = Vector3.zero;

        sprites[sprite.index] = null;
		
		// This should happen when the sprite dies!!
		//Destroy( sprite.client );
		
        vertsChanged = true;
		meshIsDirty = true;
    }

	#endregion;
	
	
	#region Show/Hide sprite functions

    public void hideSprite( GUISprite sprite )
    {
        sprite.m_hidden___DoNotAccessExternally = true;

		vertices[sprite.vertexIndices.mv.one] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.two] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.three] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.four] = Vector3.zero;

        vertsChanged = true;
		meshIsDirty = true;
    }

	
    public void showSprite( GUISprite sprite )
    {
        if( !sprite.m_hidden___DoNotAccessExternally )
            return;

        sprite.m_hidden___DoNotAccessExternally = false;

        // Update the vertices.  This will end up caling UpdatePositions() to set the vertsChanged flag
        sprite.transform();
    }

	#endregion;
	

	#region Update UV, colors and positions
	
    // Updates the UVs of the specified sprite and copies the new values into the mesh object.
    public void updateUV( GUISprite sprite )
    {
		UVs[sprite.vertexIndices.uv.one] = sprite.uvFrame.lowerLeftUV + Vector2.up * sprite.uvFrame.uvDimensions.y;  // Upper-left
		UVs[sprite.vertexIndices.uv.two] = sprite.uvFrame.lowerLeftUV;                              // Lower-left
		UVs[sprite.vertexIndices.uv.three] = sprite.uvFrame.lowerLeftUV + Vector2.right * sprite.uvFrame.uvDimensions.x;// Lower-right
		UVs[sprite.vertexIndices.uv.four] = sprite.uvFrame.lowerLeftUV + sprite.uvFrame.uvDimensions;     // Upper-right
        
        uvsChanged = true;
		meshIsDirty = true;
    }
	

    // Updates the color values of the specified sprite and copies the new values into the mesh object.
    public void updateColors( GUISprite sprite )
    {
		colors[sprite.vertexIndices.cv.one] = sprite.color;
		colors[sprite.vertexIndices.cv.two] = sprite.color;
		colors[sprite.vertexIndices.cv.three] = sprite.color;
		colors[sprite.vertexIndices.cv.four] = sprite.color;

        colorsChanged = true;
		meshIsDirty = true;
    }

	
    // Informs the SpriteManager that some vertices have changed position and the mesh needs to be reconstructed accordingly
    public void updatePositions()
    {
        vertsChanged = true;
		meshIsDirty = true;
    }

	#endregion;

}
