
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickableLetter : VisualElement
{
    public static event Action<string> onSelect;
    public string value;
    private string draggableLetterClassName = "clickable-letter";

    public ClickableLetter(char letter)
    {
        value = char.ToString(letter);
        AddToClassList(draggableLetterClassName);

        var letterLabel = new Label(value);
        Add(letterLabel);

        RegisterCallback<PointerDownEvent>(evt => Select());
    }

    public void Select() {
        onSelect?.Invoke(value);
    }
}
