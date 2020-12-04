using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindWheel : MonoBehaviour
{
    public Vector3 rotationAxis;
    public float referenceSpeed; // at wind strength 1

    public WindManager windManager;
 
    void Update()
    {
        this.transform.Rotate(rotationAxis, windManager.WindStrength * referenceSpeed * Time.deltaTime, Space.Self);
    }
}
