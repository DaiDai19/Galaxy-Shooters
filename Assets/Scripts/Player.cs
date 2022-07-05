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
    [SerializeField] bool hasThrust = true;
    [SerializeField] float thrustSpeed = 10;
    [SerializeField] float maxThrustBoost = 50;
    [SerializeField] Color[] thrustColor = new Color[2];
    [SerializeField] float thrustMultiplier = 2.5f;

    [Header("Shield Strength")]
    [SerializeField] int shieldStrength = 3;
    [SerializeField] Color[] strengthColor = new Color[3];

    [Header("Player Shooting")]
    [SerializeField] int maxAmmo = 15;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject tripleShotLaser;
    [SerializeField] GameObject spreadShotLaser;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] Transform laserPoint;
    [SerializeField] float tripleShotDur = 3;
    [SerializeField] bool tripleShot = false;
    [SerializeField] bool spreadShot = false;

    [Header("Player Bounds")]
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;

    [Header("Player Score")]
    [SerializeField] int score = 0;

    float timeBetween = 0;
    int currentAmmo = 0;
    float currentSpeed = 0;
    float thrustBoost;

    UIManager uiManager;
    Animator anim;
    AudioSource aud;

    // Start is called before the first frame update
    void Start()
    {
        timeBetween = fireRate;
        currentAmmo = maxAmmo;

        thrustBoost = maxThrustBoost;

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

        uiManager.SetMaxBooster(maxThrustBoost);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        thrusting = Input.GetKey(KeyCode.LeftShift);

        Vector2 move = new Vector2(horizontal, vertical).normalized;

        transform.Translate(move * Time.deltaTime * currentSpeed);

        if (hasThrust)
        {
            ThrusterBoost();
        }

        else
        {
            ThrustCoolDown();
        }

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
        if (currentAmmo <= 0)
        {
            return;
        }

        if (tripleShot)
        {
            GameObject curTriple = Instantiate(tripleShotLaser, laserPoint.position, Quaternion.identity);
        }

        else if (spreadShot)
        {
            GameObject curSpread = Instantiate(spreadShotLaser, laserPoint.position, Quaternion.identity);
        }

        else
        {
            GameObject curLaser = Instantiate(laser, laserPoint.position, laser.transform.rotation);
        }

        timeBetween = Time.time + fireRate;
        currentAmmo--;
        uiManager.UpdateAmmo(currentAmmo);
        aud.PlayOneShot(shootSound);
    }

    void ThrusterBoost()
    {
        if (thrusting)
        {
            thrustBoost -= Time.deltaTime * thrustMultiplier;

            if (thrustBoost <= 0)
            {
                thrustBoost = 0;
                hasThrust = false;
                thrusting = false;
            }
        }

        else
        {
            if (thrustBoost < maxThrustBoost)
            {
                thrustBoost += Time.deltaTime;
            }
        }

        uiManager.ThrustColor(thrustColor[1]);

        currentSpeed = thrusting ? thrustSpeed : normalSpeed;

        anim.SetBool("ThrusterBoost", thrusting);
        uiManager.UpdateBooster(thrustBoost);
    }

    void ThrustCoolDown()
    {
        uiManager.ThrustColor(thrustColor[0]);
        thrustBoost += Time.deltaTime;
        uiManager.UpdateBooster(thrustBoost);

        if (thrustBoost >= maxThrustBoost)
        {
            thrustBoost = maxThrustBoost;
            hasThrust = true;
        }
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


        playerLives--;
        uiManager.UpdateLives(playerLives);

        if(playerLives <= 0)
        {
            aud.PlayOneShot(explosionSound);
            Destroy(gameObject);
        }

        damagedEngines[playerLives].SetActive(true);
        CameraShake.instance.ShakeCamera();
    }

    public void RecoverHealth()
    {
        if (playerLives >= 3)
        {
            return;
        }

        damagedEngines[playerLives].SetActive(false);
        playerLives = Mathf.Min(playerLives + 1, 3);
        uiManager.UpdateLives(playerLives);
    }

    public void TripleShot()
    {
        if (spreadShot) return;

        StartCoroutine(TripleShotDuration());
    }

    public void SpreadShot()
    {
        if (tripleShot) return;

        StartCoroutine(SpreadShotDuration());
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

    public void AmmoRefill()
    {
        currentAmmo = maxAmmo;
        uiManager.UpdateAmmo(currentAmmo);
    }

    IEnumerator TripleShotDuration()
    {
        tripleShot = true;

        yield return new WaitForSeconds(tripleShotDur);

        tripleShot = false;
    }

    IEnumerator SpreadShotDuration()
    {
        spreadShot = true;

        yield return new WaitForSeconds(tripleShotDur);

        spreadShot = false;
    }

    IEnumerator SpeedUpDuration()
    {
        normalSpeed *= speedMultiplier;

        yield return new WaitForSeconds(speedDur);

        normalSpeed /= speedMultiplier;
    }
}
