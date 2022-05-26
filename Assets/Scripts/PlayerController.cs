using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Player
{
    private PlayerInputActions _playerInputActions;

    private Queue<Action> _recordedActions;

    private bool _hasMoved;

    //enable user input and subscribe to events
    private void OnEnable()
    {
        _recordedActions = new Queue<Action>();
        
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Movement.Horizontal.started += OnMove;
        _playerInputActions.Movement.Horizontal.canceled += OnMove;
        
        _playerInputActions.Movement.Jump.started += OnJump;
        _playerInputActions.Movement.Jump.canceled += OnJump;

        _playerInputActions.Enable();
    }

    //unsubscribes from events
    private void OnDisable()
    {
        _playerInputActions.Movement.Horizontal.started -= OnMove;
        _playerInputActions.Movement.Horizontal.canceled -= OnMove;
        
        _playerInputActions.Movement.Jump.started -= OnJump;
        _playerInputActions.Movement.Jump.canceled -= OnJump;
        
        _playerInputActions.Disable();
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

    private void Update()
    {
        TimePassed += Time.deltaTime;
    }

    protected override void OnDeath()
    {
        _recordedActions.Enqueue(new Action(TimePassed, -1, -1));
        SpawnScript.AddToQueue(_recordedActions);
        SpawnScript.Reset();
    }
}


