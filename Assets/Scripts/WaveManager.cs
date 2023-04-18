using System.Collections;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave System")]
    [SerializeField] private int waveCounter = 0;
    [SerializeField] private Wave[] wave;
    [SerializeField] private bool isReseting;

    [Header("SpawnPool")]
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private GameObject[] powerup;
    [SerializeField] private GameObject[] specialPowerup;

    public event Action<int, bool> OnWaveChange;

    public Wave[] CurrentWave => wave;

    private Player player;

    private void Awake()
    {
        Instance = this;
    }

    public void StartSpawning()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        isReseting = true;
        OnWaveChange?.Invoke(waveCounter + 1, true);

        yield return new WaitForSeconds(2);

        isReseting = false;
        OnWaveChange?.Invoke(waveCounter, false);
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    private IEnumerator SpawnEnemy()
    {
        int currentAmountSpawned = 0;

        while (player != null && !wave[waveCounter].finishSpawning)
        {
            if (currentAmountSpawned < wave[waveCounter].amountToSpawn)
            {
                float rand = UnityEngine.Random.Range(-7, 7);
                Instantiate(wave[waveCounter].enemy, new Vector3(rand, 10, 0), Quaternion.identity, enemyContainer.transform);
                currentAmountSpawned++;

                yield return new WaitForSeconds(3);
            }

            else if(enemyContainer.childCount == 0)
            {
                wave[waveCounter].finishSpawning = true;
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

    private IEnumerator SpawnPowerup()
    {
        while (player != null && !isReseting)
        {
            float randPos = UnityEngine.Random.Range(-7, 7);
            int randomPowerup = UnityEngine.Random.Range(0, powerup.Length);
            int specialPowerupRand = UnityEngine.Random.Range(0, 100);

            int specialPowerupSpawn = UnityEngine.Random.Range(0, specialPowerup.Length);

            if (specialPowerupRand >= 90)
            {
                Instantiate(specialPowerup[specialPowerupSpawn], new Vector3(randPos, 10, 0), Quaternion.identity);
            }

            else
            {
                Instantiate(powerup[randomPowerup], new Vector3(randPos, 10, 0), Quaternion.identity);
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 7));
        }
    }
}

[Serializable]
public class Wave
{
    public int amountToSpawn;
    public GameObject enemy;
    public bool finishSpawning;
}