using System;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(OrreryRotationSensor))]
public class OrreryEnergySensor : OrreryEnergySynchronizer
{
    [SerializeField] private AnimationCurve energyLevelMapping;
    [SerializeField] private float energyLevelDamping = 1.0f;
    private OrreryRotationSensor rotationSensor;

    private void Start()
    {
        rotationSensor = this.GetComponent<OrreryRotationSensor>();
    }

    private void FixedUpdate()
    {
        // Get the clamped energy level for the current rotaion velocity and the mapping curve.
        var currentEnergyLevel = Mathf.Clamp(energyLevelMapping.Evaluate(rotationSensor.GetValue().y), 0, 1);
        currentEnergyLevel = Mathf.Lerp(this.synchronizedValue, currentEnergyLevel, energyLevelDamping * Time.fixedDeltaTime);

        this.UpdateValue(currentEnergyLevel);
    }
}
