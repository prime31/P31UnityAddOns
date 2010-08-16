using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class SetCenterOfMass : MonoBehaviour
{
    public bool overrideWithBelow = true;
    public Vector3 centerOfMassOverride = Vector3.zero;

    void Start()
    {
        if(overrideWithBelow)
            rigidbody.centerOfMass = centerOfMassOverride;
    }
}
