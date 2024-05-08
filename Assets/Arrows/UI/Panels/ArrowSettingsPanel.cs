using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class that represents an UI Arrow settings panel and manages the initial states of the different components composing this panel
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

    // Input field to type in a new magnitude threshold value
    [SerializeField] private TMP_InputField _arrowMagnitudeThresholdInputField;

    private void Start()
    {
        if (ArrowForceVisualizerManager.Instance == null)
            SceneSettingsManager.Instance.OnArrowManagerStart += Initialize;

        else
            Initialize();
    }

    private void Initialize()
    {
        // The Button initial colors are based on the default values
        if (_arrowLowMagnitudeColorButton != null)
            _arrowLowMagnitudeColorButton.image.color = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude;

        if (_arrowHighMagnitudeColorButton != null)
            _arrowHighMagnitudeColorButton.image.color = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude;

        // The Color pickers' initial colors are based on the default values
        if (_arrowLowMagnitudeColorPicker != null)
            _arrowLowMagnitudeColorPicker.color = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude;

        if (_arrowHighMagnitudeColorPicker != null)
            _arrowHighMagnitudeColorPicker.color = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude;

        // Show or Hide arrows based on the toggle's default value
        if (arrowVisibilityToggle != null)
            ArrowForceVisualizerManager.Instance.eDI_ArrowVisibility.TriggerEvent(arrowVisibilityToggle.isOn);

        // Magnitude threshold based on the default value
        if (_arrowMagnitudeThresholdInputField != null)
            _arrowMagnitudeThresholdInputField.text = ArrowForceVisualizerManager.Instance.ArrowMagnitudeThreshold.ToString();

        // Hide Color pickers by default on start
        _arrowLowMagnitudeColorPicker.GetComponentInParent<Canvas>().enabled = false;
        _arrowHighMagnitudeColorPicker.GetComponentInParent<Canvas>().enabled = false;
    }
}