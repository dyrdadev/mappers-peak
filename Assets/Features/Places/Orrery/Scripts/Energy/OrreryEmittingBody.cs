using System;
using System.Transactions;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Light))]
public class OrreryEmittingBody : OrreryEnergySynchronizer
{
    [SerializeField] private float maxEmissionIntensity = 2.0f;
    [SerializeField] private float maxLightIntensity = 2.0f;
    [SerializeField] private Color color = new Color(1,1,1);
    Material currentMaterial;
    Light currentLight;

    public void Start()
    {
        base.Start();
        currentMaterial = GetComponent<Renderer>().material;
        currentLight = GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        currentMaterial.EnableKeyword("_EMISSION");
        currentMaterial.SetColor("_EmissionColor", color * maxEmissionIntensity * this.synchronizedValue);

        currentLight.intensity = maxLightIntensity * this.synchronizedValue;
    }
}
