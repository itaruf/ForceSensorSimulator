using UnityEngine;
using UnityEngine.UI;

public class ArrowSettingsPanel : MonoBehaviour
{
    // Buttons to show/hide the color picker panels
    [SerializeField] private Button _arrowLowMagnitudeColorButton;
    [SerializeField] private Button _arrowHighMagnitudeColorButton;

    // Viibility settings
    [SerializeField] private Toggle arrowVisibilityToggle;

    // Color picker panels to represent low and high magnitude values
    [SerializeField] private FlexibleColorPicker _arrowLowMagnitudeColorPicker;
    [SerializeField] private FlexibleColorPicker _arrowHighMagnitudeColorPicker;

    private void Start()
    {
        // The Button initial colors are based on the default values
        if (_arrowLowMagnitudeColorButton != null)
            _arrowLowMagnitudeColorButton.image.color = ArrowForceVisualizerManager.instance.ColorLowMagnitude;

        if (_arrowHighMagnitudeColorButton != null)
            _arrowHighMagnitudeColorButton.image.color = ArrowForceVisualizerManager.instance.ColorHighMagnitude;

        // The Color pickers' initial colors are based on the default values
        if (_arrowLowMagnitudeColorPicker != null)
            _arrowLowMagnitudeColorPicker.color = ArrowForceVisualizerManager.instance.ColorLowMagnitude;

        if (_arrowHighMagnitudeColorPicker != null)
            _arrowHighMagnitudeColorPicker.color = ArrowForceVisualizerManager.instance.ColorHighMagnitude;

        // Show or Hide arrows based on the toggle's default value
        if (arrowVisibilityToggle != null)
            ArrowForceVisualizerManager.instance.eDI_ArrowVisibility.TriggerEvent(arrowVisibilityToggle.isOn);

        // Hide Color pickers by default on start
        _arrowLowMagnitudeColorPicker.GetComponentInParent<Canvas>().enabled = false;
        _arrowHighMagnitudeColorPicker.GetComponentInParent<Canvas>().enabled = false;
    }
}