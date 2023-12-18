using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShade : Enemy
{
    [SerializeField]
    private float deathAfterGravity = 12;
    [SerializeField]
    private float chaseDistance;
    [SerializeField]
    private float stunDuration;
    private float stunTimer;

    public static PlayerShade Instance;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.PlayerShade_Idle);
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.PlayerShade_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (CurrentEnemyState)
        {
            case EnemyStates.PlayerShade_Idle:
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.PlayerShade_Chase);
                }
                break;

            case EnemyStates.PlayerShade_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, speed * Time.deltaTime));
                FlipPlayerShade();
                break;

            case EnemyStates.PlayerShade_Stunned:
                stunTimer += Time.deltaTime;
                if (stunTimer > stunDuration)
                {
                    ChangeState(EnemyStates.PlayerShade_Idle);
                    stunTimer = 0;
                }
                break;

            case EnemyStates.PlayerShade_Death:
                Death(Random.Range(5, 10));
                break;
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = deathAfterGravity;
        base.Death(_destroyTime);
    }

    protected override void ChangeEnemyAnimation()
    {
        if (CurrentEnemyState == EnemyStates.PlayerShade_Idle)
        {
            anim.Play("Player_Idle");
        }
        anim.SetBool("Walking", CurrentEnemyState == EnemyStates.PlayerShade_Chase);
        if (CurrentEnemyState == EnemyStates.PlayerShade_Death)
        {
            //anim.SetTrigger("Death");
            PlayerController.Instance.RestoreMana();
            anim.SetTrigger("Death");
            Destroy(gameObject, 0.5f);
        }
    }

    protected override void Attack()
    {
        anim.SetTrigger("Attacking");
        PlayerController.Instance.TakeDamage(damage);
    }

    private void FlipPlayerShade()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.PlayerShade_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.PlayerShade_Death);
        }
    }
}