using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Lives")]
    [SerializeField] int playerLives = 3;
    [SerializeField] GameObject shield;
    [SerializeField] bool shieldActivated = false;
    [SerializeField] List<GameObject> damagedEngines;

    [Header("Player Movmement")]
    [SerializeField] float normalSpeed = 7;
    [SerializeField] float speedMultiplier = 1.5f;
    [SerializeField] bool boosted = false;
    [SerializeField] float speedDur = 3;

    [Header("Player Thruster")]
    [SerializeField] bool thrusting = false;
    [SerializeField] float thrustSpeed = 10;

    [Header("Shield Strength")]
    [SerializeField] int shieldStrength = 3;
    [SerializeField] Color[] strengthColor = new Color[3];

    [Header("Player Shooting")]
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] int maxAmmo = 15;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject tripleShotLaser;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] Transform laserPoint;
    [SerializeField] float tripleShotDur = 3;
    [SerializeField] bool tripleShot = false;

    [Header("Player Bounds")]
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;

    [Header("Player Score")]
    [SerializeField] int score = 0;

    int currentAmmo = 0;
    float timeBetween = 0;
    float currentSpeed = 0;

    UIManager uiManager;
    Animator anim;
    AudioSource aud;

    // Start is called before the first frame update
    void Start()
    {
        timeBetween = fireRate;
        currentAmmo = maxAmmo;
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (uiManager == null)
        {
            Debug.LogError("No UI Manager");
        }

        aud = GetComponent<AudioSource>();

        if (aud == null)
        {
            Debug.LogError("No Audio Source on Player");
        }

        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError("No Animator on Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        thrusting = Input.GetKey(KeyCode.LeftShift);

        Vector2 move = new Vector2(horizontal, vertical).normalized;

        transform.Translate(move * Time.deltaTime * currentSpeed);

        currentSpeed = thrusting ? thrustSpeed : normalSpeed;

        anim.SetBool("ThrusterBoost", thrusting);

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
        if(currentAmmo <= 0)
        {
            return;
        }

        if (tripleShot)
        {
            GameObject curTriple = Instantiate(tripleShotLaser, laserPoint.position, Quaternion.identity);
        }

        else
        {
            GameObject curLaser = Instantiate(laser, laserPoint.position, Quaternion.identity);
        }

        timeBetween = Time.time + fireRate;
        aud.PlayOneShot(shootSound);
        currentAmmo--;
        uiManager.UpdateAmmo(currentAmmo);
    }

    public void TakeDamage()
    {
        if(shieldActivated)
        {
            shieldStrength--;
            shield.GetComponent<SpriteRenderer>().color = strengthColor[shieldStrength];

            if (shieldStrength <= 0)
            {
                shieldActivated = false;
                shield.SetActive(false);
            }

            return;
        }

        if (damagedEngines.Count > 0)
        {
            int randEngine = Random.Range(0, damagedEngines.Count);
            damagedEngines[randEngine].SetActive(true);
            damagedEngines.Remove(damagedEngines[randEngine]);
        }

        playerLives--;
        uiManager.UpdateLives(playerLives);

        if(playerLives <= 0)
        {
            aud.PlayOneShot(explosionSound);
            Destroy(gameObject);
        }
    }

    public void TripleShot()
    {
        StartCoroutine(TripleShotDuration());
    }

    public void SpeedUp()
    {
        StartCoroutine(SpeedUpDuration());
    }

    public void ActivateShield()
    {
        shieldStrength = 3;
        shield.GetComponent<SpriteRenderer>().color = strengthColor[shieldStrength];
        shieldActivated = true;
        shield.SetActive(true);
    }

    public void IncreaseScore(int points)
    {
        score += points; 
        uiManager.UpdateScore(score);
    }

    IEnumerator TripleShotDuration()
    {
        tripleShot = true;

        yield return new WaitForSeconds(tripleShotDur);

        tripleShot = false;
    }

    IEnumerator SpeedUpDuration()
    {
        normalSpeed *= speedMultiplier;

        yield return new WaitForSeconds(speedDur);

        normalSpeed /= speedMultiplier;
    }
}
