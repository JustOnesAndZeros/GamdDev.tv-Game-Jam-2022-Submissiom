using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static List<GameObject> SpawnList;
    private static Queue<GameObject> _spawnQueue;

    private void Awake()
    {
        _spawnQueue = new Queue<GameObject>();
    }

    public static void SpawnRecordings()
    {
        _spawnQueue = new Queue<GameObject>(SpawnList);
        AddNextInQueue();
    }

    private static void AddNextInQueue()
    {
        Transform tr = GameObject.Find("Spawn").transform;
        Instantiate(_spawnQueue.Dequeue(), tr.position, tr.rotation);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (_spawnQueue.Count > 0) AddNextInQueue();
        else GameObject.Find("Player").GetComponent<Rigidbody2D>().simulated = true;
    }
}
