using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    
    private float _timePassed;
    public float milliseconds;
    public float seconds;
    public float minutes;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        milliseconds = (int) ((_timePassed - Math.Truncate(_timePassed)) * 100);
        seconds = _timePassed % 60;
        minutes = _timePassed / 60 % 60;

        _textMesh.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
