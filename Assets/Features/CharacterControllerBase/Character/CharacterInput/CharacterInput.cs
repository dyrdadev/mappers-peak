using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public abstract class CharacterInput : MonoBehaviour
{
    /// <summary>
    /// Move axes in WASD / D-Pad style.
    /// Interaction type: continuous axes. (Value each update)
    /// </summary>
    public abstract IObservable<Vector2> move { get; }
    
    /// <summary>
    /// Jump button.
    /// Interaction type: Trigger. (Events on trigger.)
    /// </summary>
    public abstract IObservable<Unit> jump { get;  }
    
    /// <summary>
    /// Run button.
    /// Interaction type: Toggle. (True or false each update)
    /// </summary>
    public abstract ReadOnlyReactiveProperty<bool> run { get; }
    
    /// <summary>
    /// Look axes following the free look (mouse look) pattern.
    /// Interaction type: continuous axes. (Value each update)
    /// </summary>
    public abstract IObservable<Vector2> look { get; }
}



