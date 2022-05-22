using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Player
{
    private PlayerInputActions _playerInputActions;

    private Queue<Action> _recordedActions;

    private int _actionCount;
    
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

    private void Reset()
    {
        transform.position = spawn.transform.position;
        GetComponent<SpriteRenderer>().enabled = false;
        _rb.simulated = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Lethal")) return;
        
        //move back to spawn
        foreach (var p in GameObject.FindGameObjectsWithTag("Clone"))
        {
            //reset and hide game object
            p.GetComponent<CloneController>().Reset();
        }

        Reset();
        
        spawn.GetComponent<SpawnManager>().AddToQueue(_recordedActions);
        spawn.GetComponent<SpawnManager>().Reset();
        spawn.GetComponent<SpawnManager>().SpawnNextItemInQueue();
    }
}


