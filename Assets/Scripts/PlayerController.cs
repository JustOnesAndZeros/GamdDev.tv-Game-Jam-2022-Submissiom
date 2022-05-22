using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Player
{
    private PlayerInputActions _playerInputActions;

    private Queue<Action> _recordedActions;

    private int _actionCount;
    private bool _hasLeftSpawn;
    
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
        _recordedActions.Enqueue(new Action(SpawnManager.LoopTime, 0, direction)); //records new direction and time to be replayed next loop
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        Jump(jumpForce);
        _recordedActions.Enqueue(new Action(SpawnManager.LoopTime, 1, jumpForce)); //records jump force and time to be replayed next loop
    }

    public void SetControllable(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
        Rb.simulated = b;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Lethal")) return;
        
        //destroy all clones
        foreach (var c in GameObject.FindGameObjectsWithTag("Clone"))
        {
            Destroy(c.gameObject);
        }

        Reset();

        spawn.GetComponent<SpawnManager>().AddToQueue(_recordedActions);
        spawn.GetComponent<SpawnManager>().Reset();
    }

    private void Reset()
    {
        _hasLeftSpawn = false;
        transform.position = spawn.transform.position;
        SetControllable(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Respawn") || _hasLeftSpawn) return;
        _hasLeftSpawn = true;
        SpawnManager.SpawnTimes.Enqueue(SpawnManager.LoopTime);
    }
}


