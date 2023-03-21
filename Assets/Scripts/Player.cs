using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Header("Player Movmement")]
    [SerializeField] private float normalSpeed = 7;
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private bool boosted = false;
    [SerializeField] private float speedDur = 3;
    [SerializeField] private bool stunned = false;

    [Header("Player Thruster")]
    [SerializeField] private bool thrusting = false;
    [SerializeField] private bool hasThrust = true;
    [SerializeField] private float thrustSpeed = 10;
    [SerializeField] private float maxThrustBoost = 50;
    [SerializeField] private Color[] thrustColor = new Color[2];
    [SerializeField] private float thrustMultiplier = 2.5f;
    [SerializeField] private GameObject thruster;

    [Header("Shield Strength")]
    [SerializeField] private int shieldStrength = 3;
    [SerializeField] private Color[] strengthColor = new Color[3];
    [SerializeField] private GameObject shield;
    [SerializeField] private bool shieldActivated = false;
    [SerializeField] private List<GameObject> damagedEngines;

    [Header("Player Bounds")]
    [SerializeField] private Bounds playerBounds;

    [Header("Player Score")]
    [SerializeField] private int score = 0;

    [Header("Player Pickup")]
    [SerializeField] private List<PowerUp> currentPowerups = new List<PowerUp>();
    [SerializeField] private PowerUp closestPowerup;
    [SerializeField] private int pickUpSpeed = 5;

    public event Action<float> OnSetBooster;
    public event Action<float, Color> OnBoosterUse;
    public event Action<int> OnScoreUpdate;

    public bool ShieldActivated { get => shieldActivated; }

    private float currentSpeed = 0;
    private float thrustBoost;
    private bool pullingPickup = false;

    private Animator anim;
    private PlayerLives playerLives;
    private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();

        if (aud == null)
        {
            Debug.LogError("No Audio Source on Player");
        }

        playerLives = GetComponent<PlayerLives>();

        if (playerLives == null)
        {
            Debug.LogError("No Player Lives on Player");
        }

        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError("No Animator on Player");
        }

    }

    private void OnEnable()
    {
        playerLives.OnShieldDamage += ShieldDamage;
    }

    private void OnDisable()
    {
        playerLives.OnShieldDamage -= ShieldDamage;
    }

    private void Start()
    {
        thrustBoost = maxThrustBoost;
        OnSetBooster?.Invoke(maxThrustBoost);
    }

    private void Update()
    {
        if (stunned) return;

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

        if (Input.GetKey(KeyCode.C) && LocateClosetPowerup() != null)
        {
            MovePickupTowardsPlayer();
        }

        if (Input.GetKeyUp(KeyCode.C) && pullingPickup)
        {
            pullingPickup = false;
        }

        if (!pullingPickup)
        {
            closestPowerup = LocateClosetPowerup();
        }
    }

    private void PlayerBounds()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, playerBounds.minY, playerBounds.maxY), 0);

        if (transform.position.x > playerBounds.maxX)
        {
            transform.position = new Vector3(playerBounds.minX, transform.position.y, transform.position.z);
        }

        if (transform.position.x < playerBounds.minX)
        {
            transform.position = new Vector3(playerBounds.maxX, transform.position.y, transform.position.z);
        }
    }
   
    private void ShieldDamage()
    {
        shieldStrength--;
        shield.GetComponent<SpriteRenderer>().color = strengthColor[shieldStrength];

        if (shieldStrength <= 0)
        {
            shieldActivated = false;
            shield.SetActive(false);
        }
    }

    private void ThrusterBoost()
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

        OnBoosterUse?.Invoke(thrustBoost, thrustColor[1]);
        currentSpeed = thrusting ? thrustSpeed : normalSpeed;
        anim.SetBool("ThrusterBoost", thrusting);
    }

    private void ThrustCoolDown()
    {
        thrustBoost += Time.deltaTime;
        OnBoosterUse?.Invoke(thrustBoost, thrustColor[0]);

        if (thrustBoost >= maxThrustBoost)
        {
            thrustBoost = maxThrustBoost;
            hasThrust = true;
        }
    }

    private PowerUp LocateClosetPowerup()
    {
        int closest = 9999;

        Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, 10);

        foreach (var col in targetColliders)
        {
            if (col.GetComponent<PowerUp>())
            {
                int distance = (int)Vector3.Distance(transform.position, col.gameObject.transform.position);

                if (distance < closest)
                {
                    return col.GetComponent<PowerUp>();
                }
            }
        }

        return null;
    }

    private void MovePickupTowardsPlayer()
    { 
        pullingPickup = true;
        closestPowerup.transform.position = Vector3.MoveTowards(closestPowerup.transform.position, transform.position, pickUpSpeed * Time.deltaTime);
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
        OnScoreUpdate?.Invoke(score);
    }

    public void Stun()
    {
        StartCoroutine(StunDuration());
    }

    public void AddPowerup(PowerUp powerUp)
    {
        if (currentPowerups.Contains(powerUp)) return;

        currentPowerups.Add(powerUp);
    }

    public void RemovePowerup(PowerUp powerUp)
    {
        if (!currentPowerups.Contains(powerUp)) return;

        currentPowerups.Remove(powerUp);
    }

    public void StopPullingPickup()
    {
        if (pullingPickup) return;

        pullingPickup = false;
    }

    private IEnumerator SpeedUpDuration()
    {
        normalSpeed *= speedMultiplier;

        yield return new WaitForSeconds(speedDur);

        normalSpeed /= speedMultiplier;
    }

    private IEnumerator StunDuration()
    {
        stunned = true;
        thruster.SetActive(false);

        yield return new WaitForSeconds(2);

        stunned = false;
        thruster.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}

[Serializable]
public class Bounds
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
}