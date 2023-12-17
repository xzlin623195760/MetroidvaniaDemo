using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float gravity = 12;
    [SerializeField]
    protected float health;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float damage;

    [SerializeField]
    protected GameObject orangeBlood;
    [SerializeField]
    protected float orangeBloodLifeTime = 5.5f;

    [Header("Recoil Settings")]
    [SerializeField]
    protected float recoilLength;

    [SerializeField]
    protected float recoilFactor;

    [SerializeField]
    protected bool isRecoiling = false;

    protected float recoilTimer;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;

    protected enum EnemyStates
    {
        // Crawler
        Crawler_Idle,
        Crawler_Flip,

        // Bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

        // Charger
        Charger_Idle,
        Charger_Suprised,
        Charger_Charge
    }

    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates CurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeEnemyAnimation();
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void UpdateEnemyStates()
    {
    }

    protected virtual void ChangeState(EnemyStates _newState)
    {
        CurrentEnemyState = _newState;
    }

    protected virtual void ChangeEnemyAnimation()
    {
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (health > 0)
        {
            GameObject _orangeBlood = Instantiate(orangeBlood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, orangeBloodLifeTime);
        }
        health -= _damageDone;
        Debug.Log($"{transform.gameObject.name} Hp: {health}");
        if (!isRecoiling || (isRecoiling && recoilTimer < recoilLength))
        {
            if (isRecoiling)
            {
                recoilTimer = 0;
            }
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }
}