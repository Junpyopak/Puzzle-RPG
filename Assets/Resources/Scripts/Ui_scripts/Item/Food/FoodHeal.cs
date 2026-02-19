using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHeal : MonoBehaviour
{
    ItemID itemID;
    [Header("획득 힐량")]
    public int HealAmount = 20;
    public float pullSpeed = 3;
    public float pickupDistance = 3f;
    Transform player;
    Player playerScript;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerScript = FindObjectOfType<Player>();
        itemID = GetComponent<ItemID>();
        FieldItemManager.Instance.Register(itemID);
    }

    // Update is called once per frame
    void Update()
    {
        //if (player != null)
        //{
        //    float dist = Vector2.Distance(transform.position, player.position);
        //    if (dist <= pickupDistance)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);
        //    }

        //}
        if (player != null)
        {
            float effectiveDistance = pickupDistance + playerScript.pickupDistanceBonus;//아이템의 기본 끌어오기 거리 + 카드나 버프 등으로 플레이어에게 추가된 끌어오기 거리
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= effectiveDistance)//플레이어와 아이템 사이 거리가 effectiveDistance 이하이면 당겨오기 
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("체력 회복");
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.HealHp(HealAmount);
            Debug.Log($"체력 회복 : {HealAmount}");
        }
        FieldItemManager.Instance.Unregister(itemID);
        Destroy(gameObject);
    }
}
