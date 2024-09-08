using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

enum EnemyState
{
    Inactive,
    Ready,
    Chasing,
    Attacking,
    Stunned,
    Dead,
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float minApproachDistance = 10;
    [SerializeField] float minAttackDistance = 3;
    private EnemyState state;
    private Rigidbody2D rb;
    [SerializeField] private float speed = 1f;
    [SerializeField] private int health = 5;
    [SerializeField] private TextMeshPro healthDisplay;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = EnemyState.Inactive;
        InvokeRepeating("UpdateState", 0, 0.1f);
        UpdateHealth(health);
    }

    public void Activate(int? health)
    {
        if (health != null)
        {
            this.health = health.Value;
        }

        state = EnemyState.Ready;
    }


    void UpdateState()
    {
        Debug.Log(state);
        if (state == EnemyState.Stunned || state == EnemyState.Dead) return;

        Vector2 dir = target.position - transform.position;
        if (dir.magnitude <= minAttackDistance)
        {
            state = EnemyState.Attacking;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (dir.magnitude <= minApproachDistance)
        {
            if (state == EnemyState.Inactive || state == EnemyState.Ready)
            {
                var lessonObject = GetComponent<LessonObject>();
                FindObjectOfType<TeachingManager>().Activate(lessonObject.lessonData);
            }
            state = EnemyState.Chasing;
        }
    }

    void FixedUpdate()
    {
        if (state == EnemyState.Chasing)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget() {
        Vector2 moveDir = target.position - transform.position;
        rb.AddForce(moveDir.normalized * speed);
    }

    void Stagger()
    {
        Debug.Log("Stagger");
        state = EnemyState.Stunned;
        StartCoroutine(StaggerRoutine(2f));
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
        state = EnemyState.Ready;
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
    }
}
