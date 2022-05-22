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
    protected Rigidbody2D _rb;
    private Collider2D _col;

    public GameObject spawn; //spawn object
    
    [SerializeField] private LayerMask environmentLayer; //layer to check with boxcast
    
    [SerializeField] protected float moveSpeed; //horizontal movement speed
    [SerializeField] protected float jumpForce; //vertical impulse force for jumping

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
    }

    //sets horizontal player movement
    protected void Move(float direction)
    {
        //will not move player if a wall is in that direction (prevents sticking to walls)
        if (!CheckDirection(Vector2.right * direction, environmentLayer)) _rb.velocity = new Vector2(direction * moveSpeed, _rb.velocity.y); //sets player velocity
    }

    protected void Jump(float force)
    {
        //checks if player is touching the ground before jumping
        if (CheckDirection(Vector2.down, environmentLayer)) _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse); //applies vertical force to player
    }

    //checks if the player is touching a wall in the specified direction (used for ground checks and to prevent sticking to walls)
    private bool CheckDirection(Vector2 direction, LayerMask layer)
    {
        var bounds = _col.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, .1f, layer);
    }
}