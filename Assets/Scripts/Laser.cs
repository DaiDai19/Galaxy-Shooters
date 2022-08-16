using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Laser Speed")]
    [SerializeField] float shootSpeed = 10;
    [SerializeField] bool enemyShot;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyLaser", 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyShot)
        {
            MoveUp();
        }

        else
        {
            MoveDown();
        }
    }

    void MoveUp() => transform.Translate(transform.up * shootSpeed * Time.deltaTime);

    void MoveDown() => transform.Translate(-transform.up * shootSpeed * Time.deltaTime);

    void DestroyLaser()
    {
        Destroy(gameObject);

        if(transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void AssignLaser()
    {
        enemyShot = true;
    }

    public bool EnemyLaser()
    {
       return enemyShot;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() && enemyShot)
        {
            Player player = collision.GetComponent<Player>();
            player.TakeDamage();
        }
    }
}
