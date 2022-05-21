using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _col;
    private PlayerInputActions _playerInputActions;
    
    [SerializeField] public GameObject spawn; //spawn object
    [SerializeField] private LayerMask environmentLayer; //layer to check with boxcast
    [SerializeField] private LayerMask playerLayer; //layer to check with boxcast
    [Space]
    [SerializeField] public bool isPlayback;
    [SerializeField] private float moveSpeed; //horizontal movement speed
    [SerializeField] private float jumpForce; //vertical impulse force for jumping

    private float _velocity;
    
    public Queue<Action> RecordedActions;
    private bool _leftSpawn;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        
        RecordedActions = new Queue<Action>();
    }
    
    //enable user input and subscribe to events
    private void OnEnable()
    {
        KillOnContact.OnDeath += OnDeath;

        if (isPlayback) return;
        
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Movement.Horizontal.started += OnMove;
        _playerInputActions.Movement.Horizontal.canceled += OnMove;
        
        _playerInputActions.Movement.Jump.started += OnJump;

        _playerInputActions.Enable();
    }

    //unsubscribes from events
    private void OnDisable()
    {
        KillOnContact.OnDeath -= OnDeath;
        
        if (isPlayback) return;
        
        _playerInputActions.Movement.Horizontal.started -= OnMove;
        _playerInputActions.Movement.Horizontal.canceled -= OnMove;
        
        _playerInputActions.Movement.Jump.started -= OnJump;
        
        _playerInputActions.Disable();
    }

    private void Start()
    {
        transform.position = spawn.transform.position;
    }

    private void Update()
    {
        if (isPlayback) PlayRecording();
    }

    private void PlayRecording()
    {
        var act = RecordedActions.Peek();
        if (!(SpawnManager.LoopTime >= act.Time)) return;

        switch (act.ActionType)
        {
            case 0:
                Move(act.Value);
                break;
            case 1:
                Jump();
                break;
        }
        
        RecordedActions.Dequeue();
    }

    private void FixedUpdate()
    {
        //will not move player if a wall is in that direction (prevents sticking to walls)
        if (!CheckDirection(_velocity > 0 ? Vector2.right : Vector2.left, environmentLayer)) _rb.velocity = new Vector2(_velocity, _rb.velocity.y);
    }

    private void OnMove(InputAction.CallbackContext ctx) { Move(ctx.ReadValue<float>()); }

    //sets horizontal player movement
    private void Move(float direction)
    {
        _velocity = direction * moveSpeed; //sets player velocity
        RecordedActions.Enqueue(new Action(SpawnManager.LoopTime, 0, direction)); //records new direction and time to be replayed next loop
    }

    private void OnJump(InputAction.CallbackContext ctx) { Jump(); }

    private void Jump()
    {
        if (!(CheckDirection(Vector2.down, environmentLayer) || CheckDirection(Vector2.down, playerLayer))) return; //checks if player is touching the ground before jumping
        
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //applies vertical force to player
        RecordedActions.Enqueue(new Action(SpawnManager.LoopTime, 1, jumpForce)); //records jump force and time to be replayed next loop
    }

    //checks if the player is touching a wall in the specified direction (used for ground checks and to prevent sticking to walls)
    private bool CheckDirection(Vector2 direction, LayerMask layer)
    {
        var bounds = _col.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, .1f, layer);
    }

    //executed when the player collides with a lethal object
    private void OnDeath(GameObject player)
    {
        if (!player.CompareTag("Player")) return;
        
        //destroy all recordings
        foreach (GameObject rec in GameObject.FindGameObjectsWithTag("Recording")) Destroy(rec);
        
        //hide player and move to spawn
        _rb.simulated = false;
        GetComponent<SpriteRenderer>().enabled = false;
        transform.position = spawn.transform.position;
        
        //add recording to spawn queue
        RecordedActions.Enqueue(new Action(SpawnManager.LoopTime, 0, 0f)); //stop horizontal movement
        spawn.GetComponent<SpawnManager>().AddRecording(RecordedActions); //add to queue

        //clear recording queue
        RecordedActions = new Queue<Action>();
        SpawnManager.LoopTime = 0;
        
        spawn.GetComponent<SpawnManager>().SpawnQueue(); //start spawning in queue
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Respawn") || _leftSpawn) return;
        other.GetComponent<SpawnManager>().AddNextItemInQueue();
        _leftSpawn = true;
    }
}


