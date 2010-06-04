// /////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Transform Utilities.
//
// This window contains four useful tools for asset placing and manipulation: Align, Copy, Randomize and Add noise.
//
// Put this into Assets/Editor and once compiled by Unity you find
// the new functionality in Window -> TransformUtilities, or simply press Ctrl+t (Cmd+t for Mac users)
// 
// Developed by Daniel Rodríguez (Seth Illgard) in January 2010
// http://www.silentkraken.com
// e-mail: seth@silentkraken.com
//
// /////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

public class TransformUtilitiesWindow : EditorWindow 
{
    //Window control values
	public int toolbarOption = 0;
	public string[] toolbarTexts = {"Align", "Copy", "Randomize", "Add noise"};

    private bool xCheckbox = true;
    private bool yCheckbox = true;
    private bool zCheckbox = true;

    private Transform source;
    private float randomRangeMin = 0f;
    private float randomRangeMax = 1f;
    private int alignSelectionOption = 0;
    private int alignSourceOption = 0;
    
    /// <summary>
    /// Window drawing operations
    /// </summary>
    void OnGUI () 
	{
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts);
        switch (toolbarOption)
        {
            case 0:
                CreateAxisCheckboxes("Align");
                CreateAlignTransformWindow();
                break;
            case 1:
                CreateAxisCheckboxes("Copy");
                CreateCopyTransformWindow();
                break;
            case 2:
                CreateAxisCheckboxes("Randomize");
                CreateRandomizeTransformWindow();
                break;
            case 3:
                CreateAxisCheckboxes("Add noise");
                CreateAddNoiseToTransformWindow();
                break;
        }
    }

    /// <summary>
    /// Draws the 3 axis checkboxes (x y z)
    /// </summary>
    /// <param name="operationName"></param>
    private void CreateAxisCheckboxes(string operationName)
    {
        GUILayout.Label(operationName + " on axis", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
            xCheckbox = GUILayout.Toggle(xCheckbox, "X");
            yCheckbox = GUILayout.Toggle(yCheckbox, "Y");
            zCheckbox = GUILayout.Toggle(zCheckbox, "Z");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    /// <summary>
    /// Draws the range min and max fields
    /// </summary>
    private void CreateRangeFields()
    {
        GUILayout.Label("Range", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        randomRangeMin = EditorGUILayout.FloatField("Min:", randomRangeMin);
        randomRangeMax = EditorGUILayout.FloatField("Max:", randomRangeMax);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Creates the Align transform window
    /// </summary>
    private void CreateAlignTransformWindow()
    {
        //Source transform
        GUILayout.BeginHorizontal();
        GUILayout.Label("Align to: \t");
        source = EditorGUILayout.ObjectField(source, typeof(Transform)) as Transform;
        GUILayout.EndHorizontal();

        string[] texts = new string[4] { "Min", "Max", "Center", "Pivot" };

        //Display align options
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Selection:", EditorStyles.boldLabel);
        alignSelectionOption = GUILayout.SelectionGrid(alignSelectionOption, texts, 1);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Source:", EditorStyles.boldLabel);
        alignSourceOption = GUILayout.SelectionGrid(alignSourceOption, texts, 1);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Position
        if (GUILayout.Button("Align"))
        {
            if (source != null)
            {
                //Add a temporary box collider to the source if it doesn't have one
                Collider sourceCollider = source.collider;
                bool destroySourceCollider = false;
                if (sourceCollider == null)
                {
                    sourceCollider = source.gameObject.AddComponent<BoxCollider>();
                    destroySourceCollider = true;
                }

                foreach (Transform t in Selection.transforms)
                {
                    //Add a temporary box collider to the transform if it doesn't have one
                    Collider transformCollider = t.collider;
                    bool destroyTransformCollider = false;
                    if (transformCollider == null)
                    {
                        transformCollider = t.gameObject.AddComponent<BoxCollider>();
                        destroyTransformCollider = true;
                    }

                    Vector3 sourceAlignData = new Vector3();
                    Vector3 transformAlignData = new Vector3();

                    //Transform
                    switch (alignSelectionOption)
                    {
                        case 0: //Min
                            transformAlignData = transformCollider.bounds.min;
                            break;
                        case 1: //Max
                            transformAlignData = transformCollider.bounds.max;
                            break;
                        case 2: //Center
                            transformAlignData = transformCollider.bounds.center;
                            break;
                        case 3: //Pivot
                            transformAlignData = transformCollider.transform.position;
                            break;
                    }

                    //Source
                    switch (alignSourceOption)
                    {
                        case 0: //Min
                            sourceAlignData = sourceCollider.bounds.min;
                            break;
                        case 1: //Max
                            sourceAlignData = sourceCollider.bounds.max;
                            break;
                        case 2: //Center
                            sourceAlignData = sourceCollider.bounds.center;
                            break;
                        case 3: //Pivot
                            sourceAlignData = sourceCollider.transform.position;
                            break;
                    }

                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? sourceAlignData.x - (transformAlignData.x - t.position.x) : t.position.x;
                    tmp.y = yCheckbox ? sourceAlignData.y - (transformAlignData.y - t.position.y) : t.position.y;
                    tmp.z = zCheckbox ? sourceAlignData.z - (transformAlignData.z - t.position.z) : t.position.z;

                    //Register the Undo
                    Undo.RegisterUndo(t, "Align " + t.gameObject.name + " to " + source.gameObject.name);
                    t.position = tmp;
                    
                    //Ugly hack!
                    //Unity needs to update the collider of the selection to it's new position
                    //(it stores in cache the collider data)
                    //We can force the update by a change in a public variable (shown in the inspector), 
                    //then a call SetDirty to update the collider (it won't work if all inspector variables are the same).
                    //But we want to restore the changed property to what it was so we do it twice.
                    transformCollider.isTrigger = !transformCollider.isTrigger;
                    EditorUtility.SetDirty(transformCollider);
                    transformCollider.isTrigger = !transformCollider.isTrigger;
                    EditorUtility.SetDirty(transformCollider);

                    //Destroy the collider we added
                    if (destroyTransformCollider)
                    {
                        DestroyImmediate(transformCollider);
                    }
                }

                //Destroy the collider we added
                if (destroySourceCollider)
                {
                    DestroyImmediate(sourceCollider);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }
    }

    /// <summary>
    /// Creates the copy transform window
    /// </summary>
    private void CreateCopyTransformWindow()
    {
        //Source transform
        GUILayout.BeginHorizontal();
            GUILayout.Label("Copy from: \t");
            source = EditorGUILayout.ObjectField(source, typeof(Transform)) as Transform;
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Position
        if (GUILayout.Button("Copy Position"))
        {
            if (source != null)
            {
                foreach (Transform t in Selection.transforms)
                {
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? source.position.x : t.position.x;
                    tmp.y = yCheckbox ? source.position.y : t.position.y;
                    tmp.z = zCheckbox ? source.position.z : t.position.z;

                    Undo.RegisterUndo(t, "Copy position");
                    t.position = tmp;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }

        //Rotation
        if (GUILayout.Button("Copy Rotation"))
        {
            if (source != null)
            {
                foreach (Transform t in Selection.transforms)
                {
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? source.rotation.eulerAngles.x : t.rotation.eulerAngles.x;
                    tmp.y = yCheckbox ? source.rotation.eulerAngles.y : t.rotation.eulerAngles.y;
                    tmp.z = zCheckbox ? source.rotation.eulerAngles.z : t.rotation.eulerAngles.z;
                    Quaternion tmp2 = t.rotation;
                    tmp2.eulerAngles = tmp;

                    Undo.RegisterUndo(t, "Copy rotation");
                    t.rotation = tmp2;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }

        //Local Scale
        if (GUILayout.Button("Copy Local Scale"))
        {
            if (source != null)
            {
                foreach (Transform t in Selection.transforms)
                {
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? source.localScale.x : t.localScale.x;
                    tmp.y = yCheckbox ? source.localScale.y : t.localScale.y;
                    tmp.z = zCheckbox ? source.localScale.z : t.localScale.z;

                    Undo.RegisterUndo(t, "Copy local scale");
                    t.localScale = tmp;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }
    }

    /// <summary>
    /// Creates the Randomize transform window
    /// </summary>
    private void CreateRandomizeTransformWindow()
    {
        CreateRangeFields();

        //Position
        if (GUILayout.Button("Randomize Position"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.z;

                Undo.RegisterUndo(t, "Randomize position");
                t.position = tmp;
            }
        }

        //Rotation
        if (GUILayout.Button("Randomize Rotation"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.z;
                Quaternion tmp2 = t.rotation;
                tmp2.eulerAngles = tmp;

                Undo.RegisterUndo(t, "Randomize rotation");
                t.rotation = tmp2;
            }
        }

        //Local Scale
        if (GUILayout.Button("Randomize Local Scale"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.z;

                Undo.RegisterUndo(t, "Randomize local scale");
                t.localScale = tmp;
            }
        }
    }

    /// <summary>
    /// Creates the Add Noise To Transform window
    /// </summary>
    private void CreateAddNoiseToTransformWindow()
    {
        CreateRangeFields();

        //Position
        if (GUILayout.Button("Add noise to Position"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;

                Undo.RegisterUndo(t, "Add noise to position");
                t.position += tmp;
            }
        }

        //Rotation
        if (GUILayout.Button("Add noise to Rotation"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ?  t.rotation.eulerAngles.x + Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.y = yCheckbox ?  t.rotation.eulerAngles.y + Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.z = zCheckbox ?  t.rotation.eulerAngles.z + Random.Range(randomRangeMin, randomRangeMax) : 0;

                Undo.RegisterUndo(t, "Add noise to rotation");
                t.rotation = Quaternion.Euler(tmp);
            }
        }

        //Local Scale
        if (GUILayout.Button("Add noise to Local Scale"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;

                Undo.RegisterUndo(t, "Add noise to local scale");
                t.localScale += tmp;
            }
        }
    }
	
	/// <summary>
	/// Retrives the TransformUtilities window or creates a new one
	/// </summary>
    [MenuItem("Window/TransformUtilities %t")]
    static void Init() 
	{
        TransformUtilitiesWindow window = (TransformUtilitiesWindow)EditorWindow.GetWindow(typeof(TransformUtilitiesWindow));
        window.Show();
    }
}