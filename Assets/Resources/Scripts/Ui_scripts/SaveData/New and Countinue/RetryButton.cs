using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickRetry()
    {


        // 저장 데이터가 있든 없든 전투 씬으로 이동
        Scenemgr.Instance.ChangeScene(eSCENE.GameScene);

    }
    public void Gotittle()
    {
        int slot = SaveContext.Instance.currentSlot;

        if (slot >= 0)
            SaveManager.Delete(slot);

        SaveContext.Instance.currentSlot = -1;
        SaveContext.Instance.isLoading = false;
        Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
    }
}
