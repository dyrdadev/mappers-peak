using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;


public interface ICharacterSignals  {
    float strideLength { get; }
    ReactiveProperty<bool> isRunning { get; }
    IObservable<Vector3> moved { get; }
    IObservable<Unit> landed { get; }
    IObservable<Unit> jumped { get; }
    IObservable<Unit> stepped { get; }
}
