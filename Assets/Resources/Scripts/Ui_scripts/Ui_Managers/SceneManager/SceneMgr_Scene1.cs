using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemgr : MonoBehaviour


{
    public static Scenemgr Instance { get; private set; }

    public eSCENE Scene { get; private set; } // 현재 씬

    private void Awake()
    {
        // 싱글톤 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬 전환 메서드
    public void ChangeScene(eSCENE _e, bool _loading = false)
    {

        Scene = _e;

        if (_loading)
        {
            // 로딩씬 처리하고 싶으면 여기서 추가
            SceneManager.LoadScene("LOADING");
            // 로딩 후 실제 씬 전환 코드는 코루틴으로 처리 가능
            // StartCoroutine(LoadSceneAsync(_e));
            return;
        }

        switch (_e)
        {
            case eSCENE.TITLE:
                SceneManager.LoadScene("TITLE");
                break;

            case eSCENE.LOBBY:
                SceneManager.LoadScene("LOBBY");
                break;

            case eSCENE.BATTLE:
                SceneManager.LoadScene("BATTLE");
                break;

            default:
                Debug.LogWarning("정의되지 않은 씬: " + _e);
                break;
        }
    }
}
