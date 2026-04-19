using System.Collections;
using UnityEngine;

public class HealthStation : MonoBehaviour
{
    public float addHealthAmount = 30f;
    public float respawnTime = 60f;

    public Renderer[] rends;
    private Collider col;

    public float rotateSpeed = 30f;

    void Start()
    {
        col = GetComponent<Collider>();
        rends = GetComponentsInChildren<Renderer>();

        // make unique materials so color change only affects this object
        foreach (Renderer r in rends)
        {
            r.material = new Material(r.material);
        }
    }

    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Heal(addHealthAmount);
            }

            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        SetActive(true);
    }

    void SetActive(bool state)
    {
        // enable/disable visuals
        foreach (Renderer r in rends)
        {
            if (r != null)
            {
                r.enabled = state;
                
            }
        }
        // enable/disable collider
        if (col != null)
            col.enabled = state;
    }

}