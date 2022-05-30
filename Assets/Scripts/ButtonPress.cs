using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    private Animator _animator;
    private static readonly int IsDown = Animator.StringToHash("isDown");

    private GameObject[] _walls;

    private int _triggerCount;
    private bool _isDown;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _walls = GameObject.FindGameObjectsWithTag("Toggle");

        _isDown = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Clone") || col.CompareTag("Player"))
        {
            _triggerCount++;
            ToggleWalls();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Clone") || other.CompareTag("Player"))
        {
            _triggerCount--;
            ToggleWalls();
        }
    }

    private void ToggleWalls()
    {
        if ((_triggerCount > 0 && !_isDown) || (_triggerCount == 0 && _isDown))
        {
            _isDown = !_isDown;
            _animator.SetBool(IsDown, _isDown);
            
            foreach (var wall in _walls)
            {
                wall.GetComponent<WallToggle>().Toggle(_isDown ? 1 : -1);
            }
        }
    }
}
