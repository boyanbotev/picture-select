using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PictureSelect: Lesson
{
    public static event Action onFinish;
    public static event Action<string> onCorrect;
    public static event Action onFalse;
    private bool isActive = false;

    [SerializeField] PictureSelectChallenge[] pictureSelectSequence;
    private TaskElementData[] taskSequence;
    private int itemIndex = 0;
    private UIDocument uiDoc;
    VisualElement root;

    VisualElement bodyEl;
    VisualElement imageEl;
    VisualElement wordEl;
    PictureSelectChallenge currentChallenge;

    public override void Activate(TaskData taskData) {
        if (isActive) return;

        pictureSelectSequence = taskData.elements.Select(t => {
            if (t.images == null || t.images.Length < 1)
            {
                Debug.LogError("Images not found");
            } 
            else if (t.words == null || t.words.Length < 1)
            {
                Debug.LogError("Words not found");
            }

            return new PictureSelectChallenge(t.images, t.words[0]);
        }).ToArray();

        BuildContainers();
        BuildChallenge();

        isActive = true;
    }

    public override void Deactivate() {
        if (!isActive) return;

        bodyEl.Clear();

        isActive = false;
    }

    void BuildChallenge() {
        currentChallenge = pictureSelectSequence[itemIndex];

        CreateTargetWord();
        CreateImages();
    }

    void BuildContainers() {
        uiDoc = FindObjectOfType<UIDocument>();
        root = uiDoc.rootVisualElement;
        bodyEl = root.Q(className: "body");

        imageEl = new VisualElement();
        imageEl.AddToClassList("selectable-images");
        bodyEl.Add(imageEl);

        wordEl = new VisualElement();
        wordEl.AddToClassList("word");
        bodyEl.Add(wordEl);
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

    void CreateImage(ImageData imageData)
    {
        var image = new ClickableImage(imageData.name, imageData.texture);
        imageEl.Add(image);
        image.RegisterCallback<PointerDownEvent>(evt => OnImageSelect(imageData.name));
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
        Deactivate();
        Reset();
        onFinish?.Invoke();
    }

    void OnIncorrectAnswer() {
        // TODO: shake item
        onFalse?.Invoke();
    }

    void Reset() {
        itemIndex = 0;
    }

    private IEnumerator WinRoutine() {
        yield return new WaitForSeconds(0.5f);
        itemIndex++;
        BuildChallenge();
    }
}
