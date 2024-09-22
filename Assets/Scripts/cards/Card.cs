using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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