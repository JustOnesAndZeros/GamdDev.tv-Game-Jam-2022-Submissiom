using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Queue<Queue<PlayerController.Action>> _recordings;
    private Queue<Queue<PlayerController.Action>> _loopRecordings;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerPlayback;

    private void Awake()
    {
        _recordings = new Queue<Queue<PlayerController.Action>>();
        _loopRecordings = new Queue<Queue<PlayerController.Action>>();
    }

    public void AddRecording(Queue<PlayerController.Action> actions)
    {
        _recordings.Enqueue(actions); //add action queue to spawn queue
    }

    public void SpawnQueue()
    {
        _loopRecordings = new Queue<Queue<PlayerController.Action>>(_recordings);
        AddNextItemInQueue();
    }

    private void AddNextItemInQueue()
    {
        if (_loopRecordings.Count <= 0) return;
        Transform tr = transform;
        GameObject rec = Instantiate(playerPlayback, tr.position, tr.rotation);
        rec.GetComponent<PlayerReplay>().Actions = _loopRecordings.Dequeue();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Recording")) AddNextItemInQueue();
        else
        {
            player.GetComponent<Rigidbody2D>().simulated = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
