using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField] int movementID;
    [SerializeField] Vector2 direction;

    [Header("Enemy Speed")]
    [SerializeField] float speed = 3;
    [SerializeField] float chargeSpeed = 6;

    [Header("Enemy Shot")]
    [SerializeField] float shotSpeed = 8;
    [SerializeField] float sightRange = 5;
    [SerializeField] GameObject laser;
    [SerializeField] Transform shootPos;
    [SerializeField] float fireRate = 3;
    [SerializeField] bool canShoot;
    [SerializeField] bool specialEnemy;
    [SerializeField] bool aggressive;
    [SerializeField] bool smart;

    [Header("Enemy Shields")]
    [SerializeField] GameObject shield;
    [SerializeField] bool shieldActivated;

    [Header("Damage")]
    [SerializeField] AudioClip clip;
    [SerializeField] bool isAlive = true;

    float changeDirTime = 1;
    float aimingSpeed = 6;
    bool playerDetectedBehind;
    Quaternion originalRot;

    Animator anim;
    AudioSource aud;
    Player player;

    // Start is called before the first frame update
    void Start()
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

        originalRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (movementID == 0)
        {
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
    }

    private void FixedUpdate()
    {
        DetectPowerup();
    }

    void EnemyShoot()
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

    void EnemyBounds()
    {
        if (transform.position.y < -7)
        {
            float rand = Random.Range(-9, 9);
            transform.position = new Vector3(rand, 8, 0);
            transform.rotation = originalRot;
        }
    }

    void DetectPowerup()
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

    void Destroy()
    {
        anim.SetTrigger("Explode");
        aud.PlayOneShot(clip);
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 2.4f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                if (shieldActivated)
                {
                    shield.SetActive(false);
                    shieldActivated = false;
                    return;
                }

                player.TakeDamage();
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
