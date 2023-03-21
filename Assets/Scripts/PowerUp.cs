using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float fallSpeed = 2;

    [Header("ID")]
    [SerializeField] private int powerupID = 0;

    [Header("Sound")]
    [SerializeField] private AudioClip clip;

    private Player player;

    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<Player>();
        player.AddPowerup(this);
    }

    // Update is called once per frame
    private void Update()
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
            PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        playerShoot.TripleShot();
                        break;

                    case 1:
                        player.SpeedUp();
                        break;

                    case 2:
                        player.ActivateShield();
                        break;

                    case 3:
                        playerShoot.AmmoRefill();
                        break;

                    case 4:
                        PlayerLives playerLives = player.GetComponent<PlayerLives>();
                        playerLives.RecoverHealth();
                        break;

                    case 5:
                        playerShoot.SpreadShot();
                        break;

                    case 6:
                        player.Stun();
                        break;
                }

                AudioSource.PlayClipAtPoint(clip, transform.position);
                player.RemovePowerup(this);
                player.StopPullingPickup();
                Destroy(gameObject);
            }

        }

        if (collision.GetComponent<Laser>())
        {
            if (collision.GetComponent<Laser>().EnemyLaser())
            {
                player.RemovePowerup(this);
                player.StopPullingPickup();
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }

            player.StopPullingPickup();
        }
    }
}
