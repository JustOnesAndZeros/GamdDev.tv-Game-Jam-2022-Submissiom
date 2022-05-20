using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerInputActions _playerInputActions;
    private InputAction _horizontalMove;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _horizontalMove = _playerInputActions.Movement.Horizontal;
        
        _playerInputActions.Movement.Jump.started += OnJump;
    }

    private void OnDisable()
    {
        _playerInputActions.Movement.Jump.started -= OnJump;
        
        _playerInputActions.Disable();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_horizontalMove.ReadValue<float>() * moveSpeed, _rb.velocity.y);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
