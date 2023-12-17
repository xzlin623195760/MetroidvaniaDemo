using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [SerializeField]
    private float deathAfterGravity = 12;
    [SerializeField]
    private float chaseDistance;
    [SerializeField]
    private float stunDuration;
    private float stunTimer;

    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (CurrentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Bat_Chase);
                }
                break;

            case EnemyStates.Bat_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, speed * Time.deltaTime));
                FlipBat();
                break;

            case EnemyStates.Bat_Stunned:
                stunTimer += Time.deltaTime;
                if (stunTimer > stunDuration)
                {
                    ChangeState(EnemyStates.Bat_Idle);
                    stunTimer = 0;
                }
                break;

            case EnemyStates.Bat_Death:
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
        anim.SetBool("Idle", CurrentEnemyState == EnemyStates.Bat_Idle);
        anim.SetBool("Chase", CurrentEnemyState == EnemyStates.Bat_Chase);
        anim.SetBool("Stunned", CurrentEnemyState == EnemyStates.Bat_Stunned);
        if (CurrentEnemyState == EnemyStates.Bat_Death)
        {
            anim.SetTrigger("Death");
        }
    }

    private void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.Bat_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Bat_Death);
        }
    }
}