using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class SoundManager : MonoBehaviour
{
    //public static SoundManager Instance;

    [Header("유저 슬라이더 설정값 (0~1)")]
    public float masterVolume;
    public float bgmVolume;
    public float sfxVloume;

    [Header("최종 계산된 결과값 (스토리지 볼륨)")]
    // 실제 스피커로 나갈 저장된 볼륨
    public float s_master;
    public float s_bgm;
    public float s_sfx;

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

    private void Update()
    {
        // 일시정지 메뉴가 열려있을 때 실시간으로 값을 가져오고 계산하기
        if (GameManager.Instance.UIMgr.isOpenPauseMenu == true)
        {
            // UI 슬라이더에서 현재 위치값을 가져옴
            masterVolume = GameManager.Instance.UIMgr.masterSlider.value;
            bgmVolume = GameManager.Instance.UIMgr.bgmSlider.value;
            sfxVloume = GameManager.Instance.UIMgr.sfxSlider.value;

            // 마스터 볼륨을 기준으로 백분율 계산 후 's'에 저장
            CalculateS_Storage();
        }
    }

    private void CalculateS_Storage()
    {
        // 마스터는천장 역할
        s_master = masterVolume;

        // 최종 BGM = 마스터(전체 높이) * BGM 설정비율
        s_bgm = masterVolume * bgmVolume;

        // 최종 SFX = 마스터(전체 높이) * SFX 설정비율
        s_sfx = masterVolume * sfxVloume;
    }


    public void SoundPlay(string type, string soundName , AudioClip clip)
    {
        GameObject go = new GameObject(type + "_" +soundName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;

        // 타입에 따른 볼륨 할당
        switch (type.ToLower())
        {
            case "bgm":
                audioSource.volume = s_bgm;
                break;
            case "sfx":
                audioSource.volume = s_sfx;
                break;
            default:
                Debug.Log("현재 오디오의 타입이 정해지지 않았습니다.");
                Destroy(go);
                break;
        }

        Debug.Log("현재 재생된 오디오의 볼륨 : " + audioSource.volume);
        Debug.Log($"[Type] {type} , [Name] {soundName}");

        long memorySize = Profiler.GetRuntimeMemorySizeLong(go); //바이트 단위로 출력됨 메모리를 알기 위함.
        Debug.Log($"[Memory] {go.name} 오브젝트 메모리: {memorySize / 1024f:F2} KB"); //현재 사용중인 메모리량.

        audioSource.Play();
        Destroy(go,clip.length);
    }
}