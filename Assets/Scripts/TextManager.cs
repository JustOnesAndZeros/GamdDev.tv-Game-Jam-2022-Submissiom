using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelIndicator;
    [SerializeField] private TextMeshProUGUI cloneIndicator;

    private void Start()
    {
        SetLevelIndicator();
        SetCloneIndicator();
    }

    private void SetLevelIndicator()
    {
        string levelName = SceneManager.GetActiveScene().name;
        if (levelName == "level_end") levelIndicator.text = "End";
        else
        {
            levelIndicator.text = "Level " + levelName[6..];
        }
    }

    private void SetCloneIndicator()
    {
        int cloneCount = GameObject.FindGameObjectWithTag("Respawn").GetComponent<SpawnManager>().maxCloneCount;
        cloneIndicator.text = "x" + (cloneCount + 1);
    }
}
