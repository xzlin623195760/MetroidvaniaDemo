using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Enemy
{
    [SerializeField]
    private float flipWaitTime;

    private float flipTimer;

    [SerializeField]
    private float ledgeCheckX; // ±ßÔµ»òÇ½±Ú¼ì²â
    [SerializeField]
    private float ledgeCheckY;
    [SerializeField]
    private LayerMask whatIsGround;

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = gravity;
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (CurrentEnemyState)
        {
            case EnemyStates.Crawler_Idle:
                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
                // Î´¼ì²âµ½µØÃæ»ò¼ì²âµ½Ç½±ÚÔò·­×ªÒÆ¶¯
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Crawler_Flip);
                }
                rb.velocity = transform.localScale.x > 0 ? new Vector2(speed, rb.velocity.y) : new Vector2(-speed, rb.velocity.y);
                break;

            case EnemyStates.Crawler_Flip:
                flipTimer += Time.deltaTime;
                if (flipTimer > flipWaitTime)
                {
                    flipTimer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Crawler_Idle);
                }
                break;
        }
    }
}