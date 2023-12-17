using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField]
    private float chargeSpeedMultiplier;
    [SerializeField]
    private float chargeDuration;
    private float chareTimer;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float ledgeCheckX; // ±ßÔµ»òÇ½±Ú¼ì²â
    [SerializeField]
    private float ledgeCheckY;
    [SerializeField]
    private LayerMask whatIsGround;

    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Charger_Idle);
        rb.gravityScale = gravity;
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        switch (CurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:

                // Î´¼ì²âµ½µØÃæ»ò¼ì²âµ½Ç½±ÚÔò·­×ªÒÆ¶¯
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Charger_Suprised);
                }

                rb.velocity = transform.localScale.x > 0 ? new Vector2(speed, rb.velocity.y) : new Vector2(-speed, rb.velocity.y);
                break;

            case EnemyStates.Charger_Suprised:
                rb.velocity = new Vector2(0, jumpForce);
                ChangeState(EnemyStates.Charger_Charge);
                break;

            case EnemyStates.Charger_Charge:
                chareTimer += Time.deltaTime;
                if (chareTimer < chargeDuration)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        rb.velocity = transform.localScale.x > 0 ? new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y) : new Vector2(-speed * chargeSpeedMultiplier, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    chareTimer = 0;
                    ChangeState(EnemyStates.Charger_Idle);
                }
                break;
        }
    }

    protected override void ChangeEnemyAnimation()
    {
        if (CurrentEnemyState == EnemyStates.Charger_Idle)
        {
            anim.speed = 1;
        }
        if (CurrentEnemyState == EnemyStates.Charger_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }
}