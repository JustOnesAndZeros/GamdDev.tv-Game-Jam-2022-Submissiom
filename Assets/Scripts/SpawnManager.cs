using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static float LoopTime;
    
    private Queue<Queue<Action>> _recordings;
    private Queue<Queue<Action>> _loopRecordings;
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerClone;

    private void Awake()
    {
        _recordings = new Queue<Queue<Action>>();
        _loopRecordings = new Queue<Queue<Action>>();
    }

    private void Update()
    {
        LoopTime += Time.deltaTime;
    }

    public void AddRecording(Queue<Action> actions)
    {
        _recordings.Enqueue(actions); //add action queue to spawn queue
    }

    public void SpawnQueue()
    {
        _loopRecordings = new Queue<Queue<Action>>(_recordings);
        AddNextItemInQueue();
    }

    public void AddNextItemInQueue()
    {
        if (_loopRecordings.Count > 0)
        {
            GameObject rec = Instantiate(playerClone);
        
            PlayerController script = rec.GetComponent<PlayerController>();
            script.RecordedActions = new Queue<Action>(_loopRecordings.Dequeue());
            script.spawn = gameObject;
        }
        else
        {
            player.GetComponent<Rigidbody2D>().simulated = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
