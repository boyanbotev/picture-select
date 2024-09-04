using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DragAndDropItem
{
    public Texture2D texture;
    public string[] letters = { "p", "t", "i" };
    public string word = "pit";

    public DragAndDropItem(Texture2D texture, string[] letters, string word)
    {
        this.texture = texture;
        this.letters = letters;
        this.word = word;
    }
}
