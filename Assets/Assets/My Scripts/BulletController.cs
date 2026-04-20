using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //My components
    public Rigidbody RB;

    //How fast do I fly?
    public float Speed = 30;
    //How hard do I knockback things I hit?
    public float Knockback = 10;

    void Start()
    {
        //When I spawn, I fly straight forwards at my Speed
        RB.linearVelocity = transform.forward * Speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        //If I hit something with a rigidbody. . .
        Zombie zombie = other.gameObject.GetComponent<Zombie>();
        if (zombie != null)
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            zombie.TakeDamage(player.bulletDamage);
            if(zombie.Health <= 0f)
            {
                GameManager.Instance.AddScore(10);
                Destroy(zombie);
            }
        }
        

        if (zombie != null)
        {
            //I push them in the direction I'm flying with a power equal to my Knockback stat
            Vector3 dir = (zombie.transform.position - transform.position).normalized;
            zombie.ApplyKnockback(dir * Knockback);
        }
        
        //If I hit anything, I despawn
        
        Destroy(gameObject);
    }

}   