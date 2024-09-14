using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int health = 10;
    private bool isVulnerable = true;
    private float invulnerabilityTime = 0.4f;
    private TextMeshPro healthDisplay;

    private void Awake()
    {
        healthDisplay = GetComponentInChildren<TextMeshPro>();
        UpdateHealth();
    }
    public void DecrementHealth()
    {
        if (!isVulnerable) return;

        health--;
        isVulnerable = false;
        StartCoroutine(VulnerableRoutine());
        UpdateHealth();
    }

    void UpdateHealth()
    {
        healthDisplay.text = "Health: " + health;
    }

    private IEnumerator VulnerableRoutine()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        isVulnerable = true;
    }
}
