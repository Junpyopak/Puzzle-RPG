using TMPro;
using UnityEngine;

public class ClickableCard : MonoBehaviour
{
    void Update()
    {
        CardClickable();
    }

    //private void CardClickable()
    //{
    //    // 마우스 왼쪽 버튼이 눌렸는지 확인
    //    // 일시정지 메뉴가 열려있지 않은지 확인 (GameManager를 통한 체크)
    //    if (Input.GetMouseButtonDown(0) )//&& !GameManager.Instance.UIMgr.isOpenPauseMenu)
    //    {
    //        // 마우스 위치에서 화면을 통과하는 Ray 생성
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        // Ray가 3D 오브젝트(Collider)에 충돌했는지 확인
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            // 충돌한 오브젝트가 "Card" 태그를 가졌고, 현재 카드 선택 상태(isOpen)인지 확인
    //            if (hit.collider.CompareTag("Card") && GameManager.Instance.CardMgr.isOpen)
    //            {
    //                GameObject clickedObject = hit.collider.gameObject;
    //                Debug.Log($"현재 클릭한 카드 : {clickedObject.name}");

    //                // CardManager 클릭된 카드를 제외한 나머지 카드 애니메이션 처리 및 리셋 예약
    //                GameManager.Instance.CardMgr.CloseOtherCards(clickedObject);

    //                // CardManager 클릭된 카드의 이름을 플레이어 리스트에 저장
    //                GameManager.Instance.CardMgr.AddSelectCard(clickedObject);
    //            }
    //            else if (hit.collider.CompareTag("Card"))
    //            {
    //                Debug.Log("아직 카드가 오픈되지 않았습니다");
    //            }
    //            else
    //            {
    //                Debug.Log("뭘 누르는 거임?");
    //            }
    //        }
    //    }
    //}
    private void CardClickable()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Card") && GameManager.Instance.CardMgr.isOpen)
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    var cardMgr = GameManager.Instance.CardMgr;

                    Debug.Log($"현재 클릭한 카드 : {clickedObject.name}");

                    // TwoPick 카드일 경우 CSV value 기반 확률 체크
                    if (cardMgr.twoPickValue > 0f)
                    {
                        float roll = Random.Range(0f, 100f);
                        cardMgr.twoPick = roll < cardMgr.twoPickValue;
                        Debug.Log($"TwoPick 체크: 확률={cardMgr.twoPickValue}%, 결과={cardMgr.twoPick}");
                    }

                    // 클릭된 카드 이름 그대로 플레이어 리스트에 저장
                    GameManager.Instance.CardMgr.AddSelectCard(clickedObject);

                    // TwoPick이면 한 번 더 클릭 가능, 아닐 경우 즉시 초기화
                    if (!cardMgr.twoPick)
                    {
                        // 한 장 선택 완료 → TwoPick 초기화
                        cardMgr.twoPickValue = 0f;
                        cardMgr.twoPick = false;
                    }

                    //마지막에 클릭된 카드 제외하고 나머지 카드 닫기
                    cardMgr.CloseOtherCards(clickedObject);
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Debug.Log("아직 카드가 오픈되지 않았습니다");
                }
                else
                {
                    Debug.Log("뭘 누르는 거임?");
                }
            }
        }
    }
}
