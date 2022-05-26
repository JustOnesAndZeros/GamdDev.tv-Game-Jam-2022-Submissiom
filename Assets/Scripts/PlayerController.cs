using System;
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
        Rb = GetComponent<Rigidbody2D>();
        
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

    public void SetControllable(bool b)
    {
        transform.localScale = b ? Vector3.one : Vector3.zero;
        Rb.simulated = b;
    }

    protected override void OnDeath()
    {
        //destroy all clones
        foreach (var c in GameObject.FindGameObjectsWithTag("Clone")) Destroy(c.gameObject);
        
        _recordedActions.Enqueue(new Action(TimePassed, -1, -1));
        SpawnScript.AddToQueue(_recordedActions);
        SpawnScript.Reset();
        Reset();
    }

    private void Reset()
    {
        _recordedActions = new Queue<Action>();
        transform.position = spawn.transform.position;
        Rb.velocity = Vector2.zero;
        Move(0);
        SetControllable(false);
        TimePassed = 0;
    }
}


