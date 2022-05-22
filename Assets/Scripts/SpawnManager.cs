using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject clonePrefab;

    private static Queue<Queue<Action>> _clones;
    private Queue<Queue<Action>> _currentClones;

    public static Queue<float> SpawnTimes;

    public static float LoopTime;

    private void Awake()
    {
        _clones = new Queue<Queue<Action>>();
        _currentClones = new Queue<Queue<Action>>();
        SpawnTimes = new Queue<float>();
        SpawnTimes.Enqueue(0f);
    }

    public void Reset()
    {
        _currentClones = new Queue<Queue<Action>>(_clones);
        LoopTime = 0;
    }

    public void AddToQueue(Queue<Action> actions)
    {
        _clones.Enqueue(new Queue<Action>(actions));
    }

    private void Update()
    {
        if (SpawnTimes.TryPeek(out float t))
        {
            if (LoopTime >= t)
            {
                if (_currentClones.TryDequeue(out Queue<Action> q))
                {
                    GameObject g = Instantiate(clonePrefab);
                    g.GetComponent<CloneController>().Actions = new Queue<Action>(q);
                    g.GetComponent<CloneController>().spawn = gameObject;
                }
                else
                {
                    player.GetComponent<PlayerController>().SetControllable(true);
                }
            }
        }

        LoopTime += Time.deltaTime;
    }
}
