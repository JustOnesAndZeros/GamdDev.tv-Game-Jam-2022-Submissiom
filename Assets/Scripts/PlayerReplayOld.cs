using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayOld : MonoBehaviour
{
    [SerializeField] private LayerMask environment;
    private Rigidbody2D _rb;
    private Collider2D _col;

    public Queue<PlayerController.Action> Actions;
    private float _timePassed;
    private float _velocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();

        //copies recorded actions to replay and clears recording on player
        Actions = new Queue<PlayerController.Action>(PlayerController.RecordedActions);
        PlayerController.RecordedActions.Clear();
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        
        if (!Actions.TryPeek(out PlayerController.Action act)) return;
        if (!(_timePassed >= act.Time)) return;
        
        switch (act.ActionType) //check action type
        {
            case 0:
                _velocity = act.Value; //set velocity
                break;
            case 1:
                Jump(act); //make player clone jump
                break;
        }
        
        Actions.Dequeue(); //remove action from front of queue
    }

    private void FixedUpdate()
    {
        if (CheckDirection(_velocity > 0 ? Vector2.right : Vector2.left)) return; //will not move player if a wall is in that direction (prevents sticking to walls)
        _rb.velocity = new Vector2(_velocity, _rb.velocity.y);
    }

    private void Jump(PlayerController.Action act)
    {
        //checks if player clone is touching the ground before jumping
        if (CheckDirection(Vector2.down)) _rb.AddForce(Vector2.up * act.Value, ForceMode2D.Impulse); //applies vertical force to player
    }

    //checks if the player is touching a wall in the specified direction
    //used for ground checks and to prevent sticking to walls
    private bool CheckDirection(Vector2 direction)
    {
        var bounds = _col.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, .1f, environment);
    }
}
