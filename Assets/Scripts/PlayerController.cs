using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _col;
    private PlayerInputActions _playerInputActions;
    private InputAction _horizontalMove;

    [Tooltip("layer the player collides with")] [SerializeField] private LayerMask environment;
    [Tooltip("player will respawn here")] [SerializeField] private GameObject spawn;
    [Space]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private float _moveDirection;

    //get rigidbody and collider
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    //enable user input and subscribe to events
    private void OnEnable()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _horizontalMove = _playerInputActions.Movement.Horizontal;

        _playerInputActions.Movement.Horizontal.started += OnMove;
        _playerInputActions.Movement.Horizontal.performed += OnMove;
        _playerInputActions.Movement.Horizontal.canceled += OnMove;
        
        _playerInputActions.Movement.Jump.started += OnJump;

        KillOnContact.OnDeath += OnDeath;
    }

    private void Start()
    {
        transform.position = spawn.transform.position; //spawns player at spawn point
    }

    //unsubscribes from events
    private void OnDisable()
    {
        _playerInputActions.Movement.Horizontal.started -= OnMove;
        _playerInputActions.Movement.Horizontal.performed -= OnMove;
        _playerInputActions.Movement.Horizontal.canceled -= OnMove;
        
        _playerInputActions.Movement.Jump.started -= OnJump;
        
        _playerInputActions.Disable();
        
        KillOnContact.OnDeath -= OnDeath;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        //sets horizontal player movement
        _moveDirection = _horizontalMove.ReadValue<float>();
        if (!CheckDirection(new Vector2(_moveDirection, 0))) //will not move player if a wall is in that direction (prevents sticking to walls)
        {
            _rb.velocity = new Vector2(_horizontalMove.ReadValue<float>() * moveSpeed, _rb.velocity.y); //moves player
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (CheckDirection(Vector2.down)) _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //applies vertical force to player
    }

    //checks if the player is touching a wall in the specified direction
    //used for ground checks and to prevent sticking to walls
    private bool CheckDirection(Vector2 direction)
    {
        var bounds = _col.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, .1f, environment);
    }

    //executed when the player collides with a lethal object
    private void OnDeath(GameObject player)
    {
        transform.position = spawn.transform.position; //teleports player back to spawn point
    }
}
