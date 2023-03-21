using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerLives : MonoBehaviour
{
    [Header("Player Lives")]
    [SerializeField] private int playerLives = 3;

    public event Action<int> OnDamage;
    public event Action<int> OnRecover;
    public event Action OnShieldDamage;
    public event Action OnDeath;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void TakeDamage()
    {
        if (player.ShieldActivated)
        {
            OnShieldDamage?.Invoke();
            return;
        }


        playerLives--;
        OnDamage?.Invoke(playerLives);

        if (playerLives <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        CameraShake.instance.ShakeCamera();
    }


    public void RecoverHealth()
    {
        if (playerLives >= 3)
        {
            return;
        }

        playerLives = Mathf.Min(playerLives + 1, 3);
        OnRecover?.Invoke(playerLives);
        
    }
}
