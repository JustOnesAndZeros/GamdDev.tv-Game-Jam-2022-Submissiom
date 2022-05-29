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
    
    [SerializeField] private LayerMask playerLayer; //layer to check with boxcast
    [SerializeField] private LayerMask jumpLayer;
    
    [SerializeField] protected float moveSpeed; //horizontal movement speed
    [SerializeField] public float moveDirection;
    
    [SerializeField] private float jumpForce; //vertical impulse force for jumping
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float fallMultiplier;
    private bool _doJump;
    private bool _isGrounded;
    private bool _isJumping;

    protected float TimePassed;

    private float _gravity;
    private float _mass;

    private bool _hasTouchedLethal;

    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected static readonly int AnimIsActive = Animator.StringToHash("isActive");
    private static readonly int AnimIsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int AnimOnPlayer = Animator.StringToHash("onPlayer");
    private static readonly int AnimHorizontalSpeed = Animator.StringToHash("horizontalSpeed");
    private static readonly int AnimVerticalVelocity = Animator.StringToHash("verticalVelocity");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gravity = _rb.gravityScale;
        _mass = _rb.mass;
        _col = GetComponent<Collider2D>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        
        spawn = GameObject.FindGameObjectWithTag("Respawn");
        SpawnScript = spawn.GetComponent<SpawnManager>();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
    }

    protected virtual void Update()
    {
        TimePassed += Time.deltaTime;
        
        if (transform.parent) if (Math.Abs(transform.position.x - transform.parent.position.x) > 0.5f) transform.SetParent(null);
        
        Animator.SetFloat(AnimHorizontalSpeed, Math.Abs(_rb.velocity.x));
        Animator.SetFloat(AnimVerticalVelocity, _rb.velocity.y);
        Animator.SetBool(AnimOnPlayer, transform.parent);
    }

    public void SetVelocity(Vector2 addedMovement)
    {
        Vector2 velocity = _rb.velocity;
        
        //set x velocity
        velocity.x = moveDirection * moveSpeed + addedMovement.x;

        //set y velocity
        if (_doJump)
        {
            transform.SetParent(null);
            _rb.mass = _mass;
            velocity.y = jumpForce; //applies vertical force to player
            _doJump = false;
        }
        else if (transform.parent != null)
        {
            _rb.gravityScale = 0;
            velocity.y = addedMovement.y;
        }
        
        _rb.gravityScale = velocity.y switch
        {
            < 0 => _gravity * fallMultiplier,
            > 0 when !_isJumping => _gravity * lowJumpMultiplier,
            _ => _gravity
        };

        _rb.velocity = velocity;

        foreach (Player script in GetComponentsInChildren<Player>())
        {
            if (script.transform.parent == transform) script.SetVelocity(_rb.velocity);
        }
    }

    //sets horizontal player movement
    protected void Move(float direction)
    {
        moveDirection = direction;
        
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
        _isGrounded = BoxCast(jumpLayer);
        Animator.SetBool(AnimIsGrounded, _isGrounded);
        
        //if colliding with player
        if ((col.gameObject.CompareTag("Clone") || col.gameObject.CompareTag("Player")) && _rb.velocity.y <= 0)
        {
            bool onPlayer = false;
            RaycastHit2D hit = BoxCast(playerLayer);
            if (hit) onPlayer = hit.collider.gameObject == col.collider.gameObject;
            
            if (!transform.parent) transform.SetParent(onPlayer ? col.transform : null);
            _rb.mass = onPlayer ? 0 : _mass;
        }
    }

    private RaycastHit2D BoxCast(LayerMask layerMask)
    {
        Bounds bounds = _col.bounds;
        Vector2 pos = new Vector2(bounds.center.x, bounds.min.y - .05f);
        Vector2 size = new Vector2(bounds.size.x, .05f);

        return Physics2D.BoxCast(pos, size, 0, Vector2.down, 0, layerMask);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "Finish":
                Debug.Log("Win!");
                break;
            case "Lethal" when !_hasTouchedLethal:
                gameObject.GetComponent<Player>().OnDeath();
                _hasTouchedLethal = true;
                break;
            case "Player" when _rb.velocity.y < 0:
            case "Clone" when _rb.velocity.y < 0:
                _rb.mass = 0;
                break;
        }
    }
    
    protected virtual void OnDeath() {}
}