using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static event Action<string> onCardUsed;
    UIDocument uiDoc;
    VisualElement root;
    VisualElement mainEl;
    VisualElement cardsEl;
    VisualElement letters;

    private bool isDragging = false;
    private CardsGameManager gameManager;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        mainEl = root.Q(className: "main");
        cardsEl = root.Q(className: "cards");
        letters = root.Q(className: "letters");
        Application.targetFrameRate = 60;

        gameManager = FindObjectOfType<CardsGameManager>();
    }

    private void OnEnable()
    {
        CollectableWord.onCollected += CreateCard;
        CollectableLetter.onCollect += CreateDraggableLetter;
    }

    private void OnDisable()
    {
        CollectableWord.onCollected -= CreateCard;
        CollectableLetter.onCollect -= CreateDraggableLetter;
    }

    void CreateDraggableLetter(string letter)
    {
        var draggableLetter = new CardsDraggableLetter(letter, mainEl);

        draggableLetter.RegisterCallback<PointerDownEvent>(evt => OnDragStart(evt, draggableLetter));
        draggableLetter.RegisterCallback<PointerMoveEvent>(evt => OnDrag(evt, draggableLetter));
        draggableLetter.RegisterCallback<PointerUpEvent>(evt => OnDragEnd(evt, draggableLetter));
        letters.Add(draggableLetter);
    }

    // is a mini drag and drop challenge
    void CreateCard(string word)
    {
        Debug.Log("create card" + word);
        var card = new Card(word, gameManager);

        cardsEl.Add(card);

        //card.RegisterCallback<PointerDownEvent>(evt => OnCardClick(evt, card, word));
    }

    void OnCardClick(PointerDownEvent evt, VisualElement card, string word)
    {
        onCardUsed?.Invoke(word);
        StartCoroutine(RemoveCardRoutine(card));
    }

    private void OnDragStart(PointerDownEvent evt, CardsDraggableLetter letter)
    {
        if (cardsEl.childCount == 0)
        {
            return;
        }

        letter.Select();
        isDragging = true;
        letter.CaptureMouse();
    }

    private void OnDrag(PointerMoveEvent evt, CardsDraggableLetter letter)
    {
        if (isDragging && letter.HasMouseCapture())
        {
            letter.SetDraggedPos(evt.position);
        }
    }

    private void OnDragEnd(PointerUpEvent evt, CardsDraggableLetter draggedLetter)
    {
        if (!isDragging) return;

        CardsWritingLine target = GetTarget(draggedLetter);

        if (draggedLetter.line != null)
        {
            // set old writing line's letter reference to null
            draggedLetter.line.letter = null;
        }

        if (target != null)
        {
            // If there's a letter on the new position
            if (target.letter != null && target.letter != draggedLetter)
            {
                if (draggedLetter.line != null)
                {
                    // Swap
                    draggedLetter.line.AddLetter(target.letter);
                }
                else
                {
                    target.letter.Reset();
                }
            }

            // move to new pos
            target.AddLetter(draggedLetter);
        }
        else
        {
            draggedLetter.Reset();
        }

        // get card of writing line
        var card = target?.parent?.parent as Card;
        if (card != null)
        {
            EvaluateWord(card);
        }

        draggedLetter.ReleaseMouse();
        isDragging = false;

    }

    void EvaluateWord(Card card) // only evaluate for a given card
    {
        string joinedWord = "";

        foreach (var writingLine in card.writingLinesList)
        {
            if (writingLine.letter != null)
            {
                joinedWord += writingLine.letter.value;
            }

            if (joinedWord == card.word)
            {
                ApplyCard(card);
            }
        }
    }

    void ApplyCard(Card card)
    {
        onCardUsed?.Invoke(card.word);
        StartCoroutine(RemoveCardRoutine(card));

        foreach (var line in card.writingLinesList)
        {
            letters.Remove(line.letter);
        }
    }

    private CardsWritingLine GetTarget(CardsDraggableLetter draggedLetter)
    {
        CardsWritingLine target = null;

        foreach (Card card in cardsEl.Children())
        {
            foreach (var line in card.writingLinesList)
            {
                if (IsOverlapping(draggedLetter, line))
                {
                    target = line;
                    break;
                }
            }
        }

        return target;
    }

    private bool IsOverlapping(VisualElement a, VisualElement b)
    {
        Rect rect1 = new(a.worldBound.position, a.worldBound.size);
        Rect rect2 = new(b.worldBound.position, b.worldBound.size);
        return rect1.Overlaps(rect2);
    }

    IEnumerator RemoveCardRoutine(VisualElement card)
    {
        yield return new WaitForSeconds(0.01f);
        cardsEl.Remove(card);
    }
 }

public class Card : VisualElement
{
    public List<CardsWritingLine> writingLinesList;
    public string word;

    public Card(string word, CardsGameManager gameManager)
    {
        this.word = word;
        AddToClassList("card");

        var texture = gameManager.GetImageCorrespondingToWord(word);
        var image = new ClickableImage(word, texture);
        Add(image);

        var writingLines = new VisualElement();
        writingLines.AddToClassList("writing-lines");
        Add(writingLines);

        writingLinesList = new List<CardsWritingLine>();

        foreach (char letter in word)
        {
            CardsWritingLine writingLine = new();
            writingLines.Add(writingLine);
            writingLinesList.Add(writingLine);
        }
    }
}
