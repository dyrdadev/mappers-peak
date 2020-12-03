
using System;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;

public static class CustomObservables
{

    public static IObservable<bool> Latch(IObservable<Unit> tick, IObservable<Unit> latchTrue, bool initialValue)
    {
        // Create a custom Observable, whose behavior is determined by our calls to the provided 'observable'
        return Observable.Create<bool>(observer =>
        {
            // Our state value.
            var value = initialValue;

            // Create an inner subscription to latch:
            // Whenever latch fires, store true.
            var latchSub = latchTrue.Subscribe(_ => value = true);

            // Create an inner subscription to tick:
            var tickSub = tick.Subscribe(
                // Whenever tick fires, send the current value and reset state.
                _ =>
                {
                    observer.OnNext(value);
                    value = false;
                },
                observer.OnError, // pass through tick's errors (if any)
                observer.OnCompleted); // complete when tick completes

            // If we're disposed, dispose inner subscriptions too.
            return Disposable.Create(() =>
            {
                latchSub.Dispose();
                tickSub.Dispose();
            });
        });
    }

    public static IObservable<T> SelectRandom<T>(this IObservable<Unit> eventObs, T[] items)
    {
        // Edge-cases:
        var n = items.Length;
        if (n == 0)
        {
            // No items!
            return Observable.Empty<T>();
        }
        else if (n == 1)
        {
            // Only one item!
            return eventObs.Select(_ => items[0]);
        }

        var myItems = (T[]) items.Clone();
        return Observable.Create<T>(observer =>
        {
            var sub = eventObs.Subscribe(_ =>
                {
                    // Select any item after the first.
                    var i = Random.Range(1, n);
                    var value = myItems[i];
                    // Swap with value at index 0 to avoid selecting an item twice in a row.
                    var temp = myItems[0];
                    myItems[0] = value;
                    myItems[i] = temp;
                    // Finally emit the selected value.
                    observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);

            return Disposable.Create(() => sub.Dispose());
        });
    }
}
