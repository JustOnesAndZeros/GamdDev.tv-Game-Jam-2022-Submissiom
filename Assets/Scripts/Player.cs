using System;
using System.Linq;
using UnityEditor.SceneManagement;
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
    
    [SerializeField] private LayerMask environmentLayer; //layer to check with boxcast
    [SerializeField] private LayerMask playerLayer; //layer to check with boxcast
    [SerializeField] private LayerMask jumpLayer;
    
    [SerializeField] protected float moveSpeed; //horizontal movement speed
    private float _moveDirection;
    [SerializeField] protected float jumpForce; //vertical impulse force for jumping
    private float _mass;
    
    private bool _isGrounded;

    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected static readonly int IsActive = Animator.StringToHash("isActive");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int HorizontalSpeed = Animator.StringToHash("horizontalSpeed");
    private static readonly int VerticalVelocity = Animator.StringToHash("verticalVelocity");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
        SpawnScript = spawn.GetComponent<SpawnManager>();
        _mass = _rb.mass;
    }

    protected virtual void FixedUpdate()
    {
        Vector2 velocity = _rb.velocity;
        
        velocity.x = _moveDirection * moveSpeed; //sets player velocity

        if (carryPlayer != null) velocity.x += carryPlayer.velocity.x;
        
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

    protected void Jump(float force)
    {
        if (_isGrounded)
        {
            _rb.mass = _mass;
            _rb.velocity = Vector2.up * force; //applies vertical force to player
        }
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D col) { CheckAllDirections(col); }
    protected virtual void OnCollisionExit2D(Collision2D other) { CheckAllDirections(other); }

    private void CheckAllDirections(Collision2D col)
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
        if (col.CompareTag("Finish")) Debug.Log("Win!");
        
        if (col.gameObject.CompareTag("Lethal")) OnDeath();
    }

    protected virtual void OnDeath() {}
}