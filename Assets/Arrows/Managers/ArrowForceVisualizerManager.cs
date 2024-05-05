using UnityEngine;

// Class that manages the settings for the arrows representing the force sensors readings
public class ArrowForceVisualizerManager : MonoBehaviour
{
    public static ArrowForceVisualizerManager instance;

    // Colors for low and high magnitude values
    [SerializeField] private Color _colorLowMagnitude;
    [SerializeField] private Color _colorHighMagnitude;

    // Keep track of the visibility of all generated arrows
    private bool _arrowVisibility = true;

    // Arrow Event Dispatchers to bind to
    public ColorEventDispatcher eDI_ArrowLowMagnitudeColor;
    public ColorEventDispatcher eDI_ArrowHighMagnitudeColor;
    public BoolEventDispatcher eDI_ArrowVisibility;
    public FloatEventDispatcher eDI_ArrowMagnitude;

    // Arrow Events to trigger in response to the event dispatchers
    public static event ColorEvents.OnColorChange onArrowColorChangeByMagnitude;
    public static event VisibilityEvents.OnVisibilityChange onArrowVisibilityChange;

    // Property accessors
    public Color ColorLowMagnitude { get => _colorLowMagnitude; }
    public Color ColorHighMagnitude { get => _colorHighMagnitude; }
    public bool ArrowVisibility { get => _arrowVisibility; }

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

        if (eDI_ArrowMagnitude != null)
            eDI_ArrowMagnitude.AddListener(Test);
    }

    void UpdateVisibility(bool bVisibility)
    {
        _arrowVisibility = bVisibility;
        onArrowVisibilityChange?.Invoke(bVisibility);
    }

    private void UpdateLowMagnitudeColor(Color color)
    {
        _colorLowMagnitude = color;
        onArrowColorChangeByMagnitude?.Invoke(_colorLowMagnitude);
    }

    private void UpdateHighMagnitudeColor(Color color)
    {
        _colorHighMagnitude = color;
        onArrowColorChangeByMagnitude?.Invoke(_colorHighMagnitude);
    }

    private void Test(float value)
    {
        Debug.Log(value);
    }
}