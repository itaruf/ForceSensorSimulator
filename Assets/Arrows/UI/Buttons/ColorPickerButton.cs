using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPickerButton : MonoBehaviour, IColor, IVisibility
{
    [SerializeField] private ColorEventDispatcher eDI_Color;

    // Associated window (canvas) this button will manage
    [SerializeField] private Canvas _canvas;

    // The image's color 
    private Image _image;

    // The button displays or hides this window (canvas)
    private Button _button;

    private void Start()
    {
        TryGetComponent(out _image);

        if (eDI_Color != null)
            eDI_Color.AddListener(((IColor)this).UpdateColor);

        if (TryGetComponent(out _button) && _canvas != null)
            _button.onClick.AddListener(() => { ((IVisibility)this).UpdateVisibility(!_canvas.enabled); });
    }

    void IColor.UpdateColor(Color color)
    {
        if (_image != null)
            _image.color = color;
    }

    void IVisibility.UpdateVisibility(bool bVisibility)
    {
        _canvas.enabled = bVisibility;
    }
}