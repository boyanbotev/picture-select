using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PictureSelectChallenge
{
    public ImageData[] images;
    public string word = "test";

    public PictureSelectChallenge(ImageData[] images, string word)
    {
        this.images = images;
        this.word = word;
    }
}
