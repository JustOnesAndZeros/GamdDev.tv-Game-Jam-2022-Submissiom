using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] musicObject = GameObject.FindGameObjectsWithTag("Music");
        if (musicObject.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }
}