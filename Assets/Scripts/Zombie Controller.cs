using UnityEngine;
using System.Collections;

public class Zombie : Character
{
    public Transform player;
    public Rigidbody rb;

    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    private bool canAttack = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(AIBehavior());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
        {
            Die();
        }
    }
    IEnumerator AIBehavior()
    {
        while (true)
        {
            if(player == null)
            {
                yield return null;
                continue;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer > 30f)
            {
                // Idle
                yield return null;
                continue;
            }
            else if (distanceToPlayer <= 30f && distanceToPlayer > attackRange)
            {
                // Move towards the player
                Vector3 direction = (player.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

                Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, 
                        Quaternion.LookRotation(lookDir), 
                        Time.deltaTime * 10f);
                }
            }
            else
            {
                //Attack
                if (canAttack)
                {
                    StartCoroutine(Attack());
                    canAttack = false;
                }
            }
            yield return null;
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        Debug.Log("Zombie attacks!");

        PlayerController playerHealth = player.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 30f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.AddScore(10);
    }
}
