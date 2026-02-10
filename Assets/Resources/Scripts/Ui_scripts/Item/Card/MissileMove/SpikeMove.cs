using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMove : MonoBehaviour
{
    public Camera cam;
    public float padding = 0.5f;
    public Vector3 velocity = new Vector3(1f, 1f, 0f); // ¿Ãµø º”µµ
    public int BoundCount = 1;

    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Rect vr = cam.rect;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(vr.xMin, vr.yMin, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1f, vr.yMax, z));

        Vector3 pos = transform.position;

        // «◊ªÛ ¿Ãµø
        pos += velocity * Time.deltaTime;

        // x√‡ √Êµπ
        if (pos.x < min.x + padding)
        {
            pos.x = min.x + padding;
            if (BoundCount > 0)
            {
                velocity.x *= -1;
                BoundCount--;
            }
            else
            {
                velocity = Vector3.zero; // ∆®±Ë »Ωºˆ ¥Ÿ µ«∏È ∏ÿ√„
            }
        }
        else if (pos.x > max.x - padding)
        {
            pos.x = max.x - padding;
            if (BoundCount > 0)
            {
                velocity.x *= -1;
                BoundCount--;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }

        // y√‡ √Êµπ
        if (pos.y < min.y + padding)
        {
            pos.y = min.y + padding;
            if (BoundCount > 0)
            {
                velocity.y *= -1;
                BoundCount--;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }
        else if (pos.y > max.y - padding)
        {
            pos.y = max.y - padding;
            if (BoundCount > 0)
            {
                velocity.y *= -1;
                BoundCount--;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }

        transform.position = pos;
    }
}
