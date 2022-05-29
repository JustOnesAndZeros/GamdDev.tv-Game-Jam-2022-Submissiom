using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject clonePrefab;

    public CinemachineTargetGroup targetGroup;
    [SerializeField] public float targetGroupRadius;

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
        targetGroup = GameObject.FindGameObjectWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SpawnQueue();
    }

    public void Reset()
    {
        //destroy existing player and clones
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player").Concat(GameObject.FindGameObjectsWithTag("Clone")))
        {
            targetGroup.RemoveMember(p.transform);
            Destroy(p);
        }
        
        //reset queue and spawn
        _currentClones = new Queue<Queue<Action>>(_clones);
        SpawnQueue();
    }

    public void AddToQueue(Queue<Action> actions)
    {
        _clones.Enqueue(new Queue<Action>(actions));
        if (_clones.Count > maxCloneCount) _clones.Dequeue();
        Reset();
    }

    private void SpawnQueue()
    {
        _animator.SetTrigger(SpawnTrigger);
        
        GameObject player = Instantiate(playerPrefab);
        targetGroup.AddMember(player.transform, 1f, targetGroupRadius);
        
        while (_currentClones.Count > 0)
        {
            Queue<Action> recordedActions = _currentClones.Dequeue();
            GameObject clone = Instantiate(clonePrefab);
            clone.GetComponent<CloneController>().Actions = new Queue<Action>(recordedActions);
            targetGroup.AddMember(clone.transform, 1f, targetGroupRadius);
        }
    }

    public void ResetLevel()
    {
        _clones.Clear();
        Reset();
    }
}
