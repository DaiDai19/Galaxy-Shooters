using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private float moveInSpeed = 3;
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float fireRate = 0.4f;
    [SerializeField] private Transform[] shootPositions;
    [SerializeField] private GameObject laserShot;
    [SerializeField] private GameObject shield;
    [SerializeField] private bool shieldActivated = false;
    [SerializeField] private Bounds bounds;

    public event Action<int, int> OnHealthUpdate;

    private int currentHealth = 0;
    private float currentSpeed = 0;
    private float currentFireRate = 0;
    private bool moveRight;

    public Vector3 CurrentPosition => transform.position;

    private void Start()
    {
        EnterState(BossStates.INTRO);
    }

    private void FixedUpdate()
    {
        OnStateUpdate(state);
    }

    private void EnterState(BossStates newState)
    {
        if (newState == state) return;

        state = newState;
    }

    private void OnStateUpdate(BossStates newState)
    {
        switch (newState)
        {
            case BossStates.INTRO:

                currentSpeed = moveInSpeed;
                currentHealth = maxHealth;

                if (Vector3.Distance(transform.position, new Vector3(0, 3.5f, 0)) > 0.1f)
                {
                    transform.Translate(Vector2.down * currentSpeed * Time.deltaTime);
                }

                else
                {
                    EnterState(BossStates.NORMAL);
                }
                break;

            case BossStates.NORMAL:

                currentSpeed = moveSpeed;
                var moveDirection = moveRight ? Vector2.right : Vector2.left;
                transform.Translate(moveDirection * currentSpeed * Time.deltaTime);

                if (transform.position.x <= bounds.minX)
                {
                    moveRight = true;
                }

                else if (transform.position.x >= bounds.maxX)
                {
                    moveRight = false;
                }

                EnemyShoot();
                break;
                    
            case BossStates.FURIOUS:
                break;
                
            case BossStates.END:
                break;
        }
    }

    public void EnemyShoot()
    {
        if (Time.time >= currentFireRate)
        {
            for (int i = 0; i < shootPositions.Length; i++)
            {
                GameObject laser = Instantiate(laserShot, shootPositions[i].position, Quaternion.identity);

                if (laser.GetComponent<Laser>() is IProjectile projectile)
                {
                    projectile.AssignLaser();
                    projectile.ShotDirection(Vector2.up);
                }
            }

            currentFireRate = Time.time + fireRate;
        }
    }

    public void TakeDamage(Collider2D target)
    {
        if (shieldActivated)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - 1);
        OnHealthUpdate?.Invoke(currentHealth, maxHealth);

        if (currentHealth == maxHealth / 2)
        {
            EnterState(BossStates.FURIOUS);
        }

        if(currentHealth <= 0)
        {
            EnterState(BossStates.END);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IProjectile>() is IProjectile projectile)
        {
            if (projectile.EnemyLaser())
            {
                return;
            }

            TakeDamage(collision);
        }
    }
}
