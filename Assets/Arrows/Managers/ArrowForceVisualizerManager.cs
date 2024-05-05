using UnityEngine;

// Class that manages the settings for the arrows representing the force sensors readings
public class ArrowForceVisualizerManager : MonoBehaviour
{
    public static ArrowForceVisualizerManager instance;

    // Colors for low and high magnitude values
    [SerializeField] private Color _arrowColorLowMagnitude;
    [SerializeField] private Color _arrowColorHighMagnitude;

    // Keep track of the visibility of all generated arrows
    private bool _arrowVisibility = true;

    // Magnitude threshold
    [SerializeField] private float _arrowMagnitudeThreshold = 5f;

    // Arrow Event Dispatchers to bind to
    public ColorEventDispatcher eDI_ArrowLowMagnitudeColor;
    public ColorEventDispatcher eDI_ArrowHighMagnitudeColor;
    public BoolEventDispatcher eDI_ArrowVisibility;
    public StringEventDispatcher eDI_ArrowMagnitudeThreshold;

    // Arrow Events to trigger in response to the event dispatchers
    public static event ColorEvents.OnColorChange onArrowColorChangeByMagnitude;
    public static event VisibilityEvents.OnVisibilityChange onArrowVisibilityChange;
    public static event ForceEvents.OnForceMagnitudeChange onForceMagnitudeChangeByMagnitude;

    // Property accessors
    public Color ArrowColorLowMagnitude { get => _arrowColorLowMagnitude; }
    public Color ArrowColorHighMagnitude { get => _arrowColorHighMagnitude; }
    public bool ArrowVisibility { get => _arrowVisibility; }
    public float ArrowMagnitudeThreshold { get => _arrowMagnitudeThreshold; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (eDI_ArrowLowMagnitudeColor != null)
            eDI_ArrowLowMagnitudeColor.AddListener(UpdateLowMagnitudeColor);

        if (eDI_ArrowHighMagnitudeColor != null)
            eDI_ArrowHighMagnitudeColor.AddListener(UpdateHighMagnitudeColor);

        if (eDI_ArrowVisibility != null)
            eDI_ArrowVisibility.AddListener(UpdateVisibility);

        if (eDI_ArrowMagnitudeThreshold != null)
            eDI_ArrowMagnitudeThreshold.AddListener(UpdateMagnitudeThreshold);
    }

    private void UpdateVisibility(bool bVisibility)
    {
        _arrowVisibility = bVisibility;
        onArrowVisibilityChange?.Invoke(bVisibility);
    }

    private void UpdateLowMagnitudeColor(Color color)
    {
        _arrowColorLowMagnitude = color;
        onArrowColorChangeByMagnitude?.Invoke(_arrowColorLowMagnitude);
    }

    private void UpdateHighMagnitudeColor(Color color)
    {
        _arrowColorHighMagnitude = color;
        onArrowColorChangeByMagnitude?.Invoke(_arrowColorHighMagnitude);
    }

    private void UpdateMagnitudeThreshold(string value)
    {
        if (float.TryParse(value, out float parsedValue))
        {
            _arrowMagnitudeThreshold = parsedValue;
            onForceMagnitudeChangeByMagnitude?.Invoke(_arrowMagnitudeThreshold);
        } 
    }
}