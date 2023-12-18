using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /*********************************** 地面检测 ********************************************/
    [Header("Ground Check Settings"), Space(5)]
    [SerializeField]
    private Transform groundCheckPoint; // 地面检测点

    [SerializeField]
    private float groundCheckX = 0.5f; // 地面检测尺寸

    [SerializeField]
    private float groundCheckY = 0.2f; // 地面检测尺寸

    [SerializeField]
    private LayerMask whatIsGround; // 地面检测对象层

    /*********************************** 相机设置 ********************************************/
    [Header("Camera Stuff"), Space(5)]
    [SerializeField]
    private float playerFallSpeedTheshold = -10;

    /*********************************** 水平方向移动 ********************************************/
    [Header("Horizontal Movement Settings")]
    [SerializeField]
    private float walkSpeed = 1; // 移动速度

    /*********************************** 垂直方向移动 ********************************************/
    [Header("Vertical Movement Settings"), Space(5)]
    [SerializeField]
    private float jumpForce = 45; // 跳跃速度

    [SerializeField]
    private float minJumpSpeed = 3; // 最小跳跃速度

    [SerializeField]
    private int jumpBufferFrames; // 跳跃指令缓存帧数
    private int jumpBufferCounter = 0; //跳跃指令缓存计数器

    [SerializeField]
    private float coyoteTime; // 土狼时间
    private float coyoteTimeCounter = 0; // 土狼时间计时器

    [SerializeField]
    private int maxAirJumps; // 最大跳跃次数
    private int airJumpCounter = 0; // 跳跃次数计数器

    private float gravity; //存储重力

    /*********************************** 冲刺 ********************************************/
    [Header("Dash Settings"), Space(5)]
    [SerializeField]
    private float dashSpeed; // 冲刺速度

    [SerializeField]
    private float dashTime; // 冲刺时间

    [SerializeField]
    private float dashCooldown; // 冲刺冷却时间

    [SerializeField]
    private GameObject dashEffect; // 冲刺灰尘特效

    private bool canDash = true; // 判断是否能冲刺
    private bool dashed; // 冲刺是否结束

    /*********************************** 攻击 ********************************************/
    [Header("Attack Settings"), Space(5)]
    [SerializeField]
    private Transform sideAttackTransform; // 左右攻击框位置

    [SerializeField]
    private Vector2 sideAttackArea; // 左右攻击框尺寸

    [SerializeField]
    private Transform upAttackTransform; // 向上攻击框位置

    [SerializeField]
    private Vector2 upAttackArea; // 向上攻击框尺寸

    [SerializeField]
    private Transform downAttackTransform; // 向下攻击框位置

    [SerializeField]
    private Vector2 downAttackArea; // 向下攻击框尺寸

    [SerializeField]
    private LayerMask attackLayer; // 攻击检测对象层

    [SerializeField]
    private float damage; // 攻击伤害值

    [SerializeField]
    private GameObject slashEffect; // 刀光特效
    private int upSlashAngle = 80; // 刀光特效角度
    private int downSlashAngle = -90; // 刀光特效角度

    private float timeBetweenAttack; // 攻击间隔时间 防止单次攻击多段伤害
    private float timeSinceAttack; // 攻击间隔时间计时器

    /*********************************** 后座力 ********************************************/
    [Header("Recoil Settings"), Space(5)]
    [SerializeField]
    private int recoilXSteps = 5; // 后座力持续时间

    [SerializeField]
    private int recoilYSteps = 5;

    [SerializeField]
    private float recoilXSpeed = 100; // 后座力速度

    [SerializeField]
    private float recoilYSpeed = 100;

    private int stepsXRecoiled; // 后座力时间计时器
    private int stepsYRecoiled;

    /*********************************** 生命和受伤 ********************************************/
    [Header("Health && Hurt Settings"), Space(5)]
    [SerializeField]
    public int health; // 当前生命值

    [SerializeField]
    public int maxHealth; // 最大生命值

    [SerializeField]
    private float invincibleTime = 1f; // 受击无敌时间

    [SerializeField]
    private GameObject bloodSpurt; // 受击特效

    [SerializeField]
    private bool canFlash = true; // 是否开启闪烁

    [SerializeField]
    private float flashSpeed = 0.1f; // 闪烁速度

    private bool restoreTime; // 判断是否需要恢复时间
    private float restoreSpeed; // 恢复时间速度

    [SerializeField]
    private float timeToHeal; // 治愈时间
    private float healTimer; // 治愈计时器

    public delegate void OnHealthChangedDelegate();

    [HideInInspector]
    public OnHealthChangedDelegate OnHealthChangedCallback;  // 生命值变化回调 UI调用

    /*********************************** 法力 ********************************************/
    [Header("Mana Settings"), Space(5)]
    [SerializeField]
    private float mana; // 法力值
    [SerializeField]
    private float manaDrainSpeed; // 法力消耗速度
    [SerializeField]
    private float manaGain; // 法力获取值
    [SerializeField]
    private Image manaStorage;
    private bool isHalfMana;

    /*********************************** 法术 ********************************************/
    [Header("Spell Settings"), Space(5)]
    [SerializeField]
    private float manaSpellCast = 0.3f; // 法术所需法力消耗值
    [SerializeField]
    private float timeBetweenCast = 0.5f; // 防止无限释放法术
    private float timeSinceCast;

    [SerializeField]
    private float spellDamage; // 该伤害仅用于上吼和下砸
    [SerializeField]
    private float downSpellForce; // 下砸速度

    [SerializeField]
    private GameObject sideSpellFireball;
    [SerializeField]
    private GameObject upSpellExplosion;
    [SerializeField]
    private GameObject downSpellFireball;

    [SerializeField]
    private float castBtnUpTime = 0.05f; // 法术键松开判定时间
    private float castOrHealTimer; // 法术键按下的时间计时器

    /*******************************************************************************/
    [HideInInspector]
    public PlayerStateList pState;
    [HideInInspector]
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    // 动画参数
    private string walkingAnimParm = "Walking";
    private string jumpingAnimParm = "Jumping";
    private string dashingAniParm = "Dashing";
    private string attackingAniParm = "Attacking";
    private string takeDamageAniParm = "TakeDamage";
    private string healingAniParm = "Healing";
    private string castingAniParm = "Casting";
    private string deathAniParm = "Death";

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
        DontDestroyOnLoad(gameObject);
    }

    private void OnDrawGizmos()
    {
        // 攻击碰撞框编辑器
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
        Mana = mana;
        manaStorage.fillAmount = Mana;
    }

    // Update is called once per frame
    private void Update()
    {
        if (pState.cutScene) return;
        if (pState.alive)
        {
            GetInputs();
            Heal();
        }
        UpdateJumpVariables();
        UpdateCameraYDampForPlayerFall();
        RestoreTimeScale();

        if (pState.dashing || pState.healing) return;
        if (pState.alive)
        {
            Flip();
            Move();
            Jump();
            StartDash();
            Attack();
            CastSpell();
        }
        FlashWhileVinciable();

        // TODO: test
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Death());
        }
    }

    private void FixedUpdate()
    {
        if (pState.cutScene) return;
        if (pState.dashing || pState.healing) return;
        Recoil();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        // 下砸和上吼的碰撞伤害
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");

        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        else
        {
            castOrHealTimer = 0;
        }
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
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
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
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            pState.jumping = true;
        }

        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;
            airJumpCounter++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > minJumpSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
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
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
                Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(slashEffect, sideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, upSlashAngle, upAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(downAttackTransform, downAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, downSlashAngle, downAttackTransform);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackLayer);
        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit
                    (damage, _recoilDir, _recoilStrength);
                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
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
            // 跳跃次数重置
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
        if (pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }
    }

    private void FlashWhileVinciable()
    {
        if (pState.invincible)
        {
            if (Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    private IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(flashSpeed);
        canFlash = true;
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

    /// <summary>
    /// 治愈
    /// </summary>
    /// <returns></returns>
    private void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > castBtnUpTime && Mana > 0 && Health < maxHealth && !pState.jumping && !pState.dashing)
        {
            Debug.Log($"castTimer {castOrHealTimer}");

            pState.healing = true;
            anim.SetBool(healingAniParm, true);

            // 回复生命值
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                healTimer = 0;
                Health++;
            }

            // 消耗法力值
            Mana -= manaDrainSpeed * Time.deltaTime;
        }
        else
        {
            pState.healing = false;
            healTimer = 0;
            anim.SetBool(healingAniParm, false);
        }
    }

    private float Mana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                if (!isHalfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }
                manaStorage.fillAmount = mana;
            }
        }
    }

    /// <summary>
    /// 释放法术
    /// </summary>
    private void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= castBtnUpTime && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCast)
        {
            Debug.Log($"castTimer {castOrHealTimer}");
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            downSpellFireball.SetActive(false);
        }
        // 处于下砸期间 下落速度转化为下砸速度
        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    private IEnumerator CastCoroutine()
    {
        anim.SetBool(castingAniParm, true);
        // 在动画的n秒处释放
        yield return new WaitForSeconds(0.15f);

        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            GameObject _fireball = Instantiate(sideSpellFireball, sideAttackTransform.position, Quaternion.identity);
            // 翻转火球
            _fireball.transform.eulerAngles = pState.lookingRight ? Vector3.zero : new Vector2(_fireball.transform.eulerAngles.x, 180);
            // 加上后座力
            pState.recoilingX = true;
        }
        else if (yAxis > 0)
        {
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }
        else if (yAxis < 0 && !Grounded())
        {
            downSpellFireball.SetActive(true);
        }
        //消耗法力
        Mana -= manaSpellCast;
        yield return new WaitForSeconds(0.35f);

        anim.SetBool(castingAniParm, false);
        pState.casting = false;
    }

    private IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger(deathAniParm);

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActiveDeathScreen());

        yield return new WaitForSeconds(0.9f);
        Instantiate(GameManager.Instance.shade, transform.position, Quaternion.identity);
    }

    // 复活
    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            isHalfMana = true;
            UIManager.Instance.SwitchManaState(UIManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("Player_Idle");
        }
    }

    public void RestoreMana()
    {
        isHalfMana = false;
        UIManager.Instance.SwitchManaState(UIManager.ManaState.FullMana);
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        if (_exitDir.y != 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }
        Flip();

        yield return new WaitForSeconds(_delay);
        pState.cutScene = false;
    }

    /// <summary>
    /// 是否在地面
    /// </summary>
    /// <returns></returns>
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

    private void UpdateCameraYDampForPlayerFall()
    {
        if (rb.velocity.y < playerFallSpeedTheshold && !CameraManager.Instance.isLerpingYDamping && !CameraManager.Instance.hasLerpingYDamping)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }

        if (rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamping && CameraManager.Instance.hasLerpingYDamping)
        {
            CameraManager.Instance.hasLerpingYDamping = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }
}