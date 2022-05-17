using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Speed")]
    [SerializeField] float speed = 3;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage();
                Destroy(gameObject);
            }
        }

        if (other.GetComponent<Laser>())
        {
            Destroy(other.gameObject);
            player.IncreaseScore(Random.Range(10, 15));
            Destroy(gameObject);
        }
    }
}
