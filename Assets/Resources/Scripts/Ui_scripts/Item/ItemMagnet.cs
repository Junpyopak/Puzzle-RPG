using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    ItemID itemID;
    public float pullSpeed = 3;
    public float pickupDistance= 3f;
    Transform player;
    public GameObject OpenItem;
    // Start is called before the first frame update

    void Awake()
    {
        OpenItem = GameObject.Find("OpenItem");
    }

    void Start()
    {
        itemID = GetComponent<ItemID>();
        FieldItemManager.Instance.Register(itemID);

        //player = GameObject.FindWithTag("Player").transform;
        //if (OpenItem == null)
        //{
        //    OpenItem = GameObject.Find("OpenItem");
        //    if (OpenItem == null)
        //        Debug.LogWarning("Rollet UI를 찾을 수 없습니다!");
        //}

        //OpenItem.SetActive(false);
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾지 못했습니다!");
        }

        if (OpenItem == null)
        {
            OpenItem = GameObject.Find("OpenItem");

            if (OpenItem == null)
                Debug.LogWarning("OpenItem UI를 찾을 수 없습니다!");
            else
                OpenItem.SetActive(false);
        }
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
        Debug.Log("아이템 자동 획득");
        FieldItemManager.Instance.Unregister(itemID);
        Destroy(gameObject);
        //OpenItem.SetActive(true);
        FindObjectOfType<PanelClose>().OpenItem();
        Turn_Timer.Instance.isPaused = true;
        UI_GameTimer.Instance.isPaused = true;
    }
}
