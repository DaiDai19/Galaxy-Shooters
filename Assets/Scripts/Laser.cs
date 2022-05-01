using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Laser Speed")]
    [SerializeField] float shootSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyLaser", 2);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * shootSpeed * Time.deltaTime);
    }

    void DestroyLaser()
    {
        Destroy(gameObject);
    }
}
