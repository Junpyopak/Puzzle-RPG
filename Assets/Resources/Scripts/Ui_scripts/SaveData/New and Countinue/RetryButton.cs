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
        int slot = SaveContext.Instance.currentSlot;

        if (SaveManager.HasSave(slot))
        {
            Scenemgr.Instance.ChangeScene(eSCENE.MainMenu);
        }
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
