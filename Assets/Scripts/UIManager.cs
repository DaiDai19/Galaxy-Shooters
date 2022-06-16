using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text scoreText;
<<<<<<< HEAD
=======
    [SerializeField] Text ammoText;
>>>>>>> dev
    [SerializeField] Sprite[] livesSprite;
    [SerializeField] Image livesImg;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject restartText;

    Player player;
    GameManager gManager;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        gManager = FindObjectOfType<GameManager>();
        scoreText.text = "Score: " + 0;
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

<<<<<<< HEAD
=======
    public void UpdateAmmo(int ammo)
    {
        ammoText.text = "Ammo: " + ammo.ToString();
    }

>>>>>>> dev
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
