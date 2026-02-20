using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float speed = 10f;
    private Transform player; // public에서 private으로 변경 가능
    private Rigidbody2D rb;
    private bool isReturning = false;

    // Shot 호출 시 플레이어 정보를 직접 넘겨받음
    public void Shot(Vector2 direction, Transform playerTransform)
    {
        isReturning = false;
        player = playerTransform; // 플레이어 정보 즉시 저장

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        //if (rb != null) rb.velocity = direction.normalized * speed;
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
            Debug.Log("Boomerang Shot! Velocity: " + rb.velocity + ", Direction: " + direction);
        }
        else
        {
            Debug.LogError("Rigidbody2D is missing!");
        }
    }

    private void Update()
    {
        // player가 할당되어 있어야만 로직 실행
        if (player == null) return;

        if (!isReturning) CheckOutOfBounds();
        else ReturnToPlayer();
    }

    private void CheckOutOfBounds()
    {
        if (Camera.main == null) return;
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPos.x < -0.05f || viewPos.x > 1.05f || viewPos.y < -0.05f || viewPos.y > 1.05f || viewPos.z < 0)
            isReturning = true;
    }

    private void ReturnToPlayer()
    {
        // 이제 player가 절대 null일 리 없음 (Shot에서 받았으므로)
        Vector2 direction = (Vector2)player.position - (Vector2)transform.position;
        rb.velocity = direction.normalized * speed;

        if (Vector2.Distance(transform.position, player.position) < 0.1f) //삭제 범위
            Destroy(gameObject);
    }
}