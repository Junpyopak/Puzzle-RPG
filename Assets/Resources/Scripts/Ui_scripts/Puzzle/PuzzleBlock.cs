using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleBlock : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PuzzleBoard board;
    public int x;
    public int y;

    private Vector3 startPos;
    private CanvasGroup canvasGroup;


    [Header("Puzzle Info")]
    public int puzzleId;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = eventData.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        PuzzleBlock target = null;

        foreach (var r in results)
        {
            target = r.gameObject.GetComponent<PuzzleBlock>();
            if (target != null && target != this)
                break;
        }

        if (target != null)
        {
            board.TrySwap(this, target);
        }
        else
        {
            transform.position = startPos;
        }
    }
}
