using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHeal : MonoBehaviour
{
    ItemID itemID;
    [Header("È¹µæ Èú·®")]
    public int HealAmount = 20;
    public float pullSpeed = 3;
    public float pickupDistance = 3f;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        itemID = GetComponent<ItemID>();
        FieldItemManager.Instance.Register(itemID);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= pickupDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Ã¼·Â È¸º¹");
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.HealHp(HealAmount);
            Debug.Log($"Ã¼·Â È¸º¹ : {HealAmount}");
        }
        FieldItemManager.Instance.Unregister(itemID);
        Destroy(gameObject);
    }
}
