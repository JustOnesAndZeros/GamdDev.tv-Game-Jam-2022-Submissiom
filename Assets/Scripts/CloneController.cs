using System.Collections.Generic;
using UnityEngine;

public class CloneController : Player
{
    public Queue<Action> Actions;
    private Queue<Action> _currentActions;
    
    private bool _hasLeftSpawn;

    private void Update()
    {
        PlayRecording();

        if (Vector2.Distance(transform.position, spawn.transform.position) > SpawnScript.spawnRange && !_hasLeftSpawn)
        {
            SpawnScript.SpawnNextInQueue();
            _hasLeftSpawn = true;
        }
    }

    private void OnEnable()
    {
        Actions = new Queue<Action>();
        _currentActions = new Queue<Action>();
    }

    public void Reset()
    {
        _currentActions = new Queue<Action>(Actions);
    }

    private void PlayRecording()
    {
        if (_currentActions.TryPeek(out Action act))
        {
            if (SpawnScript.timePassed < act.Time) return;
            switch (act.ActionType)
            {
                case 0:
                    Move(act.Value);
                    break;
                case 1:
                    Jump(act.Value);
                    break;
                default:
                    OnDeath();
                    break;
            }

            _currentActions.Dequeue();
        }
    }

    protected override void OnDeath()
    {
        Move(0);
        Animator.SetBool(IsActive, false);
    }
}