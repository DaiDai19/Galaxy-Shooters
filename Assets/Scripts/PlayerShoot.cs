using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player Shooting")]
    [SerializeField] private int maxAmmo = 15;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject tripleShotLaser;
    [SerializeField] private GameObject spreadShotLaser;
    [SerializeField] private GameObject missileShotLaser;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private Transform laserPoint;
    [SerializeField] private float powerUpDuration = 3;
    [SerializeField] private bool tripleShot = false;
    [SerializeField] private bool spreadShot = false;
    [SerializeField] private bool missileShot = false;
    [SerializeField] private Vector2 shotDirection;

    public event Action<int, int> OnAmmoUse;

    private float timeBetween = 0;
    private int currentAmmo = 0;

    private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();

        if (aud == null)
        {
            Debug.LogError("No Audio Source on Player");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeBetween = fireRate;
        currentAmmo = maxAmmo;
        shotDirection = transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && Time.time >= timeBetween)
        {
            PlayerShooting();
        }
    }

    private void PlayerShooting()
    {
        if (currentAmmo <= 0)
        {
            return;
        }

        if (tripleShot)
        {
            GameObject curTriple = Instantiate(tripleShotLaser, laserPoint.position, Quaternion.identity);
            Laser laser = curTriple.GetComponent<Laser>();
            laser.ShotDirection(shotDirection);
        }

        else if (spreadShot)
        {
            GameObject curSpread = Instantiate(spreadShotLaser, laserPoint.position, Quaternion.identity);
            Laser laser = curSpread.GetComponent<Laser>();
            laser.ShotDirection(shotDirection);
        }

        else if (missileShot)
        {
            Instantiate(missileShotLaser, laserPoint.position, Quaternion.identity);
        }

        else
        {
            GameObject curLaser = Instantiate(laser, laserPoint.position, laser.transform.rotation);
            Laser laserObj = curLaser.GetComponent<Laser>();
            laserObj.ShotDirection(shotDirection);
        }

        timeBetween = Time.time + fireRate;
        currentAmmo--;
        OnAmmoUse?.Invoke(currentAmmo, maxAmmo);
        aud.PlayOneShot(shootSound);
    }
    public void TripleShot()
    {
        if (spreadShot || missileShot) return;

        StartCoroutine(TripleShotDuration());
    }

    public void SpreadShot()
    {
        if (tripleShot || missileShot) return;

        StartCoroutine(SpreadShotDuration());
    }

    public void MissileShot()
    {
        if (tripleShot || spreadShot) return;

        StartCoroutine (MissileShotDuration());
    }

    public void AmmoRefill()
    {
        currentAmmo = maxAmmo;
        OnAmmoUse?.Invoke(currentAmmo, maxAmmo);
    }
    private IEnumerator TripleShotDuration()
    {
        tripleShot = true;

        yield return new WaitForSeconds(powerUpDuration);

        tripleShot = false;
    }

    private IEnumerator SpreadShotDuration()
    {
        spreadShot = true;

        yield return new WaitForSeconds(powerUpDuration);

        spreadShot = false;
    }

    private IEnumerator MissileShotDuration()
    {
        missileShot = true;

        yield return new WaitForSeconds(powerUpDuration);

        missileShot = false;
    }
}