using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


[System.Serializable]
public class CollectableLetterData
{
    public Texture2D image;
    public string letter;
}

[System.Serializable]
public class WordData
{
    public string word;
    public Texture2D enemy;
}

public class CardsGameManager : MonoBehaviour
{
    [SerializeField] GameObject letterPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] CollectableLetterData[] letterData;
    [SerializeField] WordData[] words;
    [SerializeField] Transform player;
    [SerializeField] Vector2 bounds;
    [SerializeField] Vector2 safeBounds;
    private float gap = 6f;
    private float minGap = 3.5f;
    private float decrementAmount = 0.1f;

    private void Start()
    {
        var word = words[0];
        SpawnEnemy(word);
        SpawnLetters(word);

        StartCoroutine(SpawnRoutine());
    }

    void SpawnEnemy(WordData wordData)
    {
        var spawnPos = GetRandomPositionAwayFromPlayer();
        var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        var renderer = enemy.GetComponent<SpriteRenderer>();
        Texture2D texture = wordData.enemy;
        SetImage(renderer, texture);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.word = wordData.word;
        enemyScript.target = player;
    }

    void SpawnLetters(WordData wordData)
    {
        var parentObject = new GameObject();

        var letters = wordData.word.ToCharArray();
        var randomColor = GetRandomColor();

        foreach (var letter in letters)
        {
            var spawnPos = GetRandomPositionAwayFromPlayer();

            var letterObject = Instantiate(letterPrefab, spawnPos, Quaternion.identity, parentObject.transform);
            SpriteRenderer renderer = letterObject.GetComponent<SpriteRenderer>();

            var data = letterData.FirstOrDefault(data => data.letter == letter.ToString());
            Debug.Log("letter: " + letter + "  , data: " + data);
            var texture = data.image;
            SetImage(renderer, texture);
            renderer.color = randomColor;
        }


        parentObject.AddComponent<CollectableWord>();
        var collectableWord = parentObject.GetComponent<CollectableWord>();
        collectableWord.word = wordData.word;
    }

    Vector2 GetRandomPositionAwayFromPlayer()
    {
        Vector2 pos;

        do
        {
            pos = GetRandomPosition();
        }
        while (IsWithinInnerBounds(pos, safeBounds, player.position));

        return pos;
    }

    Vector2 GetRandomPosition()
    {
        var xPos = Random.Range(-bounds.x, bounds.x);
        var yPos = Random.Range(-bounds.y, bounds.y);
        return new Vector2(xPos, yPos);
    }
    Color GetRandomColor()
    {
        float r = Random.Range(0f, 0.5f);
        float g = Random.Range(0f, 0.5f);
        float b = Random.Range(0f, 0.5f);

        return new Color(r, g, b, 1f); // Alpha is set to 1 for full opacity
    }

    bool IsWithinInnerBounds(Vector2 position, Vector2 innerBounds, Vector2 playerPosition)
    {
        // Check if the position is inside the inner bounds
        return position.x < innerBounds.x &&
               position.x > -innerBounds.x &&
               position.y < innerBounds.y &&
               position.y > -innerBounds.y;
    }

    void SetImage(SpriteRenderer renderer, Texture2D texture)
    {
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private void OnEnable()
    {
        Enemy.onTouchPlayer += Restart;
    }

    private void OnDisable()
    {
        Enemy.onTouchPlayer -= Restart;
    }

    public void Restart()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
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

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(gap);
        var randomWord = words[Random.Range(0, words.Length)];
        SpawnEnemy(randomWord);
        SpawnLetters(randomWord);

        if (gap > minGap)
        {
            gap -= decrementAmount;
        }

        StartCoroutine(SpawnRoutine());
    }
}
