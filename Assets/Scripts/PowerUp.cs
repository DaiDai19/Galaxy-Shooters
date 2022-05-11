using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] float fallSpeed = 2;

    [Header("ID")]
    [SerializeField] int powerupID = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if(transform.position.y < -7)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Player player = collision.GetComponent<Player>();
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
                        break;
                }

                Destroy(gameObject);
            }
        }
    }
}
