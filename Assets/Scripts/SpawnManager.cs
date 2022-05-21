using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Queue<GameObject> _recordings;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerPlayback;

    private void Awake()
    {
        _recordings = new Queue<GameObject>();
    }

    public void AddRecording(Queue<PlayerController.Action> actions)
    {
        GameObject rec = playerPlayback;
        rec.GetComponent<PlayerReplay>().Actions = actions;
        _recordings.Enqueue(rec);
    }
    
    public void AddNextItemInQueue()
    {
        if (_recordings.Count > 0) Instantiate(_recordings.Dequeue(), transform.position, transform.rotation);
        else
        {
            player.GetComponent<Rigidbody2D>().simulated = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AddNextItemInQueue();
    }
}
