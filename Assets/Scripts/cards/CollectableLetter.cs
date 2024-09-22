using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableLetter : MonoBehaviour
{
    public string letter;
    public static event Action<string> onCollect;

    public void Collect()
    {
        onCollect?.Invoke(letter);
        Destroy(gameObject);
    }
}
