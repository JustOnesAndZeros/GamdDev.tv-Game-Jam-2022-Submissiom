using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static float LoopTime;
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerClone;
    private GameObject _currentPlayer;

    private Queue<GameObject> _spawnQueue;
    private Queue<GameObject> _currentSpawnQueue;

    private void Awake()
    {
        _spawnQueue = new Queue<GameObject>();
        _currentSpawnQueue = new Queue<GameObject>();
    }

    private void Start()
    {
        _currentPlayer = Instantiate(player);
        _currentPlayer.GetComponent<PlayerController>().spawn = gameObject;
        _currentPlayer.SetActive(true);
    }

    private void Update()
    {
        LoopTime += Time.deltaTime;
    }

    public void Reset()
    {
        LoopTime = 0;
        _currentSpawnQueue = new Queue<GameObject>(_spawnQueue);
    }

    public void AddToQueue(IEnumerable<Action> actions)
    {
        GameObject clone = Instantiate(playerClone);
        clone.GetComponent<CloneController>().Actions = new Queue<Action>(actions);
        clone.GetComponent<CloneController>().spawn = gameObject;
        clone.GetComponent<CloneController>().Reset();
        _spawnQueue.Enqueue(clone);
    }

    public void SpawnNextItemInQueue()
    {
        if (_currentSpawnQueue.TryDequeue(out GameObject p))
        {
            p.GetComponent<Rigidbody2D>().simulated = true;
            p.GetComponent<CloneController>().enabled = true;
        }
        else
        {
            _currentPlayer.GetComponent<SpriteRenderer>().enabled = true;
            _currentPlayer.GetComponent<Rigidbody2D>().simulated = true;
        }
    }
}
