using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float zombieHealth = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float bulletDamage = 30f;
            zombieHealth -= bulletDamage;

            if(zombieHealth < 0)
            {
                zombieDie();
            }
        }
    }
    void zombieDie()
    {
        Destroy(gameObject);
    }
}
