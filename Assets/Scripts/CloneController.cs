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

    private void PlayRecording()
    {
        if (_currentActions.TryPeek(out Action act))
        {
            if (SpawnManager.LoopTime < act.Time) return;
            switch (act.ActionType)
            {
                case 0:
                    Move(act.Value);
                    break;
                case 1:
                    Jump(act.Value);
                    break;
            }

            _currentActions.Dequeue();
        }
        else
        {
            Move(0);
        }
    }
}