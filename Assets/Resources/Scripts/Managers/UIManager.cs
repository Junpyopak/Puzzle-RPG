using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //public static UIManager Instance;
    public TMP_InputField levelInput;

    [Header("일시정지 관련")]
    public bool isOpenPauseMenu = false; //일시정지 메뉴를 열었는가; (기본값 : false)
    public GameObject pauseMenu;

    [Header("사운드 관련")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    //#region 싱글톤
    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    //#endregion

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()//일시정지 메뉴
    {
        if (Input.GetKeyDown(KeyCode.V)) //esc 키를 눌렀고 일시정지중이 아니였다면.
        {
            if (isOpenPauseMenu == false)
            {
                isOpenPauseMenu = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                isOpenPauseMenu = false;
                pauseMenu.SetActive(false);
            }
        }
    }

    //public void NextTurnBtnDown() // 다음 턴으로
    //{
    //    GameManager.Instance.TurnMgr.NextTurn(); // 턴 매니저 인스턴스에서 받아오기
    //}

    //새롭게 만든 카드 오픈 버튼
    //public void CardRarityOpenBtn()
    //{
    //    if (GameManager.Instance.CardMgr.isOpen == false)
    //    {
    //        GameManager.Instance.CardMgr.CardRarityOpen();
    //    }
    //    else
    //    {
    //        Debug.LogError("이미 카드가 오픈된 상태기에 동작할 수 없습니다!");
    //    }
    //}

    //public void CardRarityCloseBtn()
    //{
    //    if (GameManager.Instance.CardMgr.isOpen == true)
    //    {
    //        GameManager.Instance.CardMgr.ResetCardList();
    //    }
    //    else
    //    {
    //        Debug.LogError("이미 카드가 클로즈된 상태기에 동작할 수 없습니다!");
    //    }
    //}

    //----------------------------------

    /// <summary>
    /// 카드 오픈 버튼
    /// </summary>
    //public void CardOpenBtn()
    //{
    //    if (GameManager.Instance.CardMgr.isOpen == false)
    //    {
    //        GameManager.Instance.CardMgr.CardOpen();
    //    }
    //    else
    //    {
    //        Debug.Log("이미 오픈 상태임");
    //    }
    //}


    /// <summary>
    /// 카드 클로즈 버튼
    /// </summary>
    //public void CardCloseBtn()
    //{
    //    if (GameManager.Instance.CardMgr.isOpen == true)
    //    {
    //        GameManager.Instance.CardMgr.CardClose();
    //    }
    //    else
    //    {
    //        Debug.Log("이미 클로즈 상태임");
    //    }
    //}

    /// <summary>
    /// 인풋필드에 있는 레벨을 감지하고 레벨을 올려주는 버튼
    /// </summary>
    //public void LevelUpBtn() //인풋필드에 있는 int값을 감지
    //{
    //    if (levelInput.text == null) //인풋 필드가 비어있으면
    //    {
    //        Debug.Log("대머리"); //비어있음을 뜻함
    //    }
    //    else
    //    {
    //        int level = int.Parse(levelInput.text); // 인풋필드 형 변환
    //        GameManager.Instance.LevelMgr.LevelUP(level); // 레벨 매니저의 인스턴스 가져오기
    //        //GameManager.Instance.CardMgr.CardTargetLevel(); //카드 매니저의 인스턴스를 가져와 카드를 오픈할 레벨이 되는지 확인 <- 현재는 TargetLevel의 부재로 필요 없는 함수
    //    }
    //}
}   
