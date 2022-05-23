using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Player
{
    private PlayerInputActions _playerInputActions;

    private Queue<Action> _recordedActions;
    
    //enable user input and subscribe to events
    private void OnEnable()
    {
        _recordedActions = new Queue<Action>();
        
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Movement.Horizontal.started += OnMove;
        _playerInputActions.Movement.Horizontal.canceled += OnMove;
        
        _playerInputActions.Movement.Jump.started += OnJump;

        _playerInputActions.Enable();
    }

    //unsubscribes from events
    private void OnDisable()
    {
        _playerInputActions.Movement.Horizontal.started -= OnMove;
        _playerInputActions.Movement.Horizontal.canceled -= OnMove;
        
        _playerInputActions.Movement.Jump.started -= OnJump;
        
        _playerInputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        float direction = ctx.ReadValue<float>();
        Move(direction);
        _recordedActions.Enqueue(new Action(SpawnScript.timePassed, 0, direction)); //records new direction and time to be replayed next loop
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        Jump(jumpForce);
        _recordedActions.Enqueue(new Action(SpawnScript.timePassed, 1, jumpForce)); //records jump force and time to be replayed next loop
    }

    public void SetControllable(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
        Rb.simulated = b;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
       base.OnCollisionEnter2D(col);
        
       if (col.gameObject.CompareTag("Lethal")) OnDeath();
    }

    private void OnDeath()
    {
        //destroy all clones
        foreach (var c in GameObject.FindGameObjectsWithTag("Clone")) Destroy(c.gameObject);
        
        _recordedActions.Enqueue(new Action(SpawnScript.timePassed, -1, -1));
        SpawnScript.AddToQueue(_recordedActions);
        SpawnScript.Reset();
        
        Reset();
    }

    private void Reset()
    {
        _recordedActions = new Queue<Action>();
        Rb.velocity = Vector2.zero;
        transform.position = spawn.transform.position;
        SetControllable(false);
    }
}


