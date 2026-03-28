using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal,0,vertical);
        Debug.DrawRay(transform.position, dir, Color.red);
        player.SimpleMove(dir * 2);
    }
}
