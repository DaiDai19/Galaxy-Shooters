using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private GameObject explosionVFX;

    private WaveManager waveManager;

    // Start is called before the first frame update
    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IProjectile>() is IProjectile)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
            waveManager.StartSpawning();
            Destroy(gameObject);
            Destroy(collision.gameObject); 
        }
    }
}
