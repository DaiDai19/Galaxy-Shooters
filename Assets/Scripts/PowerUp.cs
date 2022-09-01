using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] float fallSpeed = 2;

    [Header("ID")]
    [SerializeField] int powerupID = 0;

    [Header("Sound")]
    [SerializeField] AudioClip clip;

    Player player;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.AddPowerup(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if(transform.position.y < -7)
        {
            player.RemovePowerup(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Player player = collision.GetComponent<Player>();

            AudioSource aud = GetComponent<AudioSource>();

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShot();
                        break;

                    case 1:
                        player.SpeedUp();
                        break;

                    case 2:
                        player.ActivateShield();
                        break;

                    case 3:
                        player.AmmoRefill();
                        break;

                    case 4:
                        player.RecoverHealth();
                        break;

                    case 5:
                        player.SpreadShot();
                        break;

                    case 6:
                        player.Stun();
                        break;
                }

                AudioSource.PlayClipAtPoint(clip, transform.position);
                player.RemovePowerup(this);
                Destroy(gameObject);
            }
        }

        if (collision.GetComponent<Laser>())
        {
            if (collision.GetComponent<Laser>().EnemyLaser())
            {
                player.RemovePowerup(this);
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
