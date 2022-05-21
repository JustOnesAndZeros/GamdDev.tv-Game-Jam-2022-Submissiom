using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _col;
    private PlayerInputActions _playerInputActions;

    [Tooltip("layer the player collides with")] [SerializeField] private LayerMask environment; //layer to check with boxcast
    [Tooltip("player will respawn here")] [SerializeField] private GameObject spawn; //spawn object
    [Space]
    [SerializeField] private float moveSpeed; //horizontal movement speed
    [SerializeField] private float jumpForce; //vertical impulse force for jumping

    private float _timePassed;
    private Queue<float[]> _playerActions;

    //get rigidbody and collider
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        
        _playerActions = new Queue<float[]>();
    }

    //enable user input and subscribe to events
    private void OnEnable()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

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

    private void Update()
    {
        _timePassed += Time.deltaTime;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        //sets horizontal player movement
        float moveDirection = ctx.ReadValue<float>();
        if (CheckDirection(new Vector2(moveDirection, 0))) return; //will not move player if a wall is in that direction (prevents sticking to walls)

        Vector2 velocity = new Vector2(moveDirection * moveSpeed, _rb.velocity.y);
        _rb.velocity = velocity;
        _playerActions.Enqueue(new[] {_timePassed, 0, velocity.x}); //records new direction and time to be replayed next loop
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!CheckDirection(Vector2.down)) return; //checks if player is touching the ground before jumping
        
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //applies vertical force to player
        _playerActions.Enqueue(new []{_timePassed, 1, jumpForce}); //records jump force and time to be replayed next loop
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
