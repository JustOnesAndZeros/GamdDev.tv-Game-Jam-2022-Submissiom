using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReplay : MonoBehaviour
{
    [SerializeField] private LayerMask environment;
    private Rigidbody2D _rb;
    private Collider2D _col;

    private Queue<float[]> _actions;
    private float _timePassed;
    private float[] _act;
    private float _velocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();

        //copies recorded actions to replay and clears recording on player
        _actions = new Queue<float[]>(PlayerController.RecordedActions);
        PlayerController.RecordedActions.Clear();
        Debug.Log(_actions.Peek());
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        if (!(_timePassed >= _actions.Peek()[0])) return;
        
        _act = _actions.Dequeue();
        switch (_act[1])
        {
            case 0:
                _velocity = _act[2];
                break;
            case 1:
                OnJump(new InputAction.CallbackContext());
                break;
        }
    }

    private void FixedUpdate()
    {
        if (CheckDirection(_velocity > 0 ? Vector2.right : Vector2.left)) return; //will not move player if a wall is in that direction (prevents sticking to walls)
        _rb.velocity = new Vector2(_velocity, _rb.velocity.y);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!CheckDirection(Vector2.down)) return; //checks if player is touching the ground before jumping
        _rb.AddForce(Vector2.up * _act[2], ForceMode2D.Impulse); //applies vertical force to player
    }

    //checks if the player is touching a wall in the specified direction
    //used for ground checks and to prevent sticking to walls
    private bool CheckDirection(Vector2 direction)
    {
        var bounds = _col.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, .1f, environment);
    }
}
