using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpJem : MonoBehaviour
{
    ItemID itemID;
    [Header("È¹µæ °æÇèÄ¡")]
    public int expAmount = 1;

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
            float effectiveDistance = pickupDistance + playerScript.pickupDistanceBonus;
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= effectiveDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("°æÇèÄ¡ È¹µæ");
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            int totalExp = expAmount + player.bonusExpIncrease;
            player.AddExp(totalExp);
            Debug.Log($"°æÇèÄ¡ {totalExp} È¹µæ (±âº» {expAmount} + º¸³Ê½º {player.bonusExpIncrease})");
        }
        FieldItemManager.Instance.Unregister(itemID);
        Destroy(gameObject);
    }
}
