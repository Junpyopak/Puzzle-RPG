using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    public Transform pullTarget;
    //나중에 플레이어 능력에 의해 끌어당기는 걸이가 바뀌는것을 통제하기 위함
    public float Range = 3;
    public float pullSpeed = 5;
    // Start is called before the first frame update
    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Range);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("ExpJem")) continue;

            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb == null) continue;

            Vector2 dir = ((Vector2)transform.position - rb.position).normalized;
            rb.velocity = dir * pullSpeed;
        }
    }

    // 범위 보이게
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
