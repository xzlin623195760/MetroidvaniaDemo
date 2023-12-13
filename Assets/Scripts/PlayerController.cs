using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField]
    private float walkSpeed = 1;

    [Space(5)]
    [Header("Vertical Movement Settings")]
    [SerializeField]
    private float jumpForce = 45;
    private int jumpBufferCounter = 0;
    [SerializeField]
    private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField]
    private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField]
    private int maxAirJumps;

    [Space(5)]
    [Header("Ground Check Settings")]
    [SerializeField]
    private Transform groundCheckPoint;
    [SerializeField]
    private float groundCheckX = 0.5f;
    [SerializeField]
    private float groundCheckY = 0.2f;
    [SerializeField]
    private LayerMask whatIsGround;

    [Space(5)]
    [Header("Dash Settings")]
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private GameObject dashEffect;

    private PlayerStateList pState;

    private Rigidbody2D rb;

    private Animator anim;
    private string WalkingAnimParm = "Walking";
    private string JumpingAnimParm = "Jumping";
    private string DashingAniParm = "Dashing";

    private float xAxis;
    private bool canDash = true;
    private bool dashed;
    private float gravity;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    private void Update()
    {
        GetInputs();
        UpdateJumpVariables();

        if (pState.dashing) return;
        Flip();
        Move();
        Jump();
        StartDash();
    }

    private void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool(WalkingAnimParm, rb.velocity.x != 0 && Grounded());
    }

    private void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger(DashingAniParm);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if (Grounded() && dashEffect != null)
        {
            Instantiate(dashEffect, transform);
        }
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        anim.SetBool(JumpingAnimParm, !Grounded());
    }

    private void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}