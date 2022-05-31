using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : Player
{
    private PlayerInputActions _playerInputActions;

    private Queue<Action> _recordedActions;

    //enable user input and subscribe to events
    private void OnEnable()
    {
        _recordedActions = new Queue<Action>();
        
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Movement.RestartLevel.started += OnRestartLevel;
        _playerInputActions.Movement.RestartGame.started += OnRestartGame;
        _playerInputActions.Movement.Quit.started += OnQuit;

        _playerInputActions.Movement.Horizontal.started += OnMove;
        _playerInputActions.Movement.Horizontal.canceled += OnMove;
        
        _playerInputActions.Movement.Jump.started += OnJump;
        _playerInputActions.Movement.Jump.canceled += OnJump;

        _playerInputActions.Enable();
    }

    //unsubscribes from events
    private void OnDisable()
    {
        _playerInputActions.Movement.RestartLevel.started -= OnRestartLevel;
        _playerInputActions.Movement.RestartGame.started -= OnRestartGame;
        _playerInputActions.Movement.Quit.started -= OnQuit;
        
        _playerInputActions.Movement.Horizontal.started -= OnMove;
        _playerInputActions.Movement.Horizontal.canceled -= OnMove;
        
        _playerInputActions.Movement.Jump.started -= OnJump;
        _playerInputActions.Movement.Jump.canceled -= OnJump;
        
        _playerInputActions.Disable();
    }

    private void FixedUpdate()
    {
        IEnumerable<GameObject> players = GameObject.FindGameObjectsWithTag("Clone").Concat(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject player in players) if (player.transform.parent == null) player.GetComponent<Player>().SetVelocity(Vector2.zero);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        float direction = ctx.ReadValue<float>();
        Move(direction);
        _recordedActions.Enqueue(new Action(TimePassed, 0, direction)); //records new direction and time to be replayed next loop
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        float isDown = ctx.started ? 1 : 0;
        Jump(isDown);
        _recordedActions.Enqueue(new Action(TimePassed, 1, isDown)); //records jump force and time to be replayed next loop
    }

    protected override void OnDeath()
    {
        _recordedActions.Enqueue(new Action(TimePassed, -1, -1));
        SpawnScript.AddToQueue(_recordedActions);
    }

    private void OnRestartLevel(InputAction.CallbackContext ctx)
    {
        SpawnScript.ResetLevel();
    }
    
    private void OnRestartGame(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(0);
    }
    
    private void OnQuit(InputAction.CallbackContext ctx)
    {
        Application.Quit();
    }
}