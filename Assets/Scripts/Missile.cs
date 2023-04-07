using UnityEngine;

public class Missile : MonoBehaviour, IProjectile
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float lookRadius = 20;

    private bool enemyTargeted = false;

    private Enemy currentEnemy;

    private void Start()
    {
        Invoke(nameof(DestroySelf), 2f);
    }

    private void Update()
    {
        if (enemyTargeted)
        {
            FollowEnemyPosition();
        }

        else
        {
            MoveVertically();
        }
    }

    private void MoveVertically()
    {
        if (SetClosestEnemy() != null) return;

        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }

    private void FollowEnemyPosition()
    {
        if (SetClosestEnemy() == null) return;

        var enemyDirection = (currentEnemy.transform.position - transform.position).normalized;

        Quaternion rot = Quaternion.LookRotation(transform.forward, enemyDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime); 

        transform.Translate(enemyDirection * moveSpeed * Time.deltaTime);
    }

    private Enemy SetClosestEnemy()
    {
        if (enemyTargeted) return currentEnemy;

        int closest = 9999;

        Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, lookRadius);

        foreach (var col in targetColliders)
        {
            if (col.GetComponent<Enemy>())
            {
                int distance = (int)Vector3.Distance(transform.position, col.gameObject.transform.position);

                if (distance < closest)
                {
                    currentEnemy = col.GetComponent<Enemy>();
                    enemyTargeted = true;
                    return currentEnemy;
                }
            }
        }

        if (currentEnemy.IsDead)
        {
            enemyTargeted = false;
            return null;
        }

        enemyTargeted = false;
        return null;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void AssignLaser()
    {

    }

    public bool EnemyLaser()
    {
        return false;
    }
}
