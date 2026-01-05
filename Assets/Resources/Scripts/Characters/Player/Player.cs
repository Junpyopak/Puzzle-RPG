using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    public Camera cam;
    public float padding = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
       
    }
    void LateUpdate()
    {
        //플레이어가 화면 밖으로 못나가도록
        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);

        Rect vr = cam.rect;

        //왼쪽·아래 경계 (Viewport 기준)
        Vector3 min = cam.ViewportToWorldPoint(
            new Vector3(vr.xMin, vr.yMin, z)
        );
        //오른쪽·위 경계 (전체 시야 기준)
        Vector3 max = cam.ViewportToWorldPoint(
            new Vector3(1f, vr.yMax, z)
        );

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, min.x + padding, max.x - padding);
        pos.y = Mathf.Clamp(pos.y, min.y + padding, max.y - padding);
        pos.z = transform.position.z;

        transform.position = pos;
    }
    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            anim.SetTrigger("Attack");
        }
    }
}
