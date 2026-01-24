using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class Grid15x15 : MonoBehaviour
{
    public int gridCount = 25;
    public GameObject cellPrefab;
    public static Grid15x15 Instance;

    public Vector2 cellWorldSize;
    bool spawned = false;
    void Awake()
    {
        Instance = this;
    }

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

        cellWorldSize = new Vector2(cellWidth / 100f,  cellHeight / 100f);
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
