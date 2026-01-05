using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    public float Range = 5;
    public float pullSpeed = 7;
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

        Debug.Log("æ∆¿Ã≈€ ¿⁄µø »πµÊ");
        Destroy(other.gameObject);
    }
}
