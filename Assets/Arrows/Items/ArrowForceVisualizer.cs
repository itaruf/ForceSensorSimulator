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

    // Draw the arrow in-game with the LineRenderer component
    [SerializeField] private LineRenderer _arrowLineRenderer;

    private void Start()
    {
        if (ArrowForceVisualizerManager.Instance == null)
            SceneSettingsManager.Instance.OnArrowManagerStart += Initialize;

        else
            Initialize();
    }

    private void Initialize()
    {
        if (!_forceSensor)
            _forceSensor = GetComponentInParent<ForceSensor>();

        if (TryGetComponent(out _arrowLineRenderer))
        {
            _arrowLineRenderer.startWidth = 0.01f;
            _arrowLineRenderer.endWidth = 0.01f;
        }

        if (ArrowForceVisualizerManager.Instance != null)
        {
            ArrowForceVisualizerManager.onArrowVisibilityChange += (bVisibility) =>
            {
                if (bVisibility)
                {
                    if (_updateArrowWithForceDataCoroutine == null)
                    {
                        _updateArrowWithForceDataCoroutine = StartCoroutine(UpdateArrowWithForceData());
                        _arrowLineRenderer.enabled = true;
                    }
                }
                else
                {
                    if (_updateArrowWithForceDataCoroutine != null)
                    {
                        StopCoroutine(_updateArrowWithForceDataCoroutine);
                        _updateArrowWithForceDataCoroutine = null;
                        _arrowLineRenderer.enabled = false;
                    }
                }
            };

            if (ArrowForceVisualizerManager.Instance.ArrowVisibility && _arrowLineRenderer != null)
                _updateArrowWithForceDataCoroutine = StartCoroutine(UpdateArrowWithForceData());
        }
    }

    private IEnumerator UpdateArrowWithForceData()
    {
        float lerpTime = 0;
        float transitionDuration = /*1f*/ArrowForceVisualizerManager.Instance.UpdateDelay;
        float transitionSpeed = 0.5f;

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
            float magnitudeThreshold = ArrowForceVisualizerManager.Instance.ArrowMagnitudeThreshold;
            float scaledMagnitude = magnitude / magnitudeThreshold;

            // Choose the color based on the magnitude between the low and high color extremum
            Color targetColor = Color.Lerp(ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude, ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude, scaledMagnitude);

            // Smoothly transition to the selected color
            _color = Color.Lerp(_color, targetColor, lerpTime * transitionSpeed);

            // Draw the arrow
            DrawArrow.DrawWithLineRenderer(_arrowLineRenderer, transform.position, normalizedForce, _color, scaledMagnitude, 0.1f);
            //DrawArrow.DrawForDebug(transform.position, normalizedForce, _color, scaledMagnitude, 0, 0.1f);

            // Update the lerp time used to draw the arrow
            lerpTime += Time.deltaTime / transitionDuration;

            // Reset lerp time
            if (lerpTime >= 1f)
                lerpTime = 0; 

            yield return null;
        }
    }
}