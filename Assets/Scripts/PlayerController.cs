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
    private const float ROLL_FORCE = 500.0f;
    private const float BOUNCE_SPEED_X = 5.0f;
    private const float BOUNCE_SPEED_Y = 8.5f;
    private const float BOUNCE_TIME = 0.6f;

    // variables
    private enum RollingState { LEFT, RIGHT, NONE, BOUNCING };
    private RollingState rolling;
    private float bounceTimer;
    private float localMaxSpeed;

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
        bounceTimer = 0;
        localMaxSpeed = INIT_ROLL_SPEED;
    }

    // Update is called once per frame
    void Update()
    {

        // check bouncing state (prevents player control)
        if(rolling == RollingState.BOUNCING)
        {
            if (bounceTimer > BOUNCE_TIME)
                rolling = RollingState.NONE;

            bounceTimer += Time.deltaTime;
        }
        // checks rolling state (constrains player movement and checks for bouncing
        else if(rolling != RollingState.NONE)
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

            // check for wall bounce state
            if(rolling == RollingState.LEFT && IsTouchingLeftWall())
            {
                rolling = RollingState.BOUNCING;
                bounceTimer = 0;
                float bounceFactor = (localMaxSpeed - INIT_ROLL_SPEED) / (MAX_ROLL_SPEED - INIT_ROLL_SPEED);
                rb.velocity = new Vector2(BOUNCE_SPEED_X * bounceFactor, BOUNCE_SPEED_Y * bounceFactor);
            }
            else if(rolling == RollingState.RIGHT && IsTouchingRightWall())
            {
                rolling = RollingState.BOUNCING;
                bounceTimer = 0;
                float bounceFactor = (localMaxSpeed - INIT_ROLL_SPEED) / (MAX_ROLL_SPEED - INIT_ROLL_SPEED);
                rb.velocity = new Vector2(-BOUNCE_SPEED_X * bounceFactor, BOUNCE_SPEED_Y * bounceFactor);
            }
            // end rolling if not only holding proper direction
            else if((rolling == RollingState.LEFT && (Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.A))) || (rolling == RollingState.RIGHT && (Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D))) )
            {
                rolling = RollingState.NONE;
            }

            // update local max speed if greater speed is obtained
            if (Mathf.Abs(rb.velocity.x) > localMaxSpeed)
                localMaxSpeed = Mathf.Abs(rb.velocity.x);
        }
        else // not rolling or bouncing (standard walk/jump controls)
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
                    localMaxSpeed = INIT_ROLL_SPEED;
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
                    localMaxSpeed = INIT_ROLL_SPEED;
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
        const float verticalCheckRange = 0.05f;
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

    private bool IsTouchingRightWall()
    {
        // send box cast below player
        const float horizontalCheckRange = 0.05f; // the amount that the boxcast sticks off the right side of the player box
        RaycastHit2D ray = Physics2D.BoxCast(box.bounds.center + new Vector3(box.bounds.extents.x / 2.0f, 0), new Vector2(box.bounds.extents.x, box.bounds.size.y), 0f, Vector2.right, horizontalCheckRange, platforms);

        // determine color for collider debug render
        Color rayColor;
        if (ray.collider != null)
            rayColor = Color.green; // successful collision
        else
            rayColor = Color.red; // no collision

        // render collider box for debug mode
        Debug.DrawRay(box.bounds.center + new Vector3(box.bounds.extents.x, box.bounds.extents.y), Vector2.right * horizontalCheckRange, rayColor);
        Debug.DrawRay(box.bounds.center + new Vector3(box.bounds.extents.x, -box.bounds.extents.y), Vector2.right * horizontalCheckRange, rayColor);
        Debug.DrawRay(box.bounds.center + new Vector3(box.bounds.extents.x + horizontalCheckRange, box.bounds.extents.y), Vector2.down * box.bounds.size.y, rayColor);

        // return if boxcast hit a platform
        return ray.collider != null;
    }

    private bool IsTouchingLeftWall()
    {
        // send box cast below player
        const float horizontalCheckRange = 0.05f; // the amount that the boxcast sticks off the left side of the player box
        RaycastHit2D ray = Physics2D.BoxCast(box.bounds.center + new Vector3(-box.bounds.extents.x / 2.0f, 0), new Vector2(box.bounds.extents.x, box.bounds.size.y), 0f, Vector2.left, horizontalCheckRange, platforms);

        // determine color for collider debug render
        Color rayColor;
        if (ray.collider != null)
            rayColor = Color.green; // successful collision
        else
            rayColor = Color.red; // no collision
        // render collider box for debug mode
        Debug.DrawRay(box.bounds.center + new Vector3(-box.bounds.extents.x, box.bounds.extents.y), Vector2.left * horizontalCheckRange, rayColor);
        Debug.DrawRay(box.bounds.center + new Vector3(-box.bounds.extents.x, -box.bounds.extents.y), Vector2.left * horizontalCheckRange, rayColor);
        Debug.DrawRay(box.bounds.center + new Vector3(-box.bounds.extents.x - horizontalCheckRange, box.bounds.extents.y), Vector2.down * box.bounds.size.y, rayColor);

        // return if boxcast hit a platform
        return ray.collider != null;
    }
}
