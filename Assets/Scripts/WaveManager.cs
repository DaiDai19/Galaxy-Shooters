using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("Wave System")]
    [SerializeField] int waveCounter = 0;
    [SerializeField] Wave[] wave;
    [SerializeField] bool isReseting;

    [Header("SpawnPool")]
    [SerializeField] Transform enemyContainer;
    [SerializeField] GameObject[] powerup;

    [SerializeField] GameObject[] specialPowerup;

    Player player;
    UIManager uiMan;

    public void StartSpawning()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        uiMan = GameObject.Find("Canvas").GetComponent<UIManager>();
        StartCoroutine(StartWave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartWave()
    {
        isReseting = true;
        uiMan.UpdateWave(waveCounter + 1, true);

        yield return new WaitForSeconds(2);

        isReseting = false;
        uiMan.UpdateWave(waveCounter, false);
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    IEnumerator SpawnEnemy()
    {
        int currentAmountSpawned = 0;

        while (player != null && !wave[waveCounter].SpawningFinished)
        {
            if (currentAmountSpawned < wave[waveCounter].EnemyAmount)
            {
                float rand = Random.Range(-9, 9);
                Instantiate(wave[waveCounter].Enemy, new Vector3(rand, 10, 0), Quaternion.identity, enemyContainer.transform);
                currentAmountSpawned++;

                yield return new WaitForSeconds(3);
            }

            else if(enemyContainer.childCount == 0)
            {
                wave[waveCounter].SpawningFinished = true;
                yield return null;
            }

            else
            {
                yield return null;
            }
        }

        if (player != null)
        {
            waveCounter++;
            StartCoroutine(StartWave());
            yield return null;
        }
    }

    IEnumerator SpawnPowerup()
    {
        while (player != null && !isReseting)
        {
            float randPos = Random.Range(-7, 7);
            int randomPowerup = Random.Range(0, powerup.Length);
            int specialPowerupRand = Random.Range(0, 100);

            int specialPowerupSpawn = Random.Range(0, specialPowerup.Length);

            if (specialPowerupRand >= 90)
            {
                Instantiate(specialPowerup[specialPowerupSpawn], new Vector3(randPos, 10, 0), Quaternion.identity);
            }

            else
            {
                Instantiate(powerup[randomPowerup], new Vector3(randPos, 10, 0), Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(3, 7));
        }
    }
}

[System.Serializable]
public class Wave
{
    [SerializeField] int amountToSpawn;
    [SerializeField] GameObject enemy;
    [SerializeField] bool finishSpawning;

    public int EnemyAmount
    {
        get
        {
            return amountToSpawn;
        }
    }

    public GameObject Enemy
    {
        get
        {
            return enemy;
        }
    }

    public bool SpawningFinished
    {
        get
        {
            return finishSpawning;
        }

        set
        {
            finishSpawning = value;
        }
    }
}
