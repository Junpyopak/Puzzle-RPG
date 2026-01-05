using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    //나중에 플레이어 능력에 의해 끌어당기는 걸이가 바뀌는것을 통제하기 위함
    public float Range = 3;
    public float pullSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("ItemBox")) return;
        //float Dis = Vector2.Distance( transform.position, other.transform.position );
        //if(Dis<=Range)
        //{
        //    other.transform.position = Vector3.MoveTowards(other.transform.position, transform.position, pullSpeed * Time.deltaTime);
        //}
        Debug.Log("아이템 자동 획득");
        Destroy(other.gameObject);
    }
}
