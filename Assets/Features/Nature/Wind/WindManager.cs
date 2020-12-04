using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    [SerializeField] protected float windStrength;
    public float WindStrength
    {
        get { return windStrength; }
        protected set { windStrength = value; }
    }

    public void SetNormalizedWindStrength(float strength)
    {
        WindStrength = strength;
    }
}
