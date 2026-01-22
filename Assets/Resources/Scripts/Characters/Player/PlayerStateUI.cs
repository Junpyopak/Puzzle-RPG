using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    [Header("플레이어 참조")]
    public Player player;

    [Header("HP UI")]
    public Slider hpSlider;
    public Text hpText;

    [Header("EXP UI")]
    public Slider expSlider;

    [Header("기본 스탯 UI")]
    public Text atkText;
    public Text defText;
    public Text levelText;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        UpdateUI(); // 스탯 자주 바뀌면 그냥 Update에서
    }

    public void UpdateUI()
    {
        // HP
        hpSlider.maxValue = player.Hp;
        hpSlider.value = player.Hp;
        hpText.text = $"{player.Hp}";

        // EXP
        expSlider.maxValue = player.NeedExp;
        expSlider.value = player.Exp;

        // 스탯
        atkText.text = $"{player.PlayerATK}";
        defText.text = $"{player.Defence}";
        levelText.text = $"Lv.{player.PlayerLevel}";
    }
}
