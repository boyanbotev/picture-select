using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

enum PlayerState
{
    Active,
    Inactive
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxYposition = 0;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float minDistance = 0.4f;
    private Vector2 targetPos;
    private PlayerState currentState = PlayerState.Inactive;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentState = PlayerState.Active;
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        var clampedPos = new Vector2(targetPos.x, Mathf.Clamp(targetPos.y, targetPos.y, maxYposition));

        var dir = new Vector2(clampedPos.x - transform.position.x, clampedPos.y - transform.position.y);

        if (dir.magnitude > minDistance)
        {
            //rb.velocity = dir.normalized * speed;
            // Smooth the velocity towards the target with a lerp for smoother motion
            //Vector2 desiredVelocity =  Mathf.Lerp(speed, 0, dir.magnitude / minDistance);
            rb.velocity = Vector2.Lerp(rb.velocity, dir + (dir.normalized * 10) * speed, 0.1f);  // Interpolate for smoother movement
        }
        else
        {
            rb.velocity = Vector2.zero;
            currentState = PlayerState.Inactive;
        }
    }
}