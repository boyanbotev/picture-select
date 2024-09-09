using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

enum PlayerState
{
    Active,
    Inactive,
    Fighting
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxYposition = 0;
    [SerializeField] private float minYposition = -20;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float minDistance = 0.4f;
    [SerializeField] private float normalizedDirMultiplier = 10;
    [SerializeField] private float maxVelocity = 10;
    [SerializeField] private float fireSpeed = 10;
    [SerializeField] GameObject projectilePrefab;
    private Vector2 targetPos;
    private PlayerState currentState = PlayerState.Inactive;
    private Rigidbody2D rb;
    private Camera mainCamera;
    TeachingManager teachingManager;

    private void OnEnable()
    {
        Lesson.onCorrect += FireAtEnemy;
        TeachingManager.OnStart += SetAsFighting;
    }

    private void OnDisable()
    {
        Lesson.onCorrect -= FireAtEnemy;
        TeachingManager.OnStart -= SetAsFighting;
    }

    public void SetAsFighting()
    {
        currentState = PlayerState.Fighting;
    }
    void FireAtEnemy(string word)
    {
        var enemy = FindObjectOfType<EnemyController>();
        if (enemy != null) Fire(enemy.transform.position);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        teachingManager = FindObjectOfType<TeachingManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    // fire green blob
    public void Fire(Vector2? target)
    {
        Vector2 fireTargetPos;
        if (target != null)
        {
            fireTargetPos = target.Value;
        } 
        else
        {
            fireTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        Vector2 fireDir = new(fireTargetPos.x - transform.position.x, fireTargetPos.y - transform.position.y);
        var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<Rigidbody2D>().velocity = fireDir.normalized * fireSpeed;
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.Active)
        {
            Move();
        }
    }

    private void Move()
    {
        var clampedPos = new Vector2(targetPos.x, Mathf.Clamp(targetPos.y, minYposition, maxYposition));

        var dir = new Vector2(clampedPos.x - transform.position.x, clampedPos.y - transform.position.y);

        if (dir.magnitude > minDistance)
        {
            rb.velocity = Vector2.ClampMagnitude(Vector2.Lerp(rb.velocity, dir + (dir.normalized * normalizedDirMultiplier) * speed, 0.1f), maxVelocity);
        }
        else
        {
            rb.velocity = Vector2.zero;
            currentState = PlayerState.Inactive;
        }
    }

    private void HandleInput()
    {
        if (currentState != PlayerState.Fighting)
        {
            currentState = PlayerState.Active;
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition));
        if (rayHit.collider) OnClick(rayHit.collider.gameObject);
    }

    private void OnClick(GameObject gameObject)
    {
        Debug.Log(gameObject);

        LessonObject lessonObject = gameObject.GetComponent<LessonObject>();
        FlyingLetterController flyingLetterController = gameObject.GetComponent<FlyingLetterController>();

        if (lessonObject != null)
        {
            teachingManager.Activate(lessonObject.lessonData);
            currentState = PlayerState.Fighting;
        } 
        else if (flyingLetterController != null)
        {
            Destroy(flyingLetterController.gameObject);
            targetPos = transform.position;
        }
    }

    public void Reset()
    {
        currentState = PlayerState.Inactive;
    }
}
