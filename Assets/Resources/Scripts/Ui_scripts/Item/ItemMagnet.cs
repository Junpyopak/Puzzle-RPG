using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    public float pullSpeed = 3;
    public float pickupDistance= 3f;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= pickupDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);
            }
            
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        //float Dis = Vector2.Distance( transform.position, other.transform.position );
        //if(Dis<=Range)
        //{
        //    other.transform.position = Vector3.MoveTowards(other.transform.position, transform.position, pullSpeed * Time.deltaTime);
        //}
        Debug.Log("¾ÆÀÌÅÛ ÀÚµ¿ È¹µæ");
        Destroy(gameObject);
    }
}
