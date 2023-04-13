using UnityEngine;

public class Laser : MonoBehaviour, IProjectile
{
    [Header("Laser Speed")]
    [SerializeField] private float shootSpeed = 10;
    [SerializeField] private bool enemyShot;

    private Vector2 direction;

    // Start is called before the first frame update
    private void Start()
    {
        Invoke(nameof(DestroyLaser), 2);
    }

    // Update is called once per frame
    private void Update()
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

    private void MoveUp() => transform.Translate(direction * shootSpeed * Time.deltaTime);

    private void MoveDown() => transform.Translate(-direction * shootSpeed * Time.deltaTime);

    private void DestroyLaser()
    {
        Destroy(gameObject);

        if(transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void ShotDirection(Vector2 direction)
    {
        this.direction = direction;
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
            PlayerLives playerLives = collision.GetComponent<PlayerLives>();
            playerLives?.TakeDamage();
        }
    }
}

public interface IProjectile
{
    public void AssignLaser();
    public bool EnemyLaser();

    public void ShotDirection(Vector2 direction);
}