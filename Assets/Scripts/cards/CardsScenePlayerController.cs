using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardsScenePlayerController : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float fireSpeed;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
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
            if (rayHit.collider) OnClick(rayHit.collider.gameObject);
        }
    }

    void FireWordProjectile(string word)
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<TextMeshPro>().text = word;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        var chosenEnemies = enemies.Where(enemy => enemy.word == word).ToArray();

        var chosenEnemy = GetClosestElement(chosenEnemies, transform.position);

        // choose closest

        if (chosenEnemy != null)
        {
            var dir = new Vector2(chosenEnemy.transform.position.x - transform.position.x, chosenEnemy.transform.position.y - transform.position.y);
            projectile.GetComponent<Rigidbody2D>().velocity = dir.normalized * fireSpeed;
        }

    }

    void OnClick(GameObject gameObject)
    {
        CollectableLetter letter = gameObject.GetComponent<CollectableLetter>();
        if (letter != null)
        {
            Destroy(gameObject);
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
}
