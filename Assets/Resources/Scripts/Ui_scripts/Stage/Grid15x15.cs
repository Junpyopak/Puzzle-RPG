using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class Grid15x15 : MonoBehaviour
{
    public int gridCount = 15;
    public GameObject cellPrefab;

    bool spawned = false;

    void Start()
    {
        SpawnCells();
    }

    void LateUpdate()
    {
        RectTransform rt = GetComponent<RectTransform>();
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();

        //float cellSize = Mathf.Min(rt.rect.width, rt.rect.height) / gridCount;
        //grid.cellSize = new Vector2(cellSize, cellSize);
        float cellWidth = rt.rect.width / gridCount;
        float cellHeight = rt.rect.height / gridCount;
        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }

    void SpawnCells()
    {
        if (spawned) return;
        spawned = true;

        // 기존 자식 제거 (중복 방지)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // 15 x 15 = 225 생성
        for (int i = 0; i < gridCount * gridCount; i++)
        {
            Instantiate(cellPrefab, transform);
        }
    }
}
