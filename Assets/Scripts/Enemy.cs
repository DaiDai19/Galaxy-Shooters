using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Speed")]
    [SerializeField] float speed = 3;

    [Header("Damage")]
    [SerializeField] AudioClip clip;

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
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        EnemyBounds();
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
                player.TakeDamage();
                Destroy();
            }
        }

        if (other.GetComponent<Laser>())
        {
            Destroy(other.gameObject);
            player.IncreaseScore(Random.Range(10, 15));
            Destroy();
        }
    }
}
