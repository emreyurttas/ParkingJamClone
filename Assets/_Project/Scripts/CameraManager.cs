using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject _confetti;

    private void Start()
    {
        GameManager.Instance.WinGame += OnWinGame;
    }

    private void OnDisable()
    {
        GameManager.Instance.WinGame -= OnWinGame;
    }

    void OnWinGame ()
    {
        _confetti.SetActive(true);
    }
}
