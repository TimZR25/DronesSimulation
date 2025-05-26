using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] private int _minValue;
    [SerializeField] private int _maxValue;
    [SerializeField] private float _defaultValue;

    [SerializeField] private Slider _slider;

    [SerializeField] private Text _valueText;

    public UnityAction<float> ValueChanged;

    private void Start()
    {
        _slider.minValue = _minValue;
        _slider.maxValue = _maxValue;

        _slider.value = _defaultValue;
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        ValueChanged?.Invoke(value);

        _valueText.text = "x" + value.ToString();
    }
}
