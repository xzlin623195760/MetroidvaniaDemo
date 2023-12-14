using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField]
    private float walkSpeed = 1;

    [Header("Vertical Movement Settings"), Space(5)]
    [SerializeField]
    private float jumpForce = 45;

    [SerializeField]
    private int jumpBufferFrames;

    private int jumpBufferCounter = 0;
    [SerializeField]
    private float coyoteTime;

    private float coyoteTimeCounter = 0;
    [SerializeField]
    private int maxAirJumps;

    private int airJumpCounter = 0;

    private float gravity;

    [Header("Dash Settings"), Space(5)]
    [SerializeField]
    private float dashSpeed;

    [SerializeField]
    private float dashTime;

    [SerializeField]
    private float dashCooldown;

    [SerializeField]
    private GameObject dashEffect;

    private bool canDash = true;
    private bool dashed;

    [Header("Attack Settings"), Space(5)]
    [SerializeField]
    private Transform sideAttackTransform;

    [SerializeField]
    private Vector2 sideAttackArea;

    [SerializeField]
    private Transform upAttackTransform;

    [SerializeField]
    private Vector2 upAttackArea;

    [SerializeField]
    private Transform downAttackTransform;

    [SerializeField]
    private Vector2 downAttackArea;

    [SerializeField]
    private LayerMask attackLayer;

    [SerializeField]
    private float damage;

    [SerializeField]
    private GameObject slashEffect;

    private float timeBetweenAttack;
    private float timeSinceAttack;
    private int upSlashAngle = 80;
    private int downSlashAngle = -90;

    [Header("Recoil Settings"), Space(5)]
    [SerializeField]
    private int recoilXSteps = 5;

    [SerializeField]
    private int recoilYSteps = 5;

    [SerializeField]
    private float recoilXSpeed = 100;

    [SerializeField]
    private float recoilYSpeed = 100;

    private int stepsXRecoiled;
    private int stepsYRecoiled;

    [Header("Ground Check Settings"), Space(5)]
    [SerializeField]
    private Transform groundCheckPoint;

    [SerializeField]
    private float groundCheckX = 0.5f;

    [SerializeField]
    private float groundCheckY = 0.2f;

    [SerializeField]
    private LayerMask whatIsGround;

    [Header("Health && Hurt Settings"), Space(5)]
    [SerializeField]
    public int health;

    [SerializeField]
    public int maxHealth;

    [SerializeField]
    private float invincibleTime = 1f; // 受击无敌时间

    [SerializeField]
    private GameObject bloodSpurt; //受击特效

    [SerializeField]
    private float hitFlashSpeed; //受击闪烁速度

    private bool restoreTime; // 判断是否需要恢复时间
    private float restoreSpeed; // 恢复时间速度

    public delegate void OnHealthChangedDelegate();

    [HideInInspector]
    public OnHealthChangedDelegate OnHealthChangedCallback;

    [HideInInspector]
    public PlayerStateList pState;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    // 动画参数
    private string walkingAnimParm = "Walking";
    private string jumpingAnimParm = "Jumping";
    private string dashingAniParm = "Dashing";
    private string attackingAniParm = "Attacking";
    private string takeDamageAniParm = "TakeDamage";

    // 输入参数
    private float xAxis;
    private float yAxis;
    private bool attack = false;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }

    // Start is called before the first frame update
    private void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        gravity = rb.gravityScale;
        Health = maxHealth;
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
        Attack();
        RestoreTimeScale();
        FlashWhileVinciable();
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return;
        Recoil();
    }

    private void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    private void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool(walkingAnimParm, rb.velocity.x != 0 && Grounded());
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
        anim.SetTrigger(dashingAniParm);
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

        anim.SetBool(jumpingAnimParm, !Grounded());
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

    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger(attackingAniParm);

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, sideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, upSlashAngle, upAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(downAttackTransform, downAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, downSlashAngle, downAttackTransform);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackLayer);
        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(
                    damage,
                    (transform.position - objectsToHit[i].GetComponent<Enemy>().transform.position).normalized,
                    _recoilStrength);
            }
        }
    }

    private void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    private void Recoil()
    {
        if (pState.recoilingX)
        {
            rb.velocity = pState.lookingRight ? new Vector2(-recoilXSpeed, 0) : new Vector2(recoilXSpeed, 0);
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            rb.velocity = yAxis < 0 ? new Vector2(rb.velocity.x, recoilYSpeed) : new Vector2(rb.velocity.x, -recoilYSpeed);
            // TODO: 跳跃次数重置
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // 停止后座力
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }
        if (Grounded())
        {
            StopRecoilY();
        }
    }

    private void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    private void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (OnHealthChangedCallback != null)
                {
                    OnHealthChangedCallback.Invoke();
                }
            }
        }
    }

    private IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger(takeDamageAniParm);
        yield return new WaitForSeconds(invincibleTime);
        pState.invincible = false;
    }

    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }

    private void FlashWhileVinciable()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, invincibleTime)) : Color.white;
    }

    private void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    // 被击中时 时间减缓
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    private IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
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
}