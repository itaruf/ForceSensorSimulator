using System.Collections.Generic;
using UnityEngine;

// Event Dispatcher for Color changes
public abstract class ColorEventDispatcher : ScriptableObject
{
    protected List<ColorEvents.OnColorChange> listeners = new List<ColorEvents.OnColorChange>();

    public virtual void TriggerEvent(Color color)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke(color);
    }

    public virtual void AddListener(ColorEvents.OnColorChange listener)
    {
        listeners.Add(listener);
    }

    public virtual void RemoveListener(ColorEvents.OnColorChange listener)
    {
        listeners.Remove(listener);
    }
}