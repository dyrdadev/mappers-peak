using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HierarchicalDataSynchronizer<DataType, SynchronizerType> : MonoBehaviour where SynchronizerType: HierarchicalDataSynchronizer<DataType, SynchronizerType>
{
    protected DataType synchronizedValue;
    private readonly List<HierarchicalDataSynchronizer<DataType, SynchronizerType>> registeredSynchronizers = new List<HierarchicalDataSynchronizer<DataType, SynchronizerType>>();
    
    public virtual void Start()
    {
        // Register this synchronizer at its next parent synchronizer.
        this.transform.parent.GetComponentInParent<SynchronizerType>().RegisterSynchronizer(this);
    }

    private void RegisterSynchronizer(HierarchicalDataSynchronizer<DataType, SynchronizerType> hierarchicalDataSynchronizer)
    {
        this.registeredSynchronizers.Add(hierarchicalDataSynchronizer);
    }

    public void UpdateValue(DataType data)
    {
        // Calculate new data.
        var newData = ProcessDataBeforeUpdate(data);
        
        // Update all registered synchronizers when update actually changed something.
        if (!EqualityComparer<DataType>.Default.Equals(newData , this.synchronizedValue))
        {
            foreach (var registeredSynchronizer in registeredSynchronizers)
            {
                registeredSynchronizer.UpdateValue(newData);
            }
        }
        
        // Update the value of this synchronizer.
        this.synchronizedValue = newData;
    }

    public DataType GetValue()
    {
        return this.synchronizedValue;
    }

    protected virtual DataType ProcessDataBeforeUpdate(DataType newData)
    {
        return newData;
    }
}
