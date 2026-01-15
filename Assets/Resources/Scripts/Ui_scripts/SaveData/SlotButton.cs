using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class SlotButton : MonoBehaviour
{
    public int slotIndex;
    public Text infoText;
    public Button button;

    public string slotLabelKey = "Slot Number";   // Table Reference = "UI"
    public string emptyLabelKey = "Empty slot";

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        var locale = LocalizationSettings.SelectedLocale;
        var table = LocalizationSettings.StringDatabase.GetTable("Btn_Language");
        bool hasSave = SaveManager.HasSave(slotIndex);

        //if (hasSave)
        //{
        //    SaveData data = SaveManager.Load(slotIndex);
        //    infoText.text = $"슬롯 {slotIndex + 1}\n{data.saveTime}";
        //}
        //else
        //{
        //    infoText.text = $"슬롯 {slotIndex + 1}\n비어 있음";
        //}
        var slotEntry = table.GetEntry(slotLabelKey);
        string slotText = slotEntry?.GetLocalizedString(new object[] { slotIndex + 1 }) ?? $"슬롯 {slotIndex + 1}";

        // 비어 있음 텍스트 가져오기
        var emptyEntry = table.GetEntry(emptyLabelKey);
        string emptyText = emptyEntry?.GetLocalizedString() ?? "비어 있음";

        if (hasSave)
        {
            SaveData data = SaveManager.Load(slotIndex);
            infoText.text = $"{slotText}\n{data.saveTime}";
        }
        else
        {
            infoText.text = $"{slotText}\n{emptyText}";
        }

        // 이어하기일 때만 저장 없는 슬롯 비활성
        if (SlotSelectContext.Instance.mode == SlotSelectMode.Continue)
            button.interactable = hasSave;
        else
            button.interactable = true;
    }

    public void OnClick()
    {
        SaveContext.Instance.currentSlot = slotIndex;

        // 새 게임이면 기존 세이브 삭제
        if (SlotSelectContext.Instance.mode == SlotSelectMode.NewGame)
        {
            if (SaveManager.HasSave(slotIndex))
                SaveManager.Delete(slotIndex);
        }

        // 씬 이동은 Scenemgr가 전담
        Scenemgr.Instance.ChangeScene(eSCENE.MainMenu);
    }
}
