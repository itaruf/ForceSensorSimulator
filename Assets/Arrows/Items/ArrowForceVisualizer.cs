using System.Collections;
using UnityEngine;

// Class that visually represents the associated force sensor readings with an arrow
public class ArrowForceVisualizer : MonoBehaviour
{
    // Associated force sensor
    [SerializeField] private ForceSensor _forceSensor;

    // Color changes based on the magnitude
    private Color _color;

    // Update the arrow with the simulated force data provided
    private Coroutine _updateArrowWithForceDataCoroutine;

    private void Start()
    {
        if (!_forceSensor)
            _forceSensor = GetComponentInParent<ForceSensor>();

        if (ArrowForceVisualizerManager.instance != null)
        {
            ArrowForceVisualizerManager.onArrowColorChangeByMagnitude += (color) => { _color = color; };
            ArrowForceVisualizerManager.onArrowVisibilityChange += (bVisibility) =>
            {
                if (bVisibility)
                {
                    if (_updateArrowWithForceDataCoroutine == null)
                    {
                        _updateArrowWithForceDataCoroutine = StartCoroutine(UpdateArrowWithForceData());
                    }
                }
                else
                {
                    if (_updateArrowWithForceDataCoroutine != null)
                    {
                        StopCoroutine(_updateArrowWithForceDataCoroutine);
                        _updateArrowWithForceDataCoroutine = null;
                    }
                }
            };

            if (ArrowForceVisualizerManager.instance.ArrowVisibility)
                _updateArrowWithForceDataCoroutine = StartCoroutine(UpdateArrowWithForceData());
        }
    }

    private IEnumerator UpdateArrowWithForceData()
    {
        float lerpTime = 0;
        float transitionDuration = /*1f*/ForceSensorManager.instance.updateDelay;
        float transitionSpeed = 1f;

        while (true)
        {
            if (!_forceSensor)
                yield break;

            // Get the provided force from the external library
            Vector3 force = _forceSensor.ForceSensorData.force;

            // Get the length of the force
            float magnitude = force.magnitude;

            // Get the direction of the force
            Vector3 normalizedForce = force.normalized;

            // The magnitude is capped
            float magnitudeThreshold = ArrowForceVisualizerManager.instance.ArrowMagnitudeThreshold;
            float scaledMagnitude = magnitude / magnitudeThreshold;

            // Smoothly transitioning the color based on the magnitude
            Color targetColor = Color.Lerp(ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude, ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude, scaledMagnitude);

            lerpTime += Time.deltaTime / transitionDuration; 
             
            _color = Color.Lerp(_color, targetColor, lerpTime * transitionSpeed);

            DebugArrow.DrawForDebug(transform.position, normalizedForce, _color, scaledMagnitude);

            if (lerpTime >= 1f)
                lerpTime = 0; // Reset lerp time

            yield return null;
        }
    }
}