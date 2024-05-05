using UnityEngine;

// Scriptable object that serves as an event dispatcher to handle and dispatch Color messages
[CreateAssetMenu(fileName = "Color Event Dispatcher", menuName = "Generic Event Dispatcher/Color Event Dispatcher", order = 0)]
public class ColorEventDispatcher : CustomEventDispatcher<Color>
{
}