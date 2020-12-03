using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class InputSystemCharacterInput : CharacterInput
{
    private IObservable<Vector2> _move;
    public override IObservable<Vector2> move
    {
        get { return _move; }
    }

    private Subject<Unit> _jump;
    public override IObservable<Unit> jump
    {
        get { return _jump; }
    }

    private ReadOnlyReactiveProperty<bool> _run;
    public override ReadOnlyReactiveProperty<bool> run
    {
        get { return _run; }
    }

    private IObservable<Vector2> _look;
    public override IObservable<Vector2> look
    {
        get { return _look; }
    }

    private MainInput controls;
    
    [Header("Look properties")]
    public Vector2 mouselookSensitivity = new Vector2(150f, 150f);
    public float mouselookSmoothing = 2.0f;
    private Vector2 smoothMouselook = new Vector2(0,0);


    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    

    protected void Awake()
    {
        // Initialize values:
        controls = new MainInput();
        _jump = new Subject<Unit>().AddTo(this);
        
        // Hide the mouse cursor and lock it in the game window.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 60;

        // Move:
        _move = this.UpdateAsObservable()
            .Select(_ =>
            {
                return controls.Character.Move.ReadValue<Vector2>();
            });
   
        // Jump:
        controls.Character.Jump.performed += context => {_jump.OnNext(Unit.Default); };

        // Run:
        _run = this.UpdateAsObservable()
            .Select(_ => false)// Input.GetButton("Fire3"))
            .ToReadOnlyReactiveProperty();
        
        // Look:
        _look = this.UpdateAsObservable()
            .Select(_ =>
            {
                // Get the current raw axis values.
                var currentMouselook = controls.Character.Look.ReadValue<Vector2>(); //Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

                // Smooth the signal.
                smoothMouselook = new Vector2(
                    Mathf.Lerp(smoothMouselook.x, currentMouselook.x, mouselookSmoothing * Time.deltaTime),
                    Mathf.Lerp(smoothMouselook.y, currentMouselook.y, mouselookSmoothing * Time.deltaTime)
                    );

                return smoothMouselook;
            });
    }
}
