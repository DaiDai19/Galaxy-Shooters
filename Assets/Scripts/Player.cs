using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Lives")]
    [SerializeField] int playerLives = 3;

    [Header("Player Movmement")]
    [SerializeField] float speed = 7;

    [Header("Player Shooting")]
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject tripleShotLaser;
    [SerializeField] Transform laserPoint;
    [SerializeField] float tripleShotDur = 3;
    [SerializeField] bool tripleShot = false;

    [Header("Player Bounds")]
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;

    float timeBetween = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeBetween = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(horizontal, vertical).normalized;

        transform.Translate(move * Time.deltaTime * speed);

        PlayerBounds();

        if(Input.GetButtonDown("Jump") && Time.time >= timeBetween)
        {
            PlayerShooting();
        }
    }

    void PlayerBounds()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minY, maxY), 0);

        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }

        if (transform.position.x < minX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
    }

    void PlayerShooting()
    {
        if (tripleShot)
        {
            GameObject curTriple = Instantiate(tripleShotLaser, laserPoint.position, Quaternion.identity);
        }

        else
        {
            GameObject curLaser = Instantiate(laser, laserPoint.position, Quaternion.identity);
        }

        timeBetween = Time.time + fireRate;
    }

    public void TakeDamage()
    {
        playerLives--;

        if(playerLives <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TripleShot()
    {
        StartCoroutine(TripleShotDuration());
    }

    IEnumerator TripleShotDuration()
    {
        tripleShot = true;

        yield return new WaitForSeconds(tripleShotDur);

        tripleShot = false;
    }
}
