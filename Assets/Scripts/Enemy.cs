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

    [Header("Enemy Shot")]
    [SerializeField] float shotSpeed = 8;
    [SerializeField] GameObject laser;
    [SerializeField] Transform shootPos;
    [SerializeField] float fireRate = 3;
    [SerializeField] bool canShoot;
    [SerializeField] bool specialEnemy;

    [Header("Enenmy Shields")]
    [SerializeField] GameObject shield;
    [SerializeField] bool shieldActivated;

    [Header("Damage")]
    [SerializeField] AudioClip clip;
    [SerializeField] bool isAlive = true;

    float changeDirTime = 1;

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
            shieldActivated = movementID > 0;
            shield.SetActive(shieldActivated);
        }
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

        EnemyBounds();

        if (canShoot)
        {
            EnemyShoot();
        }
    }

    void EnemyShoot()
    {
        if(fireRate < 0)
        { 
            fireRate = Random.Range(3, 7);
            GameObject curShot = Instantiate(laser, shootPos.position, Quaternion.identity);
            Laser[] lasers = curShot.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignLaser();
            }
            return;
        }

        fireRate -= Time.deltaTime;
    }

    void EnemyBounds()
    {
        if (transform.position.y < -7)
        {
            float rand = Random.Range(-9, 9);
            transform.position = new Vector3(rand, 8, 0);
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
}
