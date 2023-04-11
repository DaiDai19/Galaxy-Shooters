using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IEnemy
{
    public enum BossStates
    {
        INTRO,
        NORMAL,
        FURIOUS,
        END,
    };

    [SerializeField] private BossStates state;
    [SerializeField] private float maxHealth = 50;
    [SerializeField] private float moveInSpeed = 3;
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float fireRate = 0.4f;
    [SerializeField] private Transform[] shootPositions;
    [SerializeField] private GameObject laserShot;
    [SerializeField] private GameObject shield;
    [SerializeField] private bool shieldActivated = false;

    private float currentHealth = 0;
    private float currentSpeed = 0;
    private float currentFireRate = 0;

    private void EnterState(BossStates newState)
    {
        if (newState == state) return;

        state = newState;
        OnStateUpdate(state);
    }

    private void OnStateUpdate(BossStates newState)
    {
        switch (newState)
        {
            case BossStates.INTRO:

                currentSpeed = moveInSpeed;
                break;

            case BossStates.NORMAL:

                currentSpeed = moveSpeed;
                break;
                    
            case BossStates.FURIOUS:
                break;
                
            case BossStates.END:
                break;
        }
    }

    public void EnemyShoot()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(Collider2D target)
    {
        throw new System.NotImplementedException();
    }
}
