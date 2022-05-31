using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroyTimer : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] timerObject = GameObject.FindGameObjectsWithTag("Timer");
        if (timerObject.Length > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (SceneManager.GetActiveScene().name == "level_end")
        {
            TextMeshProUGUI textMesh = GameObject.FindGameObjectWithTag("EndMessage").GetComponent<TextMeshProUGUI>();
            TimerManager timer = GetComponentInChildren<TimerManager>();
            textMesh.text = $"You Win!\nYour Time : {timer.minutes:00}:{timer.seconds:00}:{timer.milliseconds:00}";
            timer.Hide();
        }
    }
}
