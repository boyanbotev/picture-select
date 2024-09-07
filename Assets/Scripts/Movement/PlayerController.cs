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
    private Vector2 targetPos;
    private PlayerState currentState = PlayerState.Inactive;
    private Rigidbody2D rb;
    private Camera mainCamera;
    TeachingManager teachingManager;

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
            rb.velocity = Vector2.Lerp(rb.velocity, dir + (dir.normalized * normalizedDirMultiplier) * speed, 0.1f);  // Interpolate for smoother movement
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
        }
        else
        {
            rb.velocity = Vector2.zero;
            currentState = PlayerState.Inactive;
        }
    }

    private void HandleInput()
    {
        Vector3 inputPosition;

        inputPosition = Input.mousePosition;

        if (currentState != PlayerState.Fighting)
        {
            currentState = PlayerState.Active;
            targetPos = Camera.main.ScreenToWorldPoint(inputPosition);
        }

        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(inputPosition));
        if (rayHit.collider) OnClick(rayHit.collider.gameObject);
    }

    private void OnClick(GameObject gameObject)
    {
        Debug.Log(gameObject);

        LessonObject lessonObject = gameObject.GetComponent<LessonObject>();

        if (lessonObject != null)
        {
            teachingManager.Activate(lessonObject.lessonData);
            currentState = PlayerState.Fighting;
        }
    }

    public void Reset()
    {
        currentState = PlayerState.Inactive;
    }
}
