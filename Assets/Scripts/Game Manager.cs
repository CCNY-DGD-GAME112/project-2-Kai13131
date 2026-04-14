using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public int score = 0;

    public GameObject zombiePrefab;
    public Transform player;

    public float spawnDistance = 30f;
    public float spawnInterval = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if(p != null)
            {
                player = p.transform;
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

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    
    void SpawnZombie()
    {
        if(player == null || zombiePrefab == null)
        {
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        Vector3 spawnDirection = new Vector3(randomCircle.x, 0, randomCircle.y);

        Vector3 spawnPosition = player.position + spawnDirection * spawnDistance;

        RaycastHit hit;
        Vector3 rayStart = spawnPosition + Vector3.down * 50f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f))
        {
            spawnPosition = hit.point;
        }

        GameObject z = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        Zombie zombieScript = z.GetComponent<Zombie>();
        if (zombieScript != null)
        {
            zombieScript.player = player;
        }
    }
}
