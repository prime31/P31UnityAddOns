/*
Description

Snaps objects to a grid in 3 dimensions. The grid spacing may be different for each axis.
[edit] Usage

You must place the script in a folder named Editor in your project's Assets folder for it to work properly.

Select some objects in the Scene view or Hierarchy window, then choose GameObject→Snap to Grid from the menu 
(or press control G). Each selected object will be independently snapped to a unit grid.

If you require a different grid spacing, change gridx, gridy, and gridz below.

*/

using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class SnapToGrid : ScriptableObject
{
    [MenuItem ("GameObject/Snap to Grid ^g")]
    static void MenuSnapToGrid()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
        
        float gridx = 1.0f;
        float gridy = 1.0f;
        float gridz = 1.0f;
        
        foreach (Transform transform in transforms)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Round(newPosition.x / gridx) * gridx;
            newPosition.y = Mathf.Round(newPosition.y / gridy) * gridy;
            newPosition.z = Mathf.Round(newPosition.z / gridz) * gridz;
            transform.position = newPosition;
        }
    }
}