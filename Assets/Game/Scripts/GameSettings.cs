using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private Slider _timeSlider;

    [SerializeField] private Text _timeText;

    private float _timeScale;

    private void Start()
    {
        _timeSlider.value = Time.timeScale;
    }

    private void OnEnable()
    {
        _timeSlider.onValueChanged.AddListener(OnTimeScaled);
    }

    private void OnDisable()
    {
        _timeSlider.onValueChanged.RemoveListener(OnTimeScaled);
    }

    private void OnTimeScaled(float value)
    {
        _timeScale = value;

        _timeText.text = "x" + _timeScale.ToString();

        Time.timeScale = _timeScale;
    }
}