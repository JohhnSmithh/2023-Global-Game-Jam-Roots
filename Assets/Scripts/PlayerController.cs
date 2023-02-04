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
    private const float INTERACT_WINDOW = 0.1f;
    private const float DIALOGUE_TIME = 5.0f;

    // variables
    private enum RollingState { LEFT, RIGHT, NONE, BOUNCING };
    private RollingState rolling;
    private float bounceTimer;
    private float localMaxSpeed;
    private bool isAttemptingInteract;
    private float interactTimer;
    private bool isInDialogue;
    private float dialogueTimer;

    // Unity variables
    Rigidbody2D rb;
    BoxCollider2D box;
    CanvasManager canvasManager;

    // Layer masks
    [SerializeField] private LayerMask platforms;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        // assign player position based on spawn point
        transform.position = GameManager.instance.GetSpawnPoint();

        rolling = RollingState.NONE;
        bounceTimer = 0;
        localMaxSpeed = INIT_ROLL_SPEED;
        isAttemptingInteract = false;
        interactTimer = 0;
        isInDialogue = false;
        dialogueTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInDialogue) // dialogue, virtually no controls, only free fall with no horizontal
        {
            if (dialogueTimer > DIALOGUE_TIME)
            {
                isInDialogue = false;
                canvasManager.ExitDialogue();
            }

            dialogueTimer += Time.deltaTime;

            // stop all horiontal movement while in dialogue
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        // check bouncing state (prevents player control)
        else if (rolling == RollingState.BOUNCING)
        {
            if (bounceTimer > BOUNCE_TIME)
                rolling = RollingState.NONE;

            bounceTimer += Time.deltaTime;
        }
        // checks rolling state (constrains player movement and checks for bouncing
        else if (rolling != RollingState.NONE)
        {
            // apply roll force if rolling
            rb.AddForce(new Vector2((rolling == RollingState.LEFT ? -1 : 1) * ROLL_FORCE * Time.deltaTime, 0), ForceMode2D.Force);

            // cap rolling speed
            if (rb.velocity.x > MAX_ROLL_SPEED)
            {
                rb.velocity = new Vector2(MAX_ROLL_SPEED, rb.velocity.y);
            }
            else if (rb.velocity.x < -MAX_ROLL_SPEED)
            {
                rb.velocity = new Vector2(-MAX_ROLL_SPEED, rb.velocity.y);
            }

            // check for wall bounce state
            if (rolling == RollingState.LEFT && IsTouchingLeftWall())
            {
                rolling = RollingState.BOUNCING;
                bounceTimer = 0;
                float bounceFactor = (localMaxSpeed - INIT_ROLL_SPEED) / (MAX_ROLL_SPEED - INIT_ROLL_SPEED);
                rb.velocity = new Vector2(BOUNCE_SPEED_X * bounceFactor, BOUNCE_SPEED_Y * bounceFactor);
            }
            else if (rolling == RollingState.RIGHT && IsTouchingRightWall())
            {
                rolling = RollingState.BOUNCING;
                bounceTimer = 0;
                float bounceFactor = (localMaxSpeed - INIT_ROLL_SPEED) / (MAX_ROLL_SPEED - INIT_ROLL_SPEED);
                rb.velocity = new Vector2(-BOUNCE_SPEED_X * bounceFactor, BOUNCE_SPEED_Y * bounceFactor);
            }
            // end rolling if not only holding proper direction
            else if ((rolling == RollingState.LEFT && (Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.A))) || (rolling == RollingState.RIGHT && (Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D))))
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
                if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded())
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
            else // no inputs -> just kinda stop moving like Hollow Knight (no momentum/sliding)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        // Jumping
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space) && !isInDialogue)
        {
            rb.velocity = new Vector2(rb.velocity.x, INIT_JUMP_SPEED);
        }

        // interacting input
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isAttemptingInteract = true;
            interactTimer = 0;
        }
        // timer system necessary because a straight input check in OnTriggerStay updates once per physics update and therefore could miss an input
        if (isAttemptingInteract)
        {
            if (interactTimer > INTERACT_WINDOW)
                isAttemptingInteract = false;

            interactTimer += Time.deltaTime;
        }

        // Notepad controls (always work unless talking)
        if(!isInDialogue)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                canvasManager.NoteLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                canvasManager.NoteRight();
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                canvasManager.NoteDown();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("CodeNPC") && isAttemptingInteract && !collision.gameObject.GetComponent<CodeNPCData>().hasSpoken)
        {
            // only permits one interaction
            collision.gameObject.GetComponent<CodeNPCData>().hasSpoken = true;

            // display message here
            canvasManager.DisplayDialogue(collision.gameObject.GetComponent<CodeNPCData>().GetName(), collision.gameObject.GetComponent<CodeNPCData>().GetMessage());

            // start timer for being locked in dialogue
            isInDialogue = true;
            dialogueTimer = 0;
            rb.velocity = new Vector2(0, 0); // stop player in place upon entering dialogue

            // close notes if opened
            canvasManager.NoteDown();
        }
        else if(collision.CompareTag("Transition") && isAttemptingInteract)
        {
            GameManager.instance.TransitionScene(collision.gameObject.GetComponent<TransitionData>());
        }
    }

    #region COLLISIONS

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

    // returns if right side of player is touching wall
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

    // returns if left side of player is touching wall
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

    #endregion

    #region CRINGE

    // silly workaround to guarantee that PlayerController has a reference to the CanvasManager that is instantiated in the Start of CameraController
    public void InstantiateCanvasManager(CanvasManager manager)
    {
        canvasManager = manager;
    }

    #endregion
}
