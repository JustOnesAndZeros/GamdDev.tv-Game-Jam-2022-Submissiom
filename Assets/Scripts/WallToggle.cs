using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class WallToggle : MonoBehaviour
{
    [SerializeField] private bool startOn;
    private bool _isOn;

    [SerializeField] private int minButtonCount = 1;
    private int _buttonCount;

    private TilemapCollider2D _col;
    private TilemapRenderer _tilemapRenderer;

    private void Awake()
    {
        _col = GetComponent<TilemapCollider2D>();
        _tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    private void Start()
    {
        _isOn = startOn;
        SetCollider();
    }

    public void Toggle(int buttonStatus)
    {
        _buttonCount += buttonStatus;
        _isOn = _buttonCount > minButtonCount-1 ^ startOn;
        SetCollider();
    }

    private void SetCollider()
    {
        _col.enabled = _isOn;
        _tilemapRenderer.enabled = _isOn;
    }
}
