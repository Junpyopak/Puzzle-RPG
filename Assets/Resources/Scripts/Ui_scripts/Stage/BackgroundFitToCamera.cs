using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BackgroundFitToCamera : MonoBehaviour
{
    public Camera targetCamera;
    RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        Rect camRect = targetCamera.pixelRect;

        rt.anchoredPosition = new Vector2(camRect.x, camRect.y);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, camRect.width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, camRect.height);
    }
}

