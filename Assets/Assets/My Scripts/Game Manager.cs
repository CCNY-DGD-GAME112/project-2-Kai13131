using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : Character
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public int score = 0;

    public TextMeshProUGUI healthText;
    
    public GameObject zombiePrefab;
    public GameObject player;

    public List<GameObject> zombies = new List<GameObject>();

    public float spawnDistance = 30f;
    public float spawnInterval = 3f;

    public int completeMissions = 0;
    public int totalMissions = 4;
    private bool gameWon = false;
    private bool gameOver = false;

    public GameObject winUI;
    public GameObject loseUI;

    public TextMeshProUGUI WinfinalPointText;
    public TextMeshProUGUI LoseFinalPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if(p != null)
            {
                player = p;
            }
        }


        InvokeRepeating("SpawnZombie", 1f, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MissionCompleted()
    {
        if (gameWon) return;

        completeMissions++;
        Debug.Log("Mission Completed! " + completeMissions);
        if(completeMissions >= totalMissions)
        {
            GameWin();
        }
    }

    public void GameWin()
    {
        gameWon = true;
        Debug.Log("YOU WIN!");
        if(winUI != null)
        {
            winUI.SetActive(true);
            if(WinfinalPointText != null)
                WinfinalPointText.text = score.ToString() + " Points";
        }

        CancelInvoke("SpawnZombie");
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
        {
            PlayerController playerScript = player.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.enabled = false;
            }
        }
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("GAME OVER!");
        
        if (loseUI != null)
        {
            loseUI.SetActive(true);
            if (LoseFinalPoints != null)
                LoseFinalPoints.text = score.ToString() + " Points";

        }
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }


    public void RestartButton()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitButton()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void playerHealthUpdate(float health)
    {
        
        Debug.Log("HP: " + health);
        if (healthText != null)
        {
            healthText.text = "HP: " + health;
        }
    }

    
    void SpawnZombie()
    {
        if (gameWon) return;

        if(player == null || zombiePrefab == null)
        {
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        Vector3 spawnDirection = new Vector3(randomCircle.x, 0, randomCircle.y);

        Vector3 spawnPosition = player.transform.position + spawnDirection * spawnDistance;

        RaycastHit hit;
        Vector3 rayStart = spawnPosition + Vector3.down * 10f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f))
        {
            spawnPosition = hit.point;
            Debug.Log("Found Ground");
        }

        if(zombies.Count >= 30)
        {
            return;
        }
        else
        {
            GameObject z;
            for (int i = 0; i < 3; i++)
            {
                z = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                zombies.Add(z);

                Zombie zombieScript = z.GetComponent<Zombie>();
                if (zombieScript != null)
                {
                    zombieScript.player = player.transform;
                }
            }


        }
    }
}
