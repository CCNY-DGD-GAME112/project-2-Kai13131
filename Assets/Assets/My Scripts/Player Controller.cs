using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{

    public Camera shoulderCamera;
    public GameObject arm_right;
    public Rigidbody RB;

    public GameObject pistol;
    public GameObject ak47;

    public Transform pistolFirePoint;
    public Transform ak47FirePoint;

    private bool hasAK47 = false;
    private bool usingAK47 = false;
    private bool usingPistol = false;

    public Bullet pistolBullet;
    public Bullet ak47Bullet;

    public float bulletDamage = 30f;

    public float MouseSensitivity = 3;
    public float WalkSpeed = 10;
    public float JumpPower = 7;
    Vector3 move;

    public List<GameObject> Floors;

    public Transform cameraPivot;
    public Vector3 cameraOffset = new Vector3(0.5f, 0f, -3);
    public Vector3 aimCameraOffset = new Vector3(1.5f, 0f, -1.5f);
    public float aimSpeed = 10f;
    private bool isAiming = false;

    float xRotation = 0f;

    public AudioSource audioSource;
    public AudioClip pistolSound;
    public AudioClip ak47Sound;

    public AudioClip playerHurtSound;

    Animator animator;
    public float semi_automatic_FiringShootCooldown = 0.5f; // Time between shots
    public float automatic_FiringShootCooldown = 0.35f; // Time between shots for automatic weapons
    float nextFireTime = 0f; // Time when the player can shoot again

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shoulderCamera.transform.localRotation = Quaternion.identity;
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerMove();
        cameraMove();
        weaponSwitch();
        Firing(); 
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(audioSource != null && playerHurtSound != null)
        {
            audioSource.PlayOneShot(playerHurtSound);
        }
    }

    void weaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipPistol();
            usingPistol = true;
            usingAK47 = false;
        }
        if (hasAK47)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipAK47();
                usingAK47 = true;
                usingPistol = false;
            }
        }

    }

    void EquipPistol()
    {
        pistol.SetActive(true);         
        ak47.SetActive(false);
        nextFireTime = 0f; // Reset fire cooldown when switching weapons
    }

    void EquipAK47()
    {
        pistol.SetActive(false);        
        ak47.SetActive(true);
        nextFireTime = 0f; // Reset fire cooldown when switching weapons
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickupAK47"))
        {
            Destroy(other.gameObject);
            hasAK47 = true;
            EquipAK47();
        }
    }

    void cameraMove()
    {
        
        //If my mouse goes left/right my body moves left/right
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        

        transform.Rotate(0, mouseX, 0);

        xRotation += mouseY * MouseSensitivity / 2 * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -30, 30);
        
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        shoulderCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        arm_right.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Apply camera offset(shoulder view)

        //Check aiming input
        isAiming = Input.GetMouseButton(1); // Right mouse button for aiming

        //Smoothly transition to aim camera offset
        Vector3 targetOffset;
        if (isAiming)
        {
            targetOffset = aimCameraOffset;
            arm_right.transform.localRotation = Quaternion.Euler(xRotation + 270, 0f, 0f);
        }
        else
        {
            targetOffset = cameraOffset;
        }
        shoulderCamera.transform.localPosition = Vector3.Lerp(
            shoulderCamera.transform.localPosition, 
            targetOffset, 
            Time.deltaTime * aimSpeed
            );
    }

    void playerMove()
    {
        if (WalkSpeed > 0)
        {
            //My temp velocity variable

            
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //transform.forward/right are relative to the direction my body is facing
            Vector3 move = transform.forward * z + transform.right * x;
            //I reduce my total movement to 1 and then multiply it by my speed
            move = move * WalkSpeed;

            //If I hit jump and am on the ground, I jump
            if (JumpPower > 0 && Input.GetKeyDown(KeyCode.Space) && OnGround())
                move.y = JumpPower;
            else  //Otherwise, my Y velocity is whatever it was last frame
                move.y = RB.linearVelocity.y;

            //Plug my calculated velocity into the rigidbody
            RB.linearVelocity = move;

            float speed = new Vector2(x,z).magnitude;
            animator.SetFloat("speed", speed);
        }
    }

    void Firing()
    {
        if (pistol.activeSelf)
        {
            if (Input.GetMouseButton(1))
            {

                animator.SetBool("isAiming", true);
                if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
                {
                    nextFireTime = Time.time + semi_automatic_FiringShootCooldown;

                    animator.SetTrigger("shoot");

                    audioSource.PlayOneShot(pistolSound);

                    Ray ray = new Ray(shoulderCamera.transform.position, shoulderCamera.transform.forward);
                    RaycastHit hit;
                    Vector3 targetPoint;

                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        targetPoint = hit.point;
                    }
                    else
                    {
                        targetPoint = ray.GetPoint(100f);
                    }

                    Transform currentFirePoint = pistolFirePoint;

                    Vector3 direction = (targetPoint - shoulderCamera.transform.position).normalized;

                    GameObject bulletObj = Instantiate(
                        pistolBullet.gameObject,
                        currentFirePoint.position,
                        Quaternion.LookRotation(direction)
                        );
                }
                else
                {
                    animator.SetBool("shoot", false);
                }
            }
            else
            {
                animator.SetBool("isAiming", false);
            }
        }
        if (ak47.activeSelf)
        {
            if (Input.GetMouseButton(1))
            {
                animator.SetBool("isAiming", true);
                if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
                {
                    nextFireTime = Time.time + automatic_FiringShootCooldown;
                    audioSource.PlayOneShot(ak47Sound);
                    animator.SetTrigger("shoot");

                    Ray ray = new Ray(shoulderCamera.transform.position, shoulderCamera.transform.forward);
                    RaycastHit hit;
                    Vector3 targetPoint;

                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        targetPoint = hit.point;
                    }
                    else
                    {
                        targetPoint = ray.GetPoint(100f);
                    }

                    Transform currentFirePoint = ak47FirePoint;

                    Vector3 direction = (targetPoint - shoulderCamera.transform.position).normalized;

                    GameObject bulletObj = Instantiate(
                        ak47Bullet.gameObject,
                        currentFirePoint.position,
                        Quaternion.LookRotation(direction)
                        );
                }
                else
                {
                    animator.SetBool("shoot", false);
                }
            }
            else
            {
                animator.SetBool("isAiming", false);
            }
        }
    }

    public Transform eyes;
    public void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(eyes.position, eyes.forward * 100);
            Gizmos.DrawCube(eyes.position + eyes.forward * 100, Vector3.one * 1f);
        }
    }

    public bool OnGround()
    {
        return Floors.Count > 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        //If I touch something and it's not already in my list of things I'm touching. . .
        //Add it to the list
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle"))
        {
            if (!Floors.Contains(other.gameObject))
                Floors.Add(other.gameObject);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        //When I stop touching something, remove it from the list of things I'm touching
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle"))
        {
            Floors.Remove(other.gameObject);

        }
    }

    protected override void Die()
    {
        Debug.Log("Player has died. Game Over.");
        //Here you could add code to show a game over screen, restart the level, etc.
        GameManager.Instance.GameOver();
    }

    public void Heal(float amount)
    {
        Health += amount;
        if(Health > 100f)
            Health = 100f;
        GameManager.Instance.playerHealthUpdate(Health);
    }
}
