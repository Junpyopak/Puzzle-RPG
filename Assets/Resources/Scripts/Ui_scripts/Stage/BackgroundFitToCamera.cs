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

        // Y 축 이동 금지 (잘림 원인)
        rt.anchoredPosition = new Vector2(0f,0f);
        // Size만 카메라 크기에 맞춤
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, camRect.width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, camRect.height);
    }
}

