using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

enum ArcEnemyState
{
    Inactive,
    Ready,
    Chasing,
    Jumping,
    Attacking,
    Stunned,
    Dead,
}

public class ArcJumpingController : MonoBehaviour
{
    public static event Action onHitPlayer;
    [SerializeField] Transform target;
    [SerializeField] float minApproachDistance = 10;
    [SerializeField] float minJumpDistance = 8;
    [SerializeField] float minAttackDistance = 3;
    private ArcEnemyState state;
    private Rigidbody2D rb;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private int health = 5;
    [SerializeField] private bool overrideHealth = false;
    [SerializeField] private TextMeshPro healthDisplay;
    [SerializeField] private float stunTime = 2f;
    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = ArcEnemyState.Inactive;
        InvokeRepeating("UpdateState", 0, 0.1f);
        UpdateHealth(health);
    }

    public void Activate(int? health)
    {
        if (health != null && !overrideHealth)
        {
            this.health = health.Value;
        }

        state = ArcEnemyState.Ready;
    }


    void UpdateState()
    {
        Debug.Log(state);
        if (state == ArcEnemyState.Stunned || state == ArcEnemyState.Dead) return;

        Vector2 dir = target.position - transform.position;
        if (dir.magnitude <= minAttackDistance)
        {
            state = ArcEnemyState.Attacking;
            Debug.Log("Game over fam");

            onHitPlayer?.Invoke();
        }
        else if (dir.magnitude <= minJumpDistance && isGrounded)
        {
            state = ArcEnemyState.Jumping;
        }
        else if (dir.magnitude <= minApproachDistance)
        {
            if (state == ArcEnemyState.Inactive || state == ArcEnemyState.Ready)
            {
                var lessonObject = GetComponent<LessonObject>();
                //FindObjectOfType<TeachingManager>().Activate(lessonObject.lessonData);
            }
            state = ArcEnemyState.Chasing;
        }
    }

    void FixedUpdate()
    {
        if (state == ArcEnemyState.Chasing && isGrounded)
        {
            MoveToTarget(speed);
        } 
        else if (state == ArcEnemyState.Chasing && !isGrounded)
        {
            MoveToTarget(speed * 0.75f);
        }

        if (state == ArcEnemyState.Jumping && isGrounded)
        {
            Jump();
            isGrounded = false;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    void MoveToTarget(float moveSpeed)
    {
        Vector2 moveDir = target.position - transform.position;
        rb.AddForce(moveDir.normalized * moveSpeed);
    }

    void Stagger()
    {
        Debug.Log("Stagger");
        state = ArcEnemyState.Stunned;
        StartCoroutine(StaggerRoutine(stunTime));
    }

    private void UpdateHealth(int health)
    {
        Debug.Log("Update health" + health);
        this.health = health;
        healthDisplay.text = "Health: " + health;
    }

    IEnumerator StaggerRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        state = ArcEnemyState.Ready;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            Stagger();
            UpdateHealth(health - 1);
            Destroy(collision.gameObject);

            if (health <= 0)
            {
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static)
        {
            isGrounded = true;
        }
    }
}
