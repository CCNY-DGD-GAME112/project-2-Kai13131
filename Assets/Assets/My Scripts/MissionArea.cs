using UnityEngine;
using TMPro;

public class MissionArea : MonoBehaviour
{
    public float requiredTime = 30f;
    private float timer = 0f;

    private bool playerInArea = false;
    private bool completed = false;

    public TextMeshProUGUI timerText;

    public string missionName = "Mission Area";

    public GameObject canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(completed) return;

        if (playerInArea)
        {
            timer += Time.deltaTime;
            if (timer >= requiredTime)
            {
                CompleteMission();
            }
        }

        UpdateUI();


        canvas.transform.LookAt(Camera.main.transform);
        canvas.transform.Rotate(0, 180, 0);
    }


    void UpdateUI()
    {
        if(timerText == null) return;
        if (!playerInArea)
        {
            timerText.text = "Mission Area";
        }

        if(timer < requiredTime && playerInArea)
        {
            timerText.text = "Mission In Process: " +
                timer.ToString("F1")
                + " / "
                + requiredTime.ToString("F0");
        }
        if (timer >= requiredTime)
        {
            timerText.text = "Mission Completed!";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
            Debug.Log("Player entered " + missionName);

            if (timer >= requiredTime)
            {
                timerText.text = "Mission Completed!";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = false;
        }
    }

    void CompleteMission()
    {
        if (completed)
        {
            return;
        }

        completed = true;
        Debug.Log(missionName + " completed!");
        GameManager.Instance.MissionCompleted();
    }
}
