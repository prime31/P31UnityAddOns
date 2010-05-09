using UnityEngine;
using UnityEditor;
 
public class PlaceSelectionOnSurface : ScriptableObject
{
    [MenuItem ("GameObject/Place Selection On Surface")]
    static void CreateWizard ()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep |
            SelectionMode.ExcludePrefab | SelectionMode.OnlyUserModifiable);
        
        if (transforms.Length > 0 && EditorUtility.DisplayDialog("Place Selection On Surface?",
            "Are you sure you want to place " + transforms.Length +
            ((transforms.Length > 1) ? " objects" : " object") +
            " on the surface in the -Y direction?", "Place", "Do Not Place"))
        {
            foreach (Transform transform in transforms)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    transform.position = hit.point;
                    Vector3 randomized = Random.onUnitSphere;
                    randomized = new Vector3(randomized.x, 0F, randomized.z);
                    transform.rotation = Quaternion.LookRotation(randomized, hit.normal);
                }
            }
        }
    }
}