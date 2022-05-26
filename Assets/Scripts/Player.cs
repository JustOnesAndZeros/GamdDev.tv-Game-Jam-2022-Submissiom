using System;
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
    private bool _onPlayer;
    
    [SerializeField] private float jumpForce; //vertical impulse force for jumping
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float fallMultiplier;
    private bool _doJump;
    private bool _isGrounded;
    private bool _isJumping;

    protected float TimePassed;
    
    private float _gravity;
    private float _mass;

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
        
        Animator = GetComponent<Animator>();
        
        spawn = GameObject.FindGameObjectWithTag("Respawn");
        SpawnScript = spawn.GetComponent<SpawnManager>();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
    }

    protected virtual void FixedUpdate()
    {
        Vector2 velocity = _rb.velocity;
        
        //set x velocity
        velocity.x = _moveDirection * moveSpeed;
        if (_onPlayer) velocity.x += carryPlayer.velocity.x;

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
        if (direction != 0) transform.eulerAngles = 180 * (direction < 0 ? Vector3.up : Vector3.zero);
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
        _isGrounded = DownCast(jumpLayer);
        Animator.SetBool(IsGrounded, _isGrounded);
        
        //if colliding with player
        if ((playerLayer.value & (1 << col.transform.gameObject.layer)) > 0) OnPlayer(col);
    }

    private bool DownCast(LayerMask layerMask)
    {
        Bounds bounds = _col.bounds;
        Vector2 pos = new Vector2(bounds.center.x, bounds.min.y - .05f);
        Vector2 size = new Vector2(bounds.size.x, .05f);
        
        return Physics2D.BoxCast(pos, size, 0, Vector2.down, 0f, layerMask);
    }

    private void OnPlayer(Collision2D col)
    {
        _onPlayer = DownCast(playerLayer);
        carryPlayer = _onPlayer ? col.gameObject.GetComponent<Rigidbody2D>() : null;
        _rb.mass = _onPlayer ? 0 : _mass;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "Finish":
                Debug.Log("Win!");
                break;
            case "Lethal":
                gameObject.GetComponent<Player>().OnDeath();
                break;
        }
    }

    protected virtual void OnDeath() {}
}