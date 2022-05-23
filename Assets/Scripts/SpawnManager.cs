using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject clonePrefab;

    public float timePassed;

    private Queue<Queue<Action>> _clones;
    private Queue<Queue<Action>> _currentClones;

    private void Awake()
    {
        _clones = new Queue<Queue<Action>>();
        _currentClones = new Queue<Queue<Action>>();
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
    }

    public void Reset()
    {
        timePassed = 0;
        _currentClones = new Queue<Queue<Action>>(_clones);
        SpawnNextInQueue();
    }

    public void AddToQueue(Queue<Action> actions)
    {
        _clones.Enqueue(new Queue<Action>(actions));
    }

    public void SpawnNextInQueue()
    {
        if (_currentClones.TryDequeue(out Queue<Action> recordedActions))
        {
            GameObject g = Instantiate(clonePrefab);
            g.GetComponent<CloneController>().Actions = new Queue<Action>(recordedActions);
            g.GetComponent<CloneController>().spawn = gameObject;
            g.GetComponent<CloneController>().Reset();
        }
        else
        {
            player.GetComponent<PlayerController>().SetControllable(true);
        }
    }
}
