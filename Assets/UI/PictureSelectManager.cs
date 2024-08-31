using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PictureSelectManager : MonoBehaviour
{
    [SerializeField] PictureSelectChallenge[] pictureSelectSequence;
    private int itemIndex = 0;
    private UIDocument uiDoc;
    VisualElement root;
    VisualElement imageEl;
    VisualElement wordEl;
    PictureSelectChallenge currentChallenge;

    private void OnEnable()
    {
        uiDoc = FindObjectOfType<UIDocument>();
        root = uiDoc.rootVisualElement;
        imageEl = root.Q(className: "selectable-images");
        wordEl = root.Q(className: "word");

        currentChallenge = pictureSelectSequence[itemIndex];

        CreateTargetWord();
        CreateImages();
    }

    void CreateTargetWord() {
        wordEl.Clear();

        foreach (char c in currentChallenge.word) {
            wordEl.Add(new ClickableLetter(c));
        }
    }

    void CreateImages() {
        imageEl.Clear();

        foreach (var image in currentChallenge.images) {
            CreateImage(image);
        }
    }

    void CreateImage(ImageSelectData imageSelectData)
    {
        var image = new ClickableImage(imageSelectData.name, imageSelectData.texture);
        imageEl.Add(image);
        image.RegisterCallback<PointerDownEvent>(evt => OnImageSelect(imageSelectData.name));
    }

    void OnImageSelect(string word) {
        if (word == currentChallenge.word) {
            Debug.Log("VICTORY");
        }
        else {
            Debug.Log("INCORRECT");
        }
    }
}
