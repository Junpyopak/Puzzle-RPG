using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID;
    private SpriteRenderer sr;
    public float flashAlpha = 0.7f;      // 깜빡일 때 알파
    public float fadeSpeed = 5f;
    public float flashDuration = 0.1f;   // 반짝임 유지 시간
    public Color flashColor = Color.red; // 깜빡일 색
    private float originalAlpha;
    private Color originalColor;
    void Start()
    {
        MonsterData data =
            MonsterDataTable.Instance.monsterDic[monsterID];

        Debug.Log($"{data.Name} 생성 / 타입 : {data.MonsterType}");
        sr = GetComponent<SpriteRenderer>();
        originalAlpha = sr.color.a;
        originalColor = sr.color;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("PlayerAttack")) return;
        Damage();
        Debug.Log("데미지 받음.");
    }
    void Damage()
    {
        StartCoroutine(FlashCoroutine());
    }
    //private IEnumerator FlashCoroutine()
    //{
    //    // 1. 알파 감소
    //    Color c = sr.color;
    //    c.a = flashAlpha;
    //    sr.color = c;

    //    // 2. 잠시 대기
    //    yield return new WaitForSeconds(flashDuration);

    //    // 3. 알파 원래대로 복원
    //    c = sr.color;
    //    c.a = originalAlpha;
    //    sr.color = c;
    //}
    private IEnumerator FlashCoroutine()//데미지 입었을때 빨간색 버전 
    {
        // 1. 빨강으로 변경
        sr.color = flashColor;

        // 2. 잠시 대기
        yield return new WaitForSeconds(flashDuration);

        // 3. 원래 색으로 복원
        sr.color = originalColor;
    }
}

