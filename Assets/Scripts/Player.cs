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
    protected Rigidbody2D Rb;
    private Collider2D _col;

    public GameObject spawn; //spawn object
    protected SpawnManager SpawnScript;
    
    [SerializeField] private LayerMask environmentLayer; //layer to check with boxcast
    [SerializeField] private LayerMask playerLayer; //layer to check with boxcast

    [SerializeField] protected PhysicsMaterial2D active;
    [SerializeField] protected PhysicsMaterial2D inactive;
    
    [SerializeField] protected float moveSpeed; //horizontal movement speed
    private float _moveDirection;
    [SerializeField] protected float jumpForce; //vertical impulse force for jumping
    private float _mass;

    public Rigidbody2D carryPlayer;
    
    private bool _canMoveRight;
    private bool _canMoveLeft;
    private bool _canJump;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
        SpawnScript = spawn.GetComponent<SpawnManager>();
        Rb.sharedMaterial = active;
        _mass = Rb.mass;
    }

    private void FixedUpdate()
    {
        if ((_canMoveRight && _moveDirection > 0) || (_canMoveLeft && _moveDirection < 0) || _moveDirection == 0)
            Rb.velocity = new Vector2(_moveDirection * moveSpeed, Rb.velocity.y); //sets player velocity

        if (carryPlayer != null)
        {
            carryPlayer.GetComponent<Rigidbody2D>().velocity += Vector2.right * Rb.velocity.x;
        }
    }

    //sets horizontal player movement
    protected void Move(float direction)
    {
        //will not move player if a wall is in that direction (prevents sticking to walls)
        _moveDirection = direction;
    }

    protected void Jump(float force)
    {
        if (_canJump)
        {
            Rb.mass = _mass;
            Rb.velocity = new Vector2(Rb.velocity.x, force); //applies vertical force to player
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D col) { CheckAllDirections(col); }
    protected virtual void OnCollisionExit2D(Collision2D other) { CheckAllDirections(other); }

    private void CheckAllDirections(Collision2D col)
    {
        _canMoveRight = !CheckDirection(Vector2.right, environmentLayer);
        _canMoveLeft = !CheckDirection(Vector2.left, environmentLayer);
        _canJump = CheckDirection(Vector2.down, environmentLayer);

        if ((playerLayer.value & (1 << col.transform.gameObject.layer)) > 0)
        {
            bool onPlayer = CheckDirection(Vector2.down, playerLayer);
            col.gameObject.GetComponent<Player>().carryPlayer = onPlayer ? GetComponent<Rigidbody2D>() : null;
            Rb.mass = onPlayer ? 0 : _mass;
        }
    }

    //checks if the player is touching a wall in the specified direction (used for ground checks and to prevent sticking to walls)
    private bool CheckDirection(Vector2 direction, LayerMask layer)
    {
        var bounds = _col.bounds;
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(bounds.center, bounds.size, 0f, direction, .1f, layer);
        return boxCast.Any(hit => hit.collider.gameObject != gameObject);
    }
}