using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    [Header("Laser Speed")]
    [SerializeField] private float shootSpeed = 10;

    // Start is called before the first frame update
    private void Start()
    {
        Invoke("DestroyLaser", 2);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector2.down * shootSpeed * Time.deltaTime);
    }

    private void DestroyLaser()
    {
        Destroy(gameObject);

        if (gameObject.transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerLives>())
        {
            PlayerLives playerLives = collision.GetComponent<PlayerLives>();
            playerLives.TakeDamage();
            Destroy(gameObject);
        }
    }
}
