using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // constants
    private const float WALK_SPEED = 5.0f;
    private const float INIT_JUMP_SPEED = 5.0f;

    // Unity variables
    Rigidbody2D rb;
    BoxCollider2D box;

    // Layer masks
    [SerializeField] private LayerMask platforms;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Walking
        if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(WALK_SPEED, rb.velocity.y);
        }
        else if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(-WALK_SPEED, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Jumping
        if(IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, INIT_JUMP_SPEED);
        }
    }

    private bool IsGrounded()
    {
        const float verticalCheckRange = 0.1f;
        RaycastHit2D ray = Physics2D.BoxCast(box.bounds.center - new Vector3(0, box.bounds.size.y / 4.0f), new Vector2(box.bounds.size.x, box.bounds.extents.y), 0f, Vector2.down, verticalCheckRange, platforms);

        // set color for debug mode
        Color rayColor;
        if (ray.collider != null)
            rayColor = Color.green; // successful collision
        else
            rayColor = Color.red; // no collision
        // render collider box for debug mode
        Debug.DrawRay(box.bounds.center + new Vector3(-box.bounds.extents.x, -box.bounds.extents.y), Vector2.down * verticalCheckRange, rayColor);
        Debug.DrawRay(box.bounds.center + new Vector3(box.bounds.extents.x, -box.bounds.extents.y), Vector2.down * verticalCheckRange, rayColor);
        Debug.DrawRay(box.bounds.center + new Vector3(-box.bounds.extents.x, -box.bounds.extents.y - verticalCheckRange), Vector2.right * box.bounds.size.x, rayColor);

        return ray.collider != null;
    }
}
