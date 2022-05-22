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
    
    private void PlayRecording()
    {
        if (_currentActions.TryPeek(out Action act))
        {
            if (SpawnManager.LoopTime < act.Time) return;
            Debug.Log($"loop time: {SpawnManager.LoopTime}\naction time: {act.Time}");
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

    public void Reset()
    {
        _hasLeftSpawn = false;
        _currentActions = new Queue<Action>(Actions);
        _rb.simulated = false;
        transform.position = spawn.transform.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Respawn") || _hasLeftSpawn) return;
        
        _hasLeftSpawn = true;
        other.gameObject.GetComponent<SpawnManager>().SpawnNextItemInQueue();
    }
}