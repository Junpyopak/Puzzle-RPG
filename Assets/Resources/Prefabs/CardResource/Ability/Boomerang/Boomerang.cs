using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float speed = 10f;
    private Transform player; // public에서 private으로 변경 가능
    private Rigidbody2D rb;
    private bool isReturning = false;
    private Player playerSc;
    private int damage;
    // Shot 호출 시 플레이어 정보를 직접 넘겨받음
    public void Shot(Vector2 direction, Transform playerTransform)
    {
        isReturning = false;
        player = playerTransform; // 플레이어 정보 즉시 저장
        playerSc = playerTransform.GetComponent<Player>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direction.normalized * speed;
    }
    private void Update()
    {
        // player가 할당되어 있어야만 로직 실행
        if (player == null) return;

        if (!isReturning) CheckOutOfBounds();
        else ReturnToPlayer();
    }
    public void SetDamage(int dmg)
    {
        damage = dmg;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Monster monster = collision.GetComponent<Monster>();
        if (monster != null && playerSc != null)
        {
            // PlayerATK 실시간 참조
            //int damage = Mathf.Max(1, Mathf.RoundToInt(playerSc.PlayerATK));
            //monster.TakeDamageFromPlayer(damage);
            int finalDamage = Mathf.Max(1, damage);
            monster.TakeDamageFromPlayer(finalDamage);
            // 2. 날카로운 검 출혈 적용
            if (playerSc.hasBloodDamage)
            {
                if (Random.value <= playerSc.bloodDamageChance / 100f) // 확률 체크
                {
                    // 몬스터에 Bleed 상태가 없으면 새로 생성
                    if (monster.bleed == null) monster.bleed = new Monster.BleedStatus();

                    monster.bleed.damagePerTurn = playerSc.bloodDamagePerTick;
                    monster.bleed.remainingTurns = playerSc.bloodDamageTurns;
                    monster.bleed.chance = playerSc.bloodDamageChance / 100f;

                    Debug.Log($"{monster.name}에게 출혈 적용! {playerSc.bloodDamagePerTick} 데미지, {playerSc.bloodDamageTurns}턴");
                }
            }
        }
    }
}