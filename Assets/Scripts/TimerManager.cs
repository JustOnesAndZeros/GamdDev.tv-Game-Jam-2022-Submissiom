using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimerManager : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    
    private TextMeshProUGUI _textMesh;
    
    private float _timePassed;
    public float milliseconds;
    public float seconds;
    public float minutes;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        
        _playerInputActions.Movement.RestartGame.started += OnRestartGame;
    }

    private void OnDisable()
    {
        _playerInputActions.Movement.RestartGame.started -= OnRestartGame;
        _playerInputActions.Enable();
    }
    
    private void OnRestartGame(InputAction.CallbackContext ctx)
    {
        _timePassed = 0;
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        milliseconds = (int)((_timePassed - Math.Truncate(_timePassed)) * 100);
        seconds = (int)(Math.Truncate(_timePassed) % 60);
        minutes = (int)Math.Truncate(Math.Truncate(_timePassed) / 60);

        _textMesh.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void Hide()
    {
        _textMesh.enabled = false;
    }
}
