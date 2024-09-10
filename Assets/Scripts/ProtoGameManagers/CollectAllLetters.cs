using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectAllLetters : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("CheckAllCollected", 1f, 0.2f);
    }

    void CheckAllCollected()
    {
        if (FindObjectOfType<FlyingLetterController>() == null)
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(SceneManager.sceneCountInBuildSettings);
            if (activeSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(activeSceneIndex + 1);
            }
        }
    }
}
