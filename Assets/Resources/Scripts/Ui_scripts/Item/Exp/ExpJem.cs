using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpJem : MonoBehaviour
{

    [Header("È¹µæ °æÇèÄ¡")]
    public int expAmount = 1;

    public float pullSpeed = 3;
    public float pickupDistance = 3f;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
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
        Debug.Log("°æÇèÄ¡ È¹µæ");
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.AddExp(expAmount);
            Debug.Log($"°æÇèÄ¡ {expAmount} È¹µæ");
        }
        Destroy(gameObject);

    }
}
