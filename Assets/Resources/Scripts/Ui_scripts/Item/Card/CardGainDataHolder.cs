using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardGainDataHolder : MonoBehaviour
{
    public static CardGainDataHolder Instance;
    public CardGainData Data;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Addressables.LoadAssetAsync<CardGainData>("CardData").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Data = handle.Result;
                Debug.Log("CardData Addressables 로드 완료");
            }
            else
            {
                Debug.LogError("CardData 로드 실패");
            }
        };
    }
}
