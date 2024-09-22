using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    VisualElement lettersEl;

    private bool isDragging = false;
    private CardsGameManager gameManager;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        mainEl = root.Q(className: "main");
        cardsEl = root.Q(className: "cards");
        lettersEl = root.Q(className: "letters");
        Application.targetFrameRate = 60;

        gameManager = FindObjectOfType<CardsGameManager>();
    }

    private void OnEnable()
    {
        //CollectableWord.onCollected += CreateCard;
        CollectableLetter.onCollect += OnCollectLetter;
    }

    private void OnDisable()
    {
        //CollectableWord.onCollected -= CreateCard;
        CollectableLetter.onCollect -= OnCollectLetter;
    }

    void OnCollectLetter(string letter)
    {
        CreateDraggableLetter(letter);
        CreateCards();
    }

    void CreateCards()
    {
        cardsEl.Clear();

        string[] words = gameManager.GetWordStrings();
        string[] letters = lettersEl.Children().Select(x => {
            var draggable = x as CardsDraggableLetter;
            return draggable.value;
         }).ToArray();


        foreach (string word in words) {
            bool shouldCreate = true;
            foreach (var letter in word)
            {
                if (!letters.Contains(letter.ToString()))
                {
                    shouldCreate = false;
                }
            }
            if (shouldCreate) CreateCard(word);
        }
    }

    void CreateDraggableLetter(string letter)
    {
        var draggableLetter = new CardsDraggableLetter(letter, mainEl);

        draggableLetter.RegisterCallback<PointerDownEvent>(evt => OnDragStart(evt, draggableLetter));
        draggableLetter.RegisterCallback<PointerMoveEvent>(evt => OnDrag(evt, draggableLetter));
        draggableLetter.RegisterCallback<PointerUpEvent>(evt => OnDragEnd(evt, draggableLetter));
        lettersEl.Add(draggableLetter);
    }

    // is a mini drag and drop challenge
    void CreateCard(string word)
    {
        Debug.Log("create card" + word);
        var card = new Card(word, gameManager);

        cardsEl.Add(card);
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
                CreateCards();
            }
        }
    }

    void ApplyCard(Card card)
    {
        onCardUsed?.Invoke(card.word);

        foreach (var line in card.writingLinesList)
        {
            lettersEl.Remove(line.letter);
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
 }
