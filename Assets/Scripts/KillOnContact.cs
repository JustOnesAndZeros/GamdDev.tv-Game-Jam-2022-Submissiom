using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnContact : MonoBehaviour
{
    public delegate void Death(GameObject player);
    public static event Death OnDeath;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        OnDeath?.Invoke(col.gameObject); //triggers player death event
    }
}
