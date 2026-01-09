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

    public bool isDisabled = false;

    [Header("Puzzle Info")]
    public int puzzleId;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        startPos = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        canvasGroup.blocksRaycasts = true;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = eventData.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        PuzzleBlock target = null;

        foreach (var r in results)
        {
            target = r.gameObject.GetComponent<PuzzleBlock>();
            if (target != null && target != this && !target.isDisabled) //투명화된 블록은 다른블록을 집어서 스왑 하려해도 스왑이 안되도록 
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
