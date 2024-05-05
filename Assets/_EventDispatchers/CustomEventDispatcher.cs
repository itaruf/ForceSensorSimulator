using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Custom generic event dispatcher to create child from
public abstract class CustomEventDispatcher<T> : ScriptableObject
{
    protected List<UnityAction<T>> listeners = new List<UnityAction<T>>();

    public virtual void TriggerEvent(T listener)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].Invoke(listener);
    }

    public virtual void AddListener(UnityAction<T> listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public virtual void RemoveListener(UnityAction<T> listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }
}