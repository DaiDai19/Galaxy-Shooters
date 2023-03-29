using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float lookRadius = 20;
    [SerializeField] private Enemy currentEnemy;

    private bool enemyTargeted = false;

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
    }

    private Enemy SetClosestEnemy()
    {
        if (enemyTargeted) return null;

        int closest = 9999;

        Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, 10);

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

        enemyTargeted = false;
        return null;
    }
}
