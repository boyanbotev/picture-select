using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableWord : MonoBehaviour
{
    public static event Action<string> onCollected;
    public string word;

    void Start()
    {
        InvokeRepeating("CheckStatus", 0.1f, 0.1f);
    }

    void CheckStatus()
    {
        if (transform.childCount == 0)
        {
            onCollected?.Invoke(word);
            Destroy(gameObject);
        }
    }
}
