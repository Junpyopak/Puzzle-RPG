using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 5;

    public Vector2 cellSize = new Vector2(60, 55);
    public Vector2 spacing = new Vector2(3, 5);

    public PuzzleBlock[,] blocks;

    private void Awake()
    {
        blocks = new PuzzleBlock[width, height];
        InitBoard();
    }

    void InitBoard()
    {
        int i = 0;

        foreach (Transform child in transform)
        {
            PuzzleBlock block = child.GetComponent<PuzzleBlock>();
            if (block == null)
                continue;

            int x = i % width;
            int y = i / width;

            blocks[x, y] = block;

            block.board = this;
            block.x = x;
            block.y = y;

            block.transform.localPosition = GetPosition(x, y);

            i++;
        }
    }

    Vector3 GetPosition(int x, int y)
    {
        float startX = -(width - 1) * (cellSize.x + spacing.x) / 2f;
        float startY = -(height - 1) * (cellSize.y + spacing.y) / 2f;

        return new Vector3(
            startX + x * (cellSize.x + spacing.x),
            startY + y * (cellSize.y + spacing.y),
            0f
        );
    }

    public void TrySwap(PuzzleBlock a, PuzzleBlock b)
    {
        blocks[a.x, a.y] = b;
        blocks[b.x, b.y] = a;

        int tx = a.x;
        int ty = a.y;

        a.x = b.x;
        a.y = b.y;

        b.x = tx;
        b.y = ty;

        a.transform.localPosition = GetPosition(a.x, a.y);
        b.transform.localPosition = GetPosition(b.x, b.y);
    }
}
