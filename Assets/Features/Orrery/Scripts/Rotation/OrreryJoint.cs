using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class OrreryJoint : OrreryRotationVelocitySynchronizer
{
    [SerializeField] private float rotationFactor = 1.0f;

    private void FixedUpdate()
    {
        // Apply the value by rotating this joint.
        this.transform.Rotate(this.synchronizedValue * Time.fixedDeltaTime);
    }

    protected override Vector3 ProcessDataBeforeUpdate(Vector3 newData)
    {
        return newData * this.rotationFactor;
    }
}
