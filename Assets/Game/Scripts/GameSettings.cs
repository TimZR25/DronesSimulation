using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private SettingSlider _droneCountSlider;

    [SerializeField] private SettingSlider _droneSpeedSlider;

    [SerializeField] private SettingSlider _simulationSpeedSlider;

    [SerializeField] private Dropdown _resourceRateDropdown;

    [SerializeField] private Toggle _pathViewToggle;

    public static UnityAction<float> DroneCountChanged;
    public static UnityAction<float> DroneSpeedChanged;
    public static UnityAction<int> ResourceRateChanged;
    public static UnityAction<bool> PathViewChanged;

    private void OnEnable()
    {
        _droneCountSlider.ValueChanged += OnDroneCountChanged;
        _droneSpeedSlider.ValueChanged += OnDroneSpeedChanged;
        _simulationSpeedSlider.ValueChanged += OnSimulationSpeedChanged;
        _pathViewToggle.onValueChanged.AddListener(OnPathViewToggleChanged);
        _resourceRateDropdown.onValueChanged.AddListener(OnResourceRateDropdownChanged);
    }

    private void OnDisable()
    {
        _droneCountSlider.ValueChanged -= OnDroneCountChanged;
        _droneSpeedSlider.ValueChanged -= OnDroneSpeedChanged;
        _simulationSpeedSlider.ValueChanged -= OnSimulationSpeedChanged;
        _pathViewToggle.onValueChanged.RemoveListener(OnPathViewToggleChanged);
        _resourceRateDropdown.onValueChanged.RemoveListener(OnResourceRateDropdownChanged);
    }

    private void OnSimulationSpeedChanged(float value)
    {
        Time.timeScale = value;
    }

    private void OnDroneCountChanged(float value)
    {
        DroneCountChanged?.Invoke(value);
    }

    private void OnDroneSpeedChanged(float value)
    {
        DroneSpeedChanged?.Invoke(value);
    }

    private void OnPathViewToggleChanged(bool value)
    {
        PathViewChanged?.Invoke(value);
    }

    private void OnResourceRateDropdownChanged(int value)
    {
        ResourceRateChanged?.Invoke(value);
    }
}