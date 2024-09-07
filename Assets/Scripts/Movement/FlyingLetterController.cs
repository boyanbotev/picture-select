using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FlyingState
{
    Random,
    Returning
}

public class FlyingLetterController : MonoBehaviour
{
    [SerializeField] float xBounds;
    [SerializeField] float yBounds;
    Vector3 startPos;
    Rigidbody2D rb;
    FlyingState state;
    Vector2 randomDir;

    void Start()
    {
        startPos = transform.position;

        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("SetRandomDir", 0f, 0.1f);
        randomDir = new Vector2(Random.Range(-xBounds, xBounds), Random.Range(-yBounds, yBounds));
    }

    void FixedUpdate()
    {
        Vector2 moveDir;
        // either move back to centre or go in random direction
        Vector2 dir = startPos - transform.position;

        if (dir.magnitude > 1) state = FlyingState.Returning;
        else state = FlyingState.Random;

        if (state == FlyingState.Returning)
        {
            moveDir = dir;
        } else
        {
            moveDir = randomDir;
        }
        rb.AddForce(moveDir);
    }

    void SetRandomDir()
    {
        randomDir = new Vector2(randomDir.x + Random.Range(-xBounds, xBounds) / 10, randomDir.y + Random.Range(-yBounds, yBounds) / 10);
    }
}