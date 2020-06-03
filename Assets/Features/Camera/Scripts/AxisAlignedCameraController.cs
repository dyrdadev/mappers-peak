using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAlignedCameraController : MonoBehaviour {

    [Header("General Parameters")]

    // The offset to the target.
    [SerializeField]
    private Vector3 offset = new Vector3(0, 5.0f, -10.0f);
    // Damping factors.
    [SerializeField]
    private float positionThreshold = 0.1f;
    [SerializeField]
    private bool lookAtTargetPlane = true;

    [Header("Damping Parameters")]

    [SerializeField]
    private AnimationCurve dampingCurve =  AnimationCurve.EaseInOut(0.0f, 0.3f, 1.0f, 1.0f);

    [SerializeField]
    private float dampingDistance = 1.0f;

    [SerializeField]
    private float horizontalDampingFactor = 14.0f;
    [SerializeField]
    private float verticalDampingFactor = 14.0f;

    [Header("Scene Object References")]

    // The camera target
    [SerializeField]
    private Transform target;
 
    void Update()
    {
        // Verify variables.
        if (!target)
        {
            return;
        }

        Vector3 targetPosition;
        Vector3 positionDelta;

        // Calculate initial target position.
        targetPosition = new Vector3(
            target.position.x + (offset.x),
            target.position.y + (offset.y),
            target.position.z + (offset.z));
        
        // Get the distance from the current position to the target position.
        positionDelta = new Vector3( Mathf.Abs(transform.position.x - targetPosition.x),
            Mathf.Abs(transform.position.y - targetPosition.y),
            Mathf.Abs(transform.position.z - targetPosition.z));

        // Verify the target position.
        if(positionDelta.magnitude < positionThreshold)
        {
            // Don't change the position if the position delta is lower than the specified threshold.
            targetPosition = transform.position;
        }

        // Calculate the new damped target position. This position is influenced by the damping values and the quadratic distance to the target.
        targetPosition = new Vector3(
            Mathf.Lerp(this.transform.position.x, targetPosition.x, horizontalDampingFactor * dampingCurve.Evaluate(Mathf.Min(positionDelta.x / dampingDistance, 1.0f)) * Time.deltaTime),
            Mathf.Lerp(this.transform.position.y, targetPosition.y, verticalDampingFactor * dampingCurve.Evaluate(Mathf.Min(positionDelta.y / dampingDistance, 1.0f)) * Time.deltaTime),
            Mathf.Lerp(this.transform.position.z, targetPosition.z, horizontalDampingFactor * dampingCurve.Evaluate(Mathf.Min(positionDelta.z / dampingDistance, 1.0f)) * Time.deltaTime));

        // Update the position of the camera.
        transform.position = targetPosition;

        // Update the rotation of the camera.
        if (lookAtTargetPlane)
        {
            transform.LookAt(new Vector3(
                transform.position.x,
                target.position.y,
                target.position.z));
        }
        else
        {
            transform.LookAt(target);
        }
    }
}