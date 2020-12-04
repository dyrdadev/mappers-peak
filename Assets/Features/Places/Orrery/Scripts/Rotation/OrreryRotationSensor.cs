using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class OrreryRotationSensor : OrreryRotationVelocitySynchronizer
{
    [SerializeField] private Transform observedTransform;
    [SerializeField] private Vector3 eulerWeights;
    private Vector3 latestEuler;

    private void Start()
    {
        latestEuler = observedTransform.localEulerAngles;
    }
    
    private void FixedUpdate()
    {
        // Determine delta rotation since last fixed update.
        // Hint: Quaternions are good for rotations but not so good for rotation velocity -> we convert them to vector3s
        var currentRotationVelocity = (observedTransform.localEulerAngles - latestEuler);

        // Apply weights.
        currentRotationVelocity.x *= eulerWeights.x;
        currentRotationVelocity.y *= eulerWeights.y;
        currentRotationVelocity.z *= eulerWeights.z;

        // Correct angles
        if (currentRotationVelocity.x > 180) currentRotationVelocity.x -= 360;
        if (currentRotationVelocity.x < -180) currentRotationVelocity.x += 360;
        if (currentRotationVelocity.y > 180) currentRotationVelocity.y -= 360;
        if (currentRotationVelocity.y < -180) currentRotationVelocity.y += 360;
        if (currentRotationVelocity.z > 180) currentRotationVelocity.z -= 360;
        if (currentRotationVelocity.z < -180) currentRotationVelocity.z += 360;

        // Convert to /second.
        currentRotationVelocity *= (1 / Time.fixedDeltaTime);
        
        this.UpdateValue(currentRotationVelocity);

        // Update latest rotation for next iteration.
        latestEuler = observedTransform.localEulerAngles;
    }
}
