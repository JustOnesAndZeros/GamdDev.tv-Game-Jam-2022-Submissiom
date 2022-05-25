using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject clonePrefab;

    public float timePassed;
    public float spawnRange;

    [SerializeField] private int maxCloneCount;
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

    private void Start()
    {
        player = Instantiate(player);
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
        if (_clones.Count > maxCloneCount) _clones.Dequeue();
    }

    public void SpawnNextInQueue()
    {
        if (_currentClones.TryDequeue(out Queue<Action> recordedActions))
        {
            GameObject g = Instantiate(clonePrefab);
            CloneController cloneScript = g.GetComponent<CloneController>();
            cloneScript.Actions = new Queue<Action>(recordedActions);
            cloneScript.spawn = gameObject;
            cloneScript.Reset();
        }
        else
        {
            player.GetComponent<PlayerController>().SetControllable(true);
        }
    }
}
