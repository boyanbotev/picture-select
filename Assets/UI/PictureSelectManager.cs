using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PictureSelectManager : MonoBehaviour
{
    public static event Action onFinish;
    public static event Action<string> onCorrect;
    public static event Action onFalse;

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

        BuildChallenge();
    }

    void BuildChallenge() {
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
            OnCorrectAnswer();
        }
        else {
            Debug.Log("INCORRECT");
            OnIncorrectAnswer();
        }
    }

    void OnCorrectAnswer() {
        onCorrect?.Invoke(currentChallenge.word);

        if (itemIndex < pictureSelectSequence.Length - 1) {
            StartCoroutine(WinRoutine());
        }
        else {
            Debug.Log("FINISH SEQUENCE");
            OnFinishSequence();
        }
    }

    void OnFinishSequence() {
        onFinish?.Invoke();
    }

    void OnIncorrectAnswer() {
        onFalse?.Invoke();
        // TODO: shake item
    }

    private IEnumerator WinRoutine() {
        yield return new WaitForSeconds(0.5f);
        itemIndex++;
        BuildChallenge();
    }
}
