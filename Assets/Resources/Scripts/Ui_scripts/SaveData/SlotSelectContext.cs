using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSelectContext : MonoBehaviour
{
    public static SlotSelectContext Instance;
    public SlotSelectMode mode;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
