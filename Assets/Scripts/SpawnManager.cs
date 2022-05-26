using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject clonePrefab;

    private Animator _animator;
    private static readonly int SpawnTrigger = Animator.StringToHash("spawnTrigger");

    [SerializeField] private int maxCloneCount;
    private Queue<Queue<Action>> _clones;
    private Queue<Queue<Action>> _currentClones;

    private void Awake()
    {
        _clones = new Queue<Queue<Action>>();
        _currentClones = new Queue<Queue<Action>>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SpawnQueue();
    }

    public void Reset()
    {
        //destroy existing player and clones
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Clone").Concat(GameObject.FindGameObjectsWithTag("Player"))) Destroy(p);
        
        //reset queue and spawn
        _currentClones = new Queue<Queue<Action>>(_clones);
        SpawnQueue();
    }

    public void AddToQueue(Queue<Action> actions)
    {
        _clones.Enqueue(new Queue<Action>(actions));
        if (_clones.Count > maxCloneCount) _clones.Dequeue();
    }

    private void SpawnQueue()
    {
        _animator.SetTrigger(SpawnTrigger);
        
        while (_currentClones.Count > 0)
        {
            Queue<Action> recordedActions = _currentClones.Dequeue();
            GameObject g = Instantiate(clonePrefab);
            g.GetComponent<CloneController>().Actions = new Queue<Action>(recordedActions);
        }
        Instantiate(playerPrefab, transform.position, transform.rotation);
    }
}
