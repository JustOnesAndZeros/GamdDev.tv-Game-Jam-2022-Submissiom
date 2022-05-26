using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerController _playerController;
    [SerializeField] private GameObject clonePrefab;

    private Animator _animator;
    private static readonly int SpawnTrigger = Animator.StringToHash("spawnTrigger");
    
    //private Vector3 _spawnPosition;
    public float spawnRange;
    
    [SerializeField] private int maxCloneCount;
    private Queue<Queue<Action>> _clones;
    private Queue<Queue<Action>> _currentClones;
    private Queue<float> _spawnTimes;
    private Queue<float> _currentSpawnTimes;

    private void Awake()
    {
        _clones = new Queue<Queue<Action>>();
        _currentClones = new Queue<Queue<Action>>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetTrigger(SpawnTrigger);
        player = Instantiate(player);
        _playerController = player.GetComponent<PlayerController>();
    }

    public void Reset()
    {
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
            _animator.SetTrigger(SpawnTrigger);
            GameObject g = Instantiate(clonePrefab);
            CloneController cloneScript = g.GetComponent<CloneController>();
            cloneScript.Actions = new Queue<Action>(recordedActions);
            cloneScript.spawn = gameObject;
        }
        else
        {
            _animator.SetTrigger(SpawnTrigger);
            _playerController.SetControllable(true);
        }
    }
}
