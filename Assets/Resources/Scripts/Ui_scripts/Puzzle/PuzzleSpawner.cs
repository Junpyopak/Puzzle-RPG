using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpawner : MonoBehaviour
{
    [Header("패널")]
    public Transform parentPanel;

    [Header("스폰할 프리펩들")]
    public List<GameObject> prefabs;

    public int width = 6;
    public int height = 5;
    public Vector2 cellSize = new Vector2(60, 55);
    public Vector2 spacing = new Vector2(3, 5);

    [Header("몇 개 스폰할지")]
    public int spawnCount = 30;

    void Start()
    {
        SpawnBlocks();
    }

    void SpawnBlocks()
    {


        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogError("프리팹 리스트 비어있음");
            return;
        }

        RectTransform panel = parentPanel.GetComponent<RectTransform>();
        if (panel == null)
        {
            Debug.LogError("parentPanel 은 RectTransform 이어야 함 (UI Panel)");
            return;
        }

        float panelWidth = panel.rect.width;
        float panelHeight = panel.rect.height;

        float cellW = panelWidth / width;
        float cellH = panelHeight / height;

        for (int i = 0; i < width * height; i++)
        {
            // 랜덤으로 선택
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject obj = Instantiate(prefab, parentPanel);

            RectTransform rt = obj.GetComponent<RectTransform>();

            int x = i % width;
            int y = i / width;

            // 셀 중앙 좌표
            float posX = (-panelWidth / 2f) + (cellW * x) + (cellW / 2f);
            float posY = (panelHeight / 2f) - (cellH * y) - (cellH / 2f);

            rt.anchoredPosition = new Vector2(posX, posY);

            // spacing 적용
            float spacingX = spacing.x;
            float spacingY = spacing.y;

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellW - spacingX);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellH - spacingY);
        }
    }
}







