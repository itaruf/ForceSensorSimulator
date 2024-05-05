using System.Collections.Generic;
using UnityEngine;

// Event Dispatcher for Visibility changes
public abstract class VisiblityEventDispatcher : ScriptableObject
{
    protected List<VisibilityEvents.OnVisibilityChange> listeners = new List<VisibilityEvents.OnVisibilityChange>();

    public virtual void TriggerEvent(bool bVisibility)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke(bVisibility);
    }

    public virtual void AddListener(VisibilityEvents.OnVisibilityChange listener)
    {
        listeners.Add(listener);
    }

    public virtual void AddInterfaceListener(VisibilityEvents.OnVisibilityChange listener)
    {
        listeners.Add(listener);
    }

    public virtual void RemoveListener(VisibilityEvents.OnVisibilityChange listener)
    {
        listeners.Remove(listener);
    }
}