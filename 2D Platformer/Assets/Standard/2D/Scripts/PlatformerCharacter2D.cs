using System;
using UnityEngine;


public class PlatformerCharacter2D : MonoBehaviour
{
    [SerializeField] private float MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private bool AirControlLock = false;
    [SerializeField] private LayerMask WhatIsGround;                  // A mask determining what is ground to the character
    [SerializeField] private LayerMask WhatIsWalls;

    public float additiveRollScalar = 1f;

    private Transform GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
    private bool Grounded;            // Whether or not the player is grounded.

    public bool IsGrounded { get { return Grounded; } }

    private Transform ForwardCheck;
    const float k_ForwardRadius = .1f;
    private bool isBlockedForward = false;

    private Transform CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private bool isBlockedUp = false;

    private Animator Anim;            // Reference to the player's animator component.
    private Rigidbody2D Rigidbody2D;
    private bool FacingRight = true;  // For determining which way the player is currently facing.
    private bool canDoubleJump = false;

    public float fallScalar = 1.5f;
    public float jumpScalar = 1f;
    public float crouchJumpScalar = 1.5f;
    public float wallJumpScalar = 0.5f;
    public float wallJumpControlDelay = 0.5f;
    private float airControlTimestamp = 0f;

    private void Awake()
    {
        // Setting up references.
        GroundCheck = transform.Find("GroundCheck");
        CeilingCheck = transform.Find("CeilingCheck");
        ForwardCheck = transform.Find("ForwardCheck");
        Anim = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Grounded = false;
        isBlockedForward = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, k_GroundedRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                Grounded = true;
        }
        Anim.SetBool("Ground", Grounded);

        colliders = Physics2D.OverlapCircleAll(ForwardCheck.position, k_ForwardRadius, WhatIsWalls);
        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject != gameObject)
            {
                isBlockedForward = true;
            }
        }
    }

    
    private void Update()
    {
        Anim.SetBool("Ground", Grounded);
        // Set the vertical animation
        Anim.SetFloat("vSpeed", Rigidbody2D.velocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && GroundCheck && CeilingCheck)
        {
            Gizmos.color = Grounded ? Color.green : Color.red;
            Gizmos.DrawSphere(GroundCheck.position, k_GroundedRadius);
            Gizmos.color = isBlockedUp ? Color.green : Color.red;
            Gizmos.DrawSphere(CeilingCheck.position, k_CeilingRadius);
            Gizmos.color = isBlockedForward ? Color.green : Color.red;
            Gizmos.DrawSphere(ForwardCheck.position, k_ForwardRadius);
        }
    }

    public void Move(float move, bool crouch, bool jump, bool roll)
    {
        isBlockedUp = false;

        // If crouching, check to see if the character can stand up
        if (!crouch && Anim.GetBool("Crouch"))
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(CeilingCheck.position, k_CeilingRadius, WhatIsGround))
            {
                crouch = true;
                isBlockedUp = true;
            }
        }

        // Set whether or not the character is crouching in the animator
        Anim.SetBool("Crouch", crouch);

        if (AirControlLock && Time.time - airControlTimestamp > wallJumpControlDelay)
            AirControlLock = false;

        //only control the player if grounded or airControl is turned on
        if (Grounded || (AirControl && !AirControlLock))
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            move = (crouch ? move * CrouchSpeed : move);

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            Anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character
            if (Mathf.Approximately(move, 0f))
                Rigidbody2D.velocity = Vector2.Lerp(Rigidbody2D.velocity, Vector2.zero, Time.deltaTime);
            else
                Rigidbody2D.velocity = new Vector2(move * MaxSpeed * Mathf.Clamp(additiveRollScalar, 1f, 4f), Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if ( jump && 
                (canDoubleJump ||
                (Grounded && Anim.GetBool("Ground")) ||
                isBlockedForward))
        {
            // Add a vertical force to the player.
            Grounded = false;

            Anim.SetBool("Ground", false);
            if (canDoubleJump || isBlockedForward)
                Rigidbody2D.velocity = Vector3.zero;

            if (crouch)
            {
                Rigidbody2D.AddForce(new Vector2(0f, JumpForce * crouchJumpScalar));
            }
            else if(isBlockedForward)
            {
                if (FacingRight)
                {
                    Rigidbody2D.AddForce(new Vector2(-1f * JumpForce, JumpForce));
                }
                else
                {
                    Rigidbody2D.AddForce(new Vector2(JumpForce, JumpForce));
                }
                Flip();
                AirControlLock = true;
                airControlTimestamp = Time.time;
            }
            else
            {
                Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
            }

            if (!canDoubleJump && !crouch && !isBlockedForward)
                canDoubleJump = true;
            else if (canDoubleJump)
                canDoubleJump = false;
  
        }

        if (Rigidbody2D.velocity.y < 0f && !Input.GetButton("Jump"))
        {
            Rigidbody2D.velocity += Vector2.up * Physics.gravity.y * fallScalar * Time.fixedDeltaTime;
        }

        else if (Rigidbody2D.velocity.y > 0f)
        {
            Rigidbody2D.velocity += Vector2.up * Physics.gravity.y * jumpScalar * Time.fixedDeltaTime;
        }

        if (roll && Grounded && Mathf.Abs(move) > 0.05f)
        {
            Anim.SetTrigger("Roll");
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        FacingRight = !FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
