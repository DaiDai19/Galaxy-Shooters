using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float fallSpeed = 3;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] GameObject explosionVFX;

    WaveManager waveManager;

    // Start is called before the first frame update
    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Laser>())
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
            waveManager.StartSpawning();
            Destroy(gameObject);
            Destroy(collision.gameObject); 
        }
    }
}
