using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillAllEnemies : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("CheckAllDead", 1f, 0.2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GoToNext();
        }
    }

    void GoToNext()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (activeSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(activeSceneIndex + 1);
        }
    }

    void CheckAllDead()
    {
        if (FindObjectOfType<EnemyController>() == null)
        {
            GoToNext();
        }
    }
}
