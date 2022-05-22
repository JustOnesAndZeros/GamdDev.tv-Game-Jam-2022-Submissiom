using System;
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
    }

    private void OnEnable()
    {
        Actions = new Queue<Action>();
        _currentActions = new Queue<Action>();
    }

    public void Reset()
    {
        _currentActions = new Queue<Action>(Actions);
        Debug.Log(Actions.Count);
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
                    Move(0);
                    break;
            }

            _currentActions.Dequeue();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Respawn") || _hasLeftSpawn) return;
        spawn.GetComponent<SpawnManager>().SpawnNextInQueue();
        _hasLeftSpawn = true;
    }
}