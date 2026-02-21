using TMPro;
using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 0.7f;
    public float lifetime = 0.3f;
    public TextMeshPro text;
    float timer;
    public void Setup(int damage)
    {
        text.text = damage.ToString();
    }

    void Update()
    {
        // 위로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 카메라 바라보기
        transform.forward = Camera.main.transform.forward;

        timer += Time.deltaTime;
        if (timer > lifetime)
            Destroy(gameObject);
    }
}