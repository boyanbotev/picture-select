using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public static event Action onTouchPlayer;
    public string word = string.Empty;
    public Transform target;
    [SerializeField] float minApproachDistance = 10;
    [SerializeField] float minAttackDistance = 3;
    private EnemyState state;
    private Rigidbody2D rb;
    [SerializeField] private float speed = 1f;
    [SerializeField] private TextMeshPro healthDisplay;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = EnemyState.Chasing;
        //InvokeRepeating("UpdateState", 0, 0.1f);
    }

    void UpdateState()
    {

    }

    void FixedUpdate()
    {
        if (state == EnemyState.Chasing)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        Vector2 moveDir = target.position - transform.position;
        rb.AddForce(moveDir.normalized * speed);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var projectile = collision.gameObject.GetComponent<Projectile>();
        var textUI = collision.gameObject.GetComponent<TextMeshPro>();
        var player = target.gameObject;

        if (projectile != null)
        {
            if (textUI?.text == word)
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
            } else
            {
                
            }
        }

        if (collision.gameObject == player)
        {
            onTouchPlayer?.Invoke();
        }
    }
}
