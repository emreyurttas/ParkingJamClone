using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UnityAction WinGame;

    [SerializeField] GameData _gameData;
    List<VehicleControl> _vehicleControls = new List<VehicleControl>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _vehicleControls.AddRange(FindObjectsByType<VehicleControl>(FindObjectsSortMode.InstanceID));
    }

    public void OnDestroyVehicle (VehicleControl vehicleControl)
    {
        _vehicleControls.Remove(vehicleControl);
        Destroy(vehicleControl.gameObject);
        if (_vehicleControls.Count == 0)
        {
            WinGame?.Invoke();
            Invoke(nameof(OnNextLevel), 1f);
        }
    }

    void OnNextLevel ()
    {
        _gameData.CurrentLevelIndex++;
        if (_gameData.CurrentLevelIndex >= _gameData.TotalLevelIndex)
            _gameData.CurrentLevelIndex = 0;

        SceneManager.LoadScene(_gameData.CurrentLevelIndex);
    }
}
