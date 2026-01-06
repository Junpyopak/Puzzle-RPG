using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveContext : MonoBehaviour
{
    public static SaveContext Instance;
    public int currentSlot = -1;

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


