using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;


// There's nothing new in CameraBob since Part 2
[RequireComponent(typeof(Camera))]
public class CameraBob : MonoBehaviour {

    [SerializeField]
    private GameObject characterSignalsInterfaceTarget;
    private ICharacterSignals characterSignals;
    
    public float walkBobMagnitude = 0.05f;
    public float runBobMagnitude = 0.10f;

    public AnimationCurve bob = new AnimationCurve(
        new Keyframe(0.00f,  0f),
        new Keyframe(0.25f,  1f),
        new Keyframe(0.50f,  0f),
        new Keyframe(0.75f, -1f),
        new Keyframe(1.00f,  0f));

    private Camera _camera;
    private Vector3 initialPosition;

    private void Awake() {
        characterSignals = characterSignalsInterfaceTarget.GetComponent<FirstPersonCharacterController>();
        _camera = GetComponent<Camera>();
        initialPosition = _camera.transform.localPosition;
    }

    private void Start() {
        var distance = 0f;
        characterSignals.moved.Subscribe(w => {
            // Accumulate distance walked (modulo stride length).
            distance += w.magnitude;
            distance %= characterSignals.strideLength;
            // Use distance to evaluate the bob curve.
            var magnitude = characterSignals.isRunning.Value ? runBobMagnitude : walkBobMagnitude;
            var deltaPos = magnitude * bob.Evaluate(distance / characterSignals.strideLength) * Vector3.up;
            // Adjust camera position.
            _camera.transform.localPosition = initialPosition + deltaPos;
        }).AddTo(this);
    }
}