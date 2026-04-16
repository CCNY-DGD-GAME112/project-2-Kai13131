using UnityEngine;
using System.Collections;

public class Zombie : Character
{
    public Transform player;
    public Rigidbody rb;

    public float moveSpeed = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    public float detectionRange = 50f;

    private bool canAttack = true;

    private Vector3 currentDirection;
    private float blockMemory = 0f;

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

            if(distanceToPlayer > detectionRange)
            {
                // Idle
                yield return null;
                continue;
            }
            else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
            {
                // Move towards the player
                Vector3 targetDirection = (player.position - transform.position).normalized;
                if (currentDirection == Vector3.zero)
                {
                    currentDirection = targetDirection;
                }
                currentDirection = Vector3.Slerp(currentDirection, targetDirection,Time.deltaTime * 3f);

                if (isBlocked(currentDirection))
                {
                    blockMemory += Time.deltaTime;
                    
                    if(blockMemory > 0.1f)
                    {
                        Vector3 left = Quaternion.Euler(0, -45, 0) * currentDirection;
                        Vector3 right = Quaternion.Euler(0, 45, 0) * currentDirection;

                        if (!isBlocked(left))
                        {
                            currentDirection = left;
                        }
                        else if (!isBlocked(right))
                        {
                            currentDirection = right;
                        }
                        else
                        {
                            currentDirection = -currentDirection;
                        }
                        blockMemory = 0f;
                    }
                }
                else
                {
                    blockMemory = 0f;
                }
                rb.MovePosition(rb.position + currentDirection * moveSpeed * Time.deltaTime);

                Vector3 lookDir = new Vector3(currentDirection.x, 0, currentDirection.z);
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
                if (!canAttack)
                {
                    yield return null;
                    continue;
                }
                StartCoroutine(Attack());
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
            GameManager.Instance.playerHealthUpdate(playerHealth.getHealth());
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.zombies.Remove(gameObject);
    }

    bool isBlocked(Vector3 direction)
    {
        
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        return Physics.SphereCast(origin, 0.3f, direction, out RaycastHit hit, 1.2f);
    }
}
