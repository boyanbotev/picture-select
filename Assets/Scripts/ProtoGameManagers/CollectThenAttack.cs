using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum CollectThenAttackState
{
    Collecting,
    Attacking
}
public class CollectThenAttack : MonoBehaviour
{
    private CollectThenAttackState state;
    PlayerHealth playerHealth;

    private void OnEnable()
    {
        ArcJumpingController.onHitPlayer += OnPlayerHit;
        PictureSelect.onFalse += Decrement;
    }

    private void OnDisable()
    {
        ArcJumpingController.onHitPlayer -= OnPlayerHit;
        PictureSelect.onFalse -= Decrement;
    }

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        state = CollectThenAttackState.Collecting;
        InvokeRepeating("UpdateState", 1f, 0.2f);
    }

    void UpdateState()
    {
        if (state == CollectThenAttackState.Collecting)
        {
            if (FindObjectOfType<FlyingLetterController>() == null)
            {
                state = CollectThenAttackState.Attacking;
                FindObjectOfType<TeachingManager>().Activate(FindObjectOfType<LessonObject>().lessonData);
            }
        } 
        else if (state == CollectThenAttackState.Attacking)
        {
            if (FindObjectOfType<ArcJumpingController>() == null)
            {
                GoToNext();
            }
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

    void OnPlayerHit()
    {
        if (state == CollectThenAttackState.Collecting || state == CollectThenAttackState.Attacking)
        {
            Decrement();
        }
    }

    void Decrement()
    {
        playerHealth.DecrementHealth();
        if (playerHealth.health == 0)
        {
            Restart();
        }
    }

    public void Restart()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }
}
