using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Camera shoulderCamera;
    public GameObject arm_right;
    public Rigidbody RB;
    public Projectile3DController ProjectilePrefab;

    public float MouseSensitivity = 3;
    public float WalkSpeed = 10;
    public float JumpPower = 7;

    public List<GameObject> Floors;

    public Transform cameraPivot;
    public Vector3 cameraOffset = new Vector3(0.5f, 0f, -3);
    public Vector3 aimCameraOffset = new Vector3(1.5f, 0f, -1.5f);
    public float aimSpeed = 10f;
    private bool isAiming = false;

    float xRotation = 0f;

    public AudioSource audioSource;
    public AudioClip ShootSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shoulderCamera.transform.localRotation = Quaternion.identity;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerMove();
        cameraMove();
        Shoot();
                      
    }

    void cameraMove()
    {
        
        //If my mouse goes left/right my body moves left/right
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        

        transform.Rotate(0, mouseX, 0);

        xRotation += mouseY * MouseSensitivity * Time.deltaTime;
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
            Vector3 move = Vector3.zero;

            //transform.forward/right are relative to the direction my body is facing
            if (Input.GetKey(KeyCode.W))
                move += transform.forward;
            if (Input.GetKey(KeyCode.S))
                move -= transform.forward;
            if (Input.GetKey(KeyCode.A))
                move -= transform.right;
            if (Input.GetKey(KeyCode.D))
                move += transform.right;
            //I reduce my total movement to 1 and then multiply it by my speed
            move = move.normalized * WalkSpeed;

            //If I hit jump and am on the ground, I jump
            if (JumpPower > 0 && Input.GetKeyDown(KeyCode.Space) && OnGround())
                move.y = JumpPower;
            else  //Otherwise, my Y velocity is whatever it was last frame
                move.y = RB.linearVelocity.y;

            //Plug my calculated velocity into the rigidbody
            RB.linearVelocity = move;
        }
    }

    void Shoot()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(ShootSound);

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

            Vector3 direction = (targetPoint - shoulderCamera.transform.position).normalized;

            GameObject bulletObj = Instantiate(
                ProjectilePrefab.gameObject,
                shoulderCamera.transform.position,
                Quaternion.LookRotation(direction)
                );
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
        if (!Floors.Contains(other.gameObject))
            Floors.Add(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        //When I stop touching something, remove it from the list of things I'm touching
        Floors.Remove(other.gameObject);
    }
}
