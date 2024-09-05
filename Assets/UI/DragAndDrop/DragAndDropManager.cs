using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDrop : Lesson
{
    public static event Action<string> onCorrectWord;
    private DragAndDropItem[] dragAndDropSequence;

    private UIDocument uiDoc;
    VisualElement root;
    VisualElement draggableLettersEl;
    VisualElement writingLinesEl;
    VisualElement imageEl;
    VisualElement bodyEl;
    private DraggableLetter draggedElement;
    bool isDragging = false;
    private bool isActive = false;

    private List<WritingLine> writingLines;

    public override void Activate(TaskData taskData)
    {
        if (isActive) return;

        dragAndDropSequence = taskData.elements.Select(t => {
            return new DragAndDropItem(t.images[0].texture, t.letters, t.images[0].name);
        }).ToArray();

        BuildContainers();
        BuildChallenge();

        isActive = true;
    }

    public override void Deactivate()
    {
        if (!isActive) return;

        bodyEl.Clear();

        isActive = false;
    }

    void BuildContainers()
    {
        uiDoc = FindObjectOfType<UIDocument>();
        root = uiDoc.rootVisualElement;
        bodyEl = root.Q(className: "body");

        writingLinesEl = new VisualElement();
        writingLinesEl.AddToClassList("writing-lines");
        bodyEl.Add(writingLinesEl);

        imageEl = new VisualElement();
        imageEl.AddToClassList("images");
        bodyEl.Add(imageEl);

        draggableLettersEl = new VisualElement();
        draggableLettersEl.AddToClassList("draggable-letters");
        bodyEl.Add(draggableLettersEl);
    }

    protected override void BuildChallenge()
    {
        CreateDraggableLetters();
        CreateWritingLines();
        CreateImage();
    }

    void CreateDraggableLetters() {
        draggableLettersEl.Clear();

        foreach (string letter in dragAndDropSequence[itemIndex].letters)
        {
            var draggableLetter = new DraggableLetter(letter);

            draggableLetter.RegisterCallback<PointerDownEvent>(evt => OnDragStart(evt, draggableLetter));
            draggableLetter.RegisterCallback<PointerMoveEvent>(evt => OnDrag(evt, draggableLetter));
            draggableLetter.RegisterCallback<PointerUpEvent>(evt => OnDragEnd(evt, draggableLetter));
            draggableLettersEl.Add(draggableLetter);
        }
    }

    void CreateWritingLines()
    {
        writingLines = new();
        writingLinesEl.Clear();

        for (int i = 0; i < dragAndDropSequence[itemIndex].word.Length; i++)
        {
            var writingLine = new WritingLine();

            writingLinesEl.Add(writingLine);
            writingLines.Add(writingLine);
        }
    }

    void CreateImage()
    {
        imageEl.Clear();

        var item = dragAndDropSequence[itemIndex];

        ClickableImage image = new(item.word, item.texture);
        imageEl.Add(image);
    }

    private void OnDragStart(PointerDownEvent evt, DraggableLetter letter)
    {
        draggedElement = letter;
        letter.Select();
        isDragging = true;
        letter.CaptureMouse();
    }

    private void OnDrag(PointerMoveEvent evt, DraggableLetter letter)
    {
        if (isDragging && draggedElement != null && draggedElement.HasMouseCapture())
        {
            SetDraggedPos(evt.position);
        }
    }

    private void OnDragEnd(PointerUpEvent evt, DraggableLetter draggedLetter)
    {
        if (isDragging && draggedElement != null)
        {
            WritingLine target = GetTarget(draggedElement);

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
                        // Move old letter back to original position
                        ResetLetter(target.letter);
                    }
                }

                // move to new pos
                target.AddLetter(draggedLetter);
            }
            else
            {
                ResetLetter(draggedLetter);
            }

            EvaluateWord();

            draggedElement.ReleaseMouse();
            isDragging = false;
            draggedElement = null;
        }
    }

    void SetDraggedPos(Vector2 pos)
    {
        float elementWidth = draggedElement.resolvedStyle.width;
        float elementHeight = draggedElement.resolvedStyle.height;

        var clampedPos = new Vector2(
            Math.Clamp(pos.x, bodyEl.worldBound.x + elementWidth / 2, bodyEl.worldBound.x + bodyEl.worldBound.width - elementWidth / 2),
            Math.Clamp(pos.y, bodyEl.worldBound.y + elementHeight / 2, bodyEl.worldBound.y + bodyEl.worldBound.height - elementHeight / 2)
        );

        var adjustedPos = new Vector2(
            clampedPos.x - draggedElement.originalPos.x - elementWidth / 2,
            clampedPos.y - draggedElement.originalPos.y - elementHeight / 2
        );

        draggedElement.style.left = adjustedPos.x;
        draggedElement.style.top = adjustedPos.y;
    }

    void EvaluateWord()
    {
        string joinedWord = "";

        foreach (var writingLine in writingLines)
        {
            if (writingLine.letter != null)
            {
                joinedWord += writingLine.letter.value;
            }
        }

        if (joinedWord == dragAndDropSequence[itemIndex].word)
        {
            OnCorrectAnswer();
        }
    }

    protected override void OnCorrectAnswer()
    {
        if (itemIndex < dragAndDropSequence.Length - 1)
        {
            onCorrectWord?.Invoke(dragAndDropSequence[itemIndex].word);
            StartCoroutine("WinRoutine");
        } else
        {
            OnFinishSequence();
        }

    }

    private WritingLine GetTarget(DraggableLetter draggedLetter)
    {
        WritingLine target = null;

        foreach (var line in writingLines)
        {
            if (IsOverlapping(draggedLetter, line))
            {
                target = line;
                break;
            }
        }

        return target;
    }

    private void ResetLetter(DraggableLetter draggedLetter)
    {
        draggedLetter.style.left = 0;
        draggedLetter.style.top = 0;
 
        draggedLetter.line = null;
    }

    private bool IsOverlapping(VisualElement a, VisualElement b)
    {
        Rect rect1 = new(a.worldBound.position, a.worldBound.size);
        Rect rect2 = new(b.worldBound.position, b.worldBound.size);
        return rect1.Overlaps(rect2);
    }
}
