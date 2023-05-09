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
    [SerializeField] private int maxShieldHealth = 25;
    [SerializeField] private float moveInSpeed = 3;
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float fireRate = 0.4f;
    [SerializeField] private Transform[] shootPositions;
    [SerializeField] private GameObject laserShot;
    [SerializeField] private GameObject shield;
    [SerializeField] private bool shieldActivated = false;
    [SerializeField] private Bounds bounds;
    [SerializeField] private EnemyUI enemyUI;

    public event Action<int, int> OnHealthUpdate;
    public event Action<int, int> OnShieldHealthUpdate;
    public event Action<bool> OnHealthActivated;
    public event Action<bool> OnShieldActivated;

    private int currentHealth = 0;
    private int currentShieldHealth = 0;
    private float currentSpeed = 0;
    private float currentFireRate = 0;
    private bool moveRight;
    private Vector2 initialPosition;

    public Vector3 CurrentPosition => transform.position;

    private Animator animator;
    private Player player;

    private void Awake()
    {
        enemyUI = FindObjectOfType<EnemyUI>();  
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<Player>();
        enemyUI.SetBossForUI(this);
        initialPosition = transform.position;
    }

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
                currentShieldHealth = maxShieldHealth;
                OnHealthActivated?.Invoke(true);

                if (Vector2.Distance(transform.position, new Vector2(transform.position.x, 3.5f)) > 0.1f)
                {
                    transform.Translate(Vector2.down * currentSpeed * Time.deltaTime);
                }

                else
                {
                    EnterState(BossStates.NORMAL);
                }
                break;

            case BossStates.NORMAL:
                shield.SetActive(false);
                OnShieldActivated?.Invoke(false);
                OnHealthActivated?.Invoke(true);
                currentSpeed = moveSpeed;

                Movement();
                EnemyShoot();
                break;
                    
            case BossStates.FURIOUS:
                shield.SetActive(true);
                OnShieldActivated?.Invoke(true);
                OnHealthActivated?.Invoke(false);
                Movement();
                EnemyShoot();
                break;
                
            case BossStates.END:
                shield.SetActive(false);
                OnShieldActivated?.Invoke(false);
                OnHealthActivated?.Invoke(false);
                break;
        }
    }

    private void Movement()
    {
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
        if (shield.activeInHierarchy)
        {
            currentShieldHealth = Mathf.Max(0, currentShieldHealth - 1);
            OnShieldHealthUpdate?.Invoke(currentShieldHealth, maxShieldHealth);

            if (currentShieldHealth <= 0)
            {
                EnterState(BossStates.NORMAL);
            }

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
            CameraShake.Instance.CameraShakeFinish();
            animator.SetTrigger("Explode");
            Destroy(gameObject, 6);
            player.EndMovement();
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
            Destroy(collision.gameObject);
        }
    }
}
