
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private GameObject explosionVFX;


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
            WaveManager.Instance.StartSpawning();
            Destroy(gameObject);
            Destroy(collision.gameObject); 
        }
    }
}
