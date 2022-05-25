using System;
using System.Linq;
using UnityEngine;

public struct Action
{
    public readonly float Time;
    public readonly int ActionType;
    public readonly float Value;

    public Action(float time, int actionType, float value)
    {
        Time = time;
        ActionType = actionType;
        Value = value;
    }
}

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _col;

    public GameObject spawn; //spawn object
    protected SpawnManager SpawnScript;

    [SerializeField] private Rigidbody2D carryPlayer;
    
    [SerializeField] private LayerMask playerLayer; //layer to check with boxcast
    [SerializeField] private LayerMask jumpLayer;
    
    [SerializeField] protected float moveSpeed; //horizontal movement speed
    private float _moveDirection;
    
    [SerializeField] private float jumpForce; //vertical impulse force for jumping
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float fallMultiplier;
    private bool _doJump;
    private bool _isGrounded;
    private bool _isJumping;

    private float _gravity;
    private float _mass;

    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected static readonly int IsActive = Animator.StringToHash("isActive");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int HorizontalSpeed = Animator.StringToHash("horizontalSpeed");
    private static readonly int VerticalVelocity = Animator.StringToHash("verticalVelocity");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gravity = _rb.gravityScale;
        _mass = _rb.mass;
        _col = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
        SpawnScript = spawn.GetComponent<SpawnManager>();
    }

    protected virtual void FixedUpdate()
    {
        Vector2 velocity = _rb.velocity;
        
        //set x velocity
        velocity.x = _moveDirection * moveSpeed;
        if (carryPlayer != null) velocity.x += carryPlayer.velocity.x ;

        //set y velocity
        if (_doJump)
        {
            _rb.mass = _mass;
            velocity.y = jumpForce; //applies vertical force to player
            _doJump = false;
        }

        _rb.gravityScale = velocity.y switch
        {
            < 0 => _gravity * fallMultiplier,
            > 0 when !_isJumping => _gravity * lowJumpMultiplier,
            _ => _gravity
        };

        _rb.velocity = velocity;
        
        Animator.SetFloat(HorizontalSpeed, Math.Abs(velocity.x));
        Animator.SetFloat(VerticalVelocity, velocity.y);
    }

    //sets horizontal player movement
    protected void Move(float direction)
    {
        _moveDirection = direction;
        if (direction != 0) _spriteRenderer.flipX = direction < 0;
    }

    protected void Jump(float isDown)
    {
        switch (isDown)
        {
            case 1 when _isGrounded:
                _isJumping = true;
                _doJump = true;
                break;
            default:
                _isJumping = false;
                break;
        }
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D col) { CheckDown(col); }
    protected virtual void OnCollisionExit2D(Collision2D other) { CheckDown(other); }

    private void CheckDown(Collision2D col)
    {
        _isGrounded = CheckDirection(Vector2.down, jumpLayer);
        Animator.SetBool(IsGrounded, _isGrounded);
        
        //if colliding with player
        if ((playerLayer.value & (1 << col.transform.gameObject.layer)) > 0) OnPlayer(col);
    }

    private void OnPlayer(Collision2D col)
    {
        //check if collider is below player
        var bounds = _col.bounds;
        bool onPlayer = Physics2D.BoxCastAll(bounds.center, bounds.size, 0f, Vector2.down, .1f, playerLayer)
            .Any(hit => hit.transform == col.transform);
        carryPlayer = onPlayer ? col.gameObject.GetComponent<Rigidbody2D>() : null;
        _rb.mass = onPlayer ? 0 : _mass;
    }

    //checks if the player is touching a wall in the specified direction (used for ground checks and to prevent sticking to walls)
    private bool CheckDirection(Vector2 direction, LayerMask layer)
    {
        var bounds = _col.bounds;
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(bounds.center, bounds.size, 0f, direction, .1f, layer);
        return boxCast.Any(hit => hit.collider.gameObject != gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "Finish":
                Debug.Log("Win!");
                break;
            case "Lethal":
                OnDeath();
                break;
            case "Player":
            case "Clone":
                _rb.mass = _mass;
                break;
        }
    }

    protected virtual void OnDeath() {}
}