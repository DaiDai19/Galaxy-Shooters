using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("SpawnPool")]
    [SerializeField] Transform enemyContainer;
    [SerializeField] GameObject enemy;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemy()
    {
        while (player != null)
        {
            float rand = Random.Range(-9, 9);
            Instantiate(enemy, new Vector3(rand, 10, 0), Quaternion.identity, enemyContainer.transform);
            yield return new WaitForSeconds(3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            player.TakeDamage();
            Destroy(gameObject);
        }

        if (other.GetComponent<Laser>())
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
