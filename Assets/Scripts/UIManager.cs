using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text scoreText;
    [SerializeField] Text ammoText;
    [SerializeField] Sprite[] livesSprite;
    [SerializeField] Image livesImg;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject restartText;
    [SerializeField] Image fillColor;
    [SerializeField] Slider thrusterBar;

    Player player;
    GameManager gManager;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        gManager = FindObjectOfType<GameManager>();
        scoreText.text = "Score: " + 0;
        ammoText.text = "Ammo: " + 15 + "/" + 15;
    }

    // Update is called once per frame
    void Update()
    {

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

    public void UpdateBooster(float amount)
    {
        thrusterBar.value = amount;
    }

    public void SetMaxBooster(float maxBoost)
    {
        thrusterBar.maxValue = maxBoost;
    }

    public void ThrustColor(Color thrustColor)
    {
        fillColor.color = thrustColor;
    }

    void GameOverSequence()
    {
        gManager.GameOver();
        gameOverPanel.SetActive(true);
        restartText.SetActive(true);
        StartCoroutine(BlinkingText());
    }

    IEnumerator BlinkingText()
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
