using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /*********************************** ������ ********************************************/
    [Header("Ground Check Settings"), Space(5)]
    [SerializeField]
    private Transform groundCheckPoint; // �������

    [SerializeField]
    private float groundCheckX = 0.5f; // ������ߴ�

    [SerializeField]
    private float groundCheckY = 0.2f; // ������ߴ�

    [SerializeField]
    private LayerMask whatIsGround; // ����������

    /*********************************** ������� ********************************************/
    [Header("Camera Stuff"), Space(5)]
    [SerializeField]
    private float playerFallSpeedTheshold = -10;

    /*********************************** ˮƽ�����ƶ� ********************************************/
    [Header("Horizontal Movement Settings")]
    [SerializeField]
    private float walkSpeed = 1; // �ƶ��ٶ�

    /*********************************** ��ֱ�����ƶ� ********************************************/
    [Header("Vertical Movement Settings"), Space(5)]
    [SerializeField]
    private float jumpForce = 45; // ��Ծ�ٶ�

    [SerializeField]
    private float minJumpSpeed = 3; // ��С��Ծ�ٶ�

    [SerializeField]
    private int jumpBufferFrames; // ��Ծָ���֡��
    private int jumpBufferCounter = 0; //��Ծָ��������

    [SerializeField]
    private float coyoteTime; // ����ʱ��
    private float coyoteTimeCounter = 0; // ����ʱ���ʱ��

    [SerializeField]
    private int maxAirJumps; // �����Ծ����
    private int airJumpCounter = 0; // ��Ծ����������

    private float gravity; //�洢����

    /*********************************** ��� ********************************************/
    [Header("Dash Settings"), Space(5)]
    [SerializeField]
    private float dashSpeed; // ����ٶ�

    [SerializeField]
    private float dashTime; // ���ʱ��

    [SerializeField]
    private float dashCooldown; // �����ȴʱ��

    [SerializeField]
    private GameObject dashEffect; // ��̻ҳ���Ч

    private bool canDash = true; // �ж��Ƿ��ܳ��
    private bool dashed; // ����Ƿ����

    /*********************************** ���� ********************************************/
    [Header("Attack Settings"), Space(5)]
    [SerializeField]
    private Transform sideAttackTransform; // ���ҹ�����λ��

    [SerializeField]
    private Vector2 sideAttackArea; // ���ҹ�����ߴ�

    [SerializeField]
    private Transform upAttackTransform; // ���Ϲ�����λ��

    [SerializeField]
    private Vector2 upAttackArea; // ���Ϲ�����ߴ�

    [SerializeField]
    private Transform downAttackTransform; // ���¹�����λ��

    [SerializeField]
    private Vector2 downAttackArea; // ���¹�����ߴ�

    [SerializeField]
    private LayerMask attackLayer; // �����������

    [SerializeField]
    private float damage; // �����˺�ֵ

    [SerializeField]
    private GameObject slashEffect; // ������Ч
    private int upSlashAngle = 80; // ������Ч�Ƕ�
    private int downSlashAngle = -90; // ������Ч�Ƕ�

    private float timeBetweenAttack; // �������ʱ�� ��ֹ���ι�������˺�
    private float timeSinceAttack; // �������ʱ���ʱ��

    /*********************************** ������ ********************************************/
    [Header("Recoil Settings"), Space(5)]
    [SerializeField]
    private int recoilXSteps = 5; // ����������ʱ��

    [SerializeField]
    private int recoilYSteps = 5;

    [SerializeField]
    private float recoilXSpeed = 100; // �������ٶ�

    [SerializeField]
    private float recoilYSpeed = 100;

    private int stepsXRecoiled; // ������ʱ���ʱ��
    private int stepsYRecoiled;

    /*********************************** ���������� ********************************************/
    [Header("Health && Hurt Settings"), Space(5)]
    [SerializeField]
    public int health; // ��ǰ����ֵ

    [SerializeField]
    public int maxHealth; // �������ֵ

    [SerializeField]
    private float invincibleTime = 1f; // �ܻ��޵�ʱ��

    [SerializeField]
    private GameObject bloodSpurt; // �ܻ���Ч

    [SerializeField]
    private bool canFlash = true; // �Ƿ�����˸

    [SerializeField]
    private float flashSpeed = 0.1f; // ��˸�ٶ�

    private bool restoreTime; // �ж��Ƿ���Ҫ�ָ�ʱ��
    private float restoreSpeed; // �ָ�ʱ���ٶ�

    [SerializeField]
    private float timeToHeal; // ����ʱ��
    private float healTimer; // ������ʱ��

    public delegate void OnHealthChangedDelegate();

    [HideInInspector]
    public OnHealthChangedDelegate OnHealthChangedCallback;  // ����ֵ�仯�ص� UI����

    /*********************************** ���� ********************************************/
    [Header("Mana Settings"), Space(5)]
    [SerializeField]
    private float mana; // ����ֵ
    [SerializeField]
    private float manaDrainSpeed; // ���������ٶ�
    [SerializeField]
    private float manaGain; // ������ȡֵ
    [SerializeField]
    private Image manaStorage;
    private bool isHalfMana;

    /*********************************** ���� ********************************************/
    [Header("Spell Settings"), Space(5)]
    [SerializeField]
    private float manaSpellCast = 0.3f; // �������跨������ֵ
    [SerializeField]
    private float timeBetweenCast = 0.5f; // ��ֹ�����ͷŷ���
    private float timeSinceCast;

    [SerializeField]
    private float spellDamage; // ���˺��������Ϻ������
    [SerializeField]
    private float downSpellForce; // �����ٶ�

    [SerializeField]
    private GameObject sideSpellFireball;
    [SerializeField]
    private GameObject upSpellExplosion;
    [SerializeField]
    private GameObject downSpellFireball;

    [SerializeField]
    private float castBtnUpTime = 0.05f; // �������ɿ��ж�ʱ��
    private float castOrHealTimer; // ���������µ�ʱ���ʱ��

    /*******************************************************************************/
    [HideInInspector]
    public PlayerStateList pState;
    [HideInInspector]
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    // ��������
    private string walkingAnimParm = "Walking";
    private string jumpingAnimParm = "Jumping";
    private string dashingAniParm = "Dashing";
    private string attackingAniParm = "Attacking";
    private string takeDamageAniParm = "TakeDamage";
    private string healingAniParm = "Healing";
    private string castingAniParm = "Casting";
    private string deathAniParm = "Death";

    // �������
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
        // ������ײ��༭��
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
        // ���Һ��Ϻ����ײ�˺�
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
            // ��Ծ��������
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // ֹͣ������
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

    // ������ʱ ʱ�����
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
    /// ����
    /// </summary>
    /// <returns></returns>
    private void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > castBtnUpTime && Mana > 0 && Health < maxHealth && !pState.jumping && !pState.dashing)
        {
            Debug.Log($"castTimer {castOrHealTimer}");

            pState.healing = true;
            anim.SetBool(healingAniParm, true);

            // �ظ�����ֵ
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                healTimer = 0;
                Health++;
            }

            // ���ķ���ֵ
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
    /// �ͷŷ���
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
        // ���������ڼ� �����ٶ�ת��Ϊ�����ٶ�
        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    private IEnumerator CastCoroutine()
    {
        anim.SetBool(castingAniParm, true);
        // �ڶ�����n�봦�ͷ�
        yield return new WaitForSeconds(0.15f);

        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            GameObject _fireball = Instantiate(sideSpellFireball, sideAttackTransform.position, Quaternion.identity);
            // ��ת����
            _fireball.transform.eulerAngles = pState.lookingRight ? Vector3.zero : new Vector2(_fireball.transform.eulerAngles.x, 180);
            // ���Ϻ�����
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
        //���ķ���
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

    // ����
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
    /// �Ƿ��ڵ���
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