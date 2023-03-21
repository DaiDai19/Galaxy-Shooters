using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField] private int movementID;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float dodgeTime;

    [Header("Enemy Bounds")]
    [SerializeField] private Bounds bounds;

    [Header("Enemy Speed")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float chargeSpeed = 6;
    [SerializeField] private float dodgeSpeed = 6;

    [Header("Enemy Shot")]
    [SerializeField] private float shotSpeed = 8;
    [SerializeField] private float sightRange = 5;
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform shootPos;
    [SerializeField] private float fireRate = 3;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool canDodge;
    [SerializeField] private bool specialEnemy;
    [SerializeField] private bool aggressive;
    [SerializeField] private bool smart;

    [Header("Enemy Shields")]
    [SerializeField] private GameObject shield;
    [SerializeField] private bool shieldActivated;

    [Header("Damage")]
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool isAlive = true;

    private float changeDirTime = 1;
    private float aimingSpeed = 6;
    private float curDodgeTime;
    private int randomDirection;
    private bool playerDetectedBehind;
    private bool dodging;
    private Vector2 dodgeDistance;
    private Quaternion originalRot;

    private Animator anim;
    private AudioSource aud;
    private Player player;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("Player does not exist.");
        }

        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError("Animator does not exist.");
        }

        aud = GetComponent<AudioSource>();

        if (aud == null)
        {
            Debug.LogError("Audio Source does not exist");
        }

        if (specialEnemy)
        {
            movementID = Random.Range(0, 2);

            if (shield != null)
            {
                shieldActivated = movementID > 0;
                shield.SetActive(shieldActivated);
            }
        }

        if (canDodge)
        {
            randomDirection = Random.Range(-3, 3);
        }

        originalRot = transform.rotation;
        curDodgeTime = dodgeTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (movementID == 0)
        {
            if (dodging) return;

            if (aggressive && Vector2.Distance(transform.position, player.transform.position) < 3)
            {
                Vector3 moveDirection = (transform.position - player.transform.position).normalized;
                Quaternion toRotation = Quaternion.LookRotation(transform.forward, moveDirection);

                if (transform.rotation == toRotation)
                {
                    transform.Translate(Vector2.down * chargeSpeed * Time.deltaTime);
                    return;
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * aimingSpeed);

                return;
            }

            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }

        else
        {
            if (dodging) return;

            transform.Translate(direction * speed * Time.deltaTime);
            changeDirTime -= Time.deltaTime;

            if (changeDirTime <= 0)
            {
                direction.x = -direction.x;
                changeDirTime = 1;
            }
        }

        if (smart)
        {
            if (player != null)
            {
                Vector2 target = (transform.position - player.transform.position).normalized;

                if (target.y > transform.position.y)
                {
                    playerDetectedBehind = true;
                }

                else
                {
                    playerDetectedBehind = false;
                }
            }
        }

        EnemyBounds();

        if (canShoot)
        {
            EnemyShoot();
        }

        if (DetectLaser())
        {
            StartCoroutine(Dodge());
        }
    }

    private void FixedUpdate()
    {
        DetectPowerup();
    }

    private void EnemyShoot()
    {
        if(fireRate < 0)
        {
            Vector2 shotDirection = Vector2.up;

            fireRate = Random.Range(3, 7);
            GameObject curShot = Instantiate(this.laser, shootPos.position, Quaternion.identity);
            Laser laser = curShot.GetComponentInChildren<Laser>();

            laser.AssignLaser();

            if (playerDetectedBehind)
            {
                laser.ShotDirection(-shotDirection);
                return;
            }

            laser.ShotDirection(shotDirection);
        }

        fireRate -= Time.deltaTime;
    }

    private void EnemyBounds()
    {
        if (transform.position.y < bounds.minY)
        {
            float rand = Random.Range(-9, 9);
            transform.position = new Vector3(rand, 8, 0);
            transform.rotation = originalRot;
        }
    }

    private void DetectPowerup()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, sightRange);

        if (hit && hit.collider.GetComponent<PowerUp>())
        {
            Vector2 shotDirection = Vector2.up;
            GameObject curShot = Instantiate(this.laser, shootPos.position, Quaternion.identity);
            Laser laser = curShot.GetComponentInChildren<Laser>();

            laser.AssignLaser();

            if (playerDetectedBehind)
            {
                laser.ShotDirection(-shotDirection);
                return;
            }

            laser.ShotDirection(shotDirection);
        }
    }

    private bool DetectLaser()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 5, Vector2.down);

        if (dodging) return false;

        if (hit.collider.GetComponent<Laser>() && !hit.collider.GetComponent<Laser>().EnemyLaser())
        {
            return true;
        }

        return false;
    }

    private IEnumerator Dodge()
    {
        int randDirection = Random.Range(-3, 3);
        int randChance = Random.Range(0, 10);

        while (curDodgeTime > 1 && randChance >= 7)
        {
            curDodgeTime -= Time.deltaTime;
            transform.Translate(Vector2.right * randDirection * dodgeSpeed * Time.deltaTime);
            dodging = true;

            if (transform.position.x > bounds.maxX)
            {
                transform.position = new Vector3(bounds.minX, transform.position.y, transform.position.z);
            }

            if (transform.position.x < bounds.minX)
            {
                transform.position = new Vector3(bounds.maxX, transform.position.y, transform.position.z);
            }

            yield return null;
        }

        curDodgeTime = dodgeTime;
        dodging = false;
    }

    private void Destroy()
    {
        anim.SetTrigger("Explode");
        aud.PlayOneShot(clip);
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 2.4f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerLives>())
        {
            PlayerLives playerLives = other.GetComponent<PlayerLives>();

            if (playerLives != null)
            {
                if (shieldActivated)
                {
                    shield.SetActive(false);
                    shieldActivated = false;
                    return;
                }

                playerLives.TakeDamage();
                Destroy();
            }
        }

        if (other.GetComponent<Laser>() && !other.GetComponent<Laser>().EnemyLaser())
        {
            if (shieldActivated)
            {
                shield.SetActive(false);
                shieldActivated = false;
                return;
            }

            Destroy(other.gameObject);
            player.IncreaseScore(Random.Range(10, 15));
            isAlive = false;
            Destroy();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * 5);
    }
}
