using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text waveText;
    [SerializeField] private Sprite[] livesSprite;
    [SerializeField] private Image livesImg;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject restartText;
    [SerializeField] private Image fillColor;
    [SerializeField] private Slider thrusterBar;

    private Player player;
    private PlayerLives playerLives;
    private PlayerShoot playerShoot;
    private GameManager gameManager;
    private WaveManager waveManager;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        playerLives = FindObjectOfType<PlayerLives>();
        playerShoot = FindObjectOfType<PlayerShoot>();
        gameManager = FindObjectOfType<GameManager>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    private void OnEnable()
    {
        player.OnBoosterUse += UpdateBooster;
        player.OnScoreUpdate += UpdateScore;
        player.OnSetBooster += SetMaxBooster;
        playerLives.OnDamage += UpdateLives;
        playerShoot.OnAmmoUse += UpdateAmmo;
        waveManager.OnWaveChange += UpdateWave;
    }

    private void OnDisable()
    {
        player.OnBoosterUse -= UpdateBooster;
        player.OnScoreUpdate -= UpdateScore;
        player.OnSetBooster -= SetMaxBooster;
        playerLives.OnDamage -= UpdateLives;
        playerShoot.OnAmmoUse -= UpdateAmmo;
        waveManager.OnWaveChange -= UpdateWave;
    }

    // Start is called before the first frame update
    private void Start()
    {
        scoreText.text = "Score: " + 0;
        ammoText.text = "Ammo: " + 15 + "/" + 15;
    }

    public void UpdateScore(int playerScore)
    {
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int lives)
    {
        livesImg.sprite = livesSprite[lives];

        if(lives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmo(int playerAmmo, int maxAmmo)
    {
        ammoText.text = "Ammo: " + playerAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateBooster(float amount, Color thrustColor)
    {
        thrusterBar.value = amount;
        fillColor.color = thrustColor;
    }

    public void UpdateWave(int wave, bool enabled)
    {
        if (wave < WaveManager.Instance.CurrentWave.Length)
        {
            waveText.enabled = enabled;
            waveText.text = "Wave " + wave.ToString();
        }

        else
        {
            waveText.enabled = enabled;
            waveText.text = "Final Wave!";
        }
    }

    public void SetMaxBooster(float maxBoost)
    {
        thrusterBar.maxValue = maxBoost;
    }

    private void GameOverSequence()
    {
        gameManager.GameOver();
        gameOverPanel.SetActive(true);
        restartText.SetActive(true);
        StartCoroutine(BlinkingText());
    }

    private IEnumerator BlinkingText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            gameOverPanel.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            gameOverPanel.SetActive(true);
        }        
    }
}