using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CardsScenePlayerController : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float fireSpeed;
    Camera mainCamera;

    [SerializeField] private float minDistance = 0.4f;
    [SerializeField] private float normalizedDirMultiplier = 10;
    [SerializeField] private float maxVelocity = 10;
    [SerializeField] private float speed = 10;
    private Vector2 targetPos;
    private Rigidbody2D rb;
    private PlayerState currentState = PlayerState.Inactive;
    private UIDocument uiDoc;
    private VisualElement rootEl;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        uiDoc = FindObjectOfType<UIDocument>();
        rootEl = uiDoc.rootVisualElement;
    }

    private void OnEnable()
    {
        UIManager.onCardUsed += FireWordProjectile;
    }

    private void OnDisable()
    {
        UIManager.onCardUsed -= FireWordProjectile;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, LayerMask.GetMask("Collectable"));
            if (rayHit.collider)
            {
                OnClick(rayHit.collider.gameObject);
            }
            else if (!IsPointerOverUIWithClass("card") && !IsPointerOverUIWithClass("letters"))
            {
                currentState = PlayerState.Active;
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentState == PlayerState.Active)
        {
            Move();
        }
    }

    void FireWordProjectile(string word)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        var chosenEnemies = enemies.Where(enemy => enemy.word == word).ToArray();

        foreach (var chosenEnemy in chosenEnemies)
        {
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<TextMeshPro>().text = word;

            var dir = new Vector2(chosenEnemy.transform.position.x - transform.position.x, chosenEnemy.transform.position.y - transform.position.y);
            projectile.GetComponent<Rigidbody2D>().velocity = dir.normalized * fireSpeed;
        }

    }

    void OnClick(GameObject gameObject)
    {
        CollectableLetter letter = gameObject.GetComponent<CollectableLetter>();
        if (letter != null)
        {
            letter.Collect();
        }
    }

    GameObject GetClosestElement(Enemy[] elements, Vector3 playerPosition)
    {
        GameObject closestElement = null;
        float closestDistance = Mathf.Infinity;

        foreach (var element in elements)
        {
            // Calculate the distance between the player and the current element
            float distance = Vector3.Distance(playerPosition, element.transform.position);

            // If this element is closer than the previously found closest, update closest
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestElement = element.gameObject;
            }
        }

        return closestElement;
    }

    private void Move()
    {
        var dir = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);

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

    private bool IsPointerOverUIWithClass(string className)
    {
        List<VisualElement> items = rootEl.Query<VisualElement>(null, className).ToList();

        if (items != null)
        {
            foreach (var item in items)
            {
                Debug.Log(item);
                if (IsInsideElement(item, RuntimePanelUtils.CameraTransformWorldToPanel(rootEl.panel, Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main)))
                {
                    Debug.Log("found");
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsInsideElement(VisualElement v, Vector2 pos)
    {
        Debug.Log(v.worldBound + "" + pos);

        return v.worldBound.Contains(pos);
    }
}
