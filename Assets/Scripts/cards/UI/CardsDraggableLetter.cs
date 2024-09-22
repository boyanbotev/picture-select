using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class CardsDraggableLetter : VisualElement
{
    public static event Action<string> onSelect;
    public CardsWritingLine line;
    public Vector2 originalPos;
    public string value;
    private string draggableLetterClassName = "draggable-letter";
    private VisualElement bodyEl;

    public CardsDraggableLetter(string letter, VisualElement mainEl)
    {
        value = letter;
        AddToClassList(draggableLetterClassName);

        var letterLabel = new Label(letter);
        Add(letterLabel);
        this.bodyEl = mainEl;
    }

    public void Select()
    {
        onSelect?.Invoke(value);
        CalculatePos();
    }
    private void CalculatePos()
    {
        Vector2 pos = worldTransform.GetPosition();
        originalPos = new Vector2(pos.x - style.left.value.value, pos.y - style.top.value.value);
    }

    public void Reset()
    {
        style.left = 0;
        style.top = 0;
        line = null;
    }

    public void SetDraggedPos(Vector2 pos)
    {
        float elementWidth = resolvedStyle.width;
        float elementHeight = resolvedStyle.height;

        var clampedPos = new Vector2(
            Math.Clamp(pos.x, bodyEl.worldBound.x + elementWidth / 2, bodyEl.worldBound.x + bodyEl.worldBound.width - elementWidth / 2),
            Math.Clamp(pos.y, bodyEl.worldBound.y + elementHeight / 2, bodyEl.worldBound.y + bodyEl.worldBound.height - elementHeight / 2)
        );

        var adjustedPos = new Vector2(
            clampedPos.x - originalPos.x - elementWidth / 2,
            clampedPos.y - originalPos.y - elementHeight / 2
        );

        style.left = adjustedPos.x;
        style.top = adjustedPos.y;
    }
}
