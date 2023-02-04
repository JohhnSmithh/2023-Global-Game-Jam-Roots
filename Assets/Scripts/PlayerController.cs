using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // constants
    private const float WALK_SPEED = 5.0f;
    private const float INIT_JUMP_SPEED = 5.0f;
    private const float INIT_ROLL_SPEED = 3.0f;
    private const float MAX_ROLL_SPEED = 10.0f;
    private const float ROLL_FORCE = 1000.0f;

    // variables
    private enum RollingState { LEFT, RIGHT, NONE };
    private RollingState rolling;

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

        rolling = RollingState.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        if(rolling != RollingState.NONE)
        {
            // apply roll force if rolling
            rb.AddForce(new Vector2((rolling==RollingState.LEFT?-1:1)*ROLL_FORCE*Time.deltaTime, 0), ForceMode2D.Force);

            // cap rolling speed
            if(rb.velocity.x > MAX_ROLL_SPEED)
            {
                rb.velocity = new Vector2(MAX_ROLL_SPEED, rb.velocity.y);
            }
            else if(rb.velocity.x < -MAX_ROLL_SPEED)
            {
                rb.velocity = new Vector2(-MAX_ROLL_SPEED, rb.velocity.y);
            }

            // end rolling if not only holding proper direction
            if((rolling == RollingState.LEFT && (Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.A))) || (rolling == RollingState.RIGHT && (Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D))) )
            {
                rolling = RollingState.NONE;
            }
        }
        else // not rolling
        {
            // Walking
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                rb.velocity = new Vector2(WALK_SPEED, rb.velocity.y);

                // enter right roll
                if(Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded())
                {
                    rb.velocity = new Vector2(INIT_ROLL_SPEED, rb.velocity.y);
                    rolling = RollingState.RIGHT;
                }
            }
            else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector2(-WALK_SPEED, rb.velocity.y);

                // enter left roll
                if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded())
                {
                    rb.velocity = new Vector2(-INIT_ROLL_SPEED, rb.velocity.y);
                    rolling = RollingState.LEFT;
                }
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
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
