using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static event Action<string> onCardUsed;
    UIDocument uiDoc;
    VisualElement root;
    VisualElement cards;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        cards = root.Q(className: "cards");
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        CollectableWord.onCollected += CreateCard;
    }

    private void OnDisable()
    {
        CollectableWord.onCollected -= CreateCard;
    }

    void CreateCard(string word)
    {
        Debug.Log("create card" + word);
        var card = new VisualElement();
        card.AddToClassList("card");

        var label = new Label();
        label.text = word;
        label.AddToClassList("cardLabel");
        label.pickingMode = PickingMode.Position;

        card.Add(label);
        card.pickingMode = PickingMode.Position;
        cards.Add(card);

        card.RegisterCallback<PointerDownEvent>(evt => OnCardClick(evt, card, word));
    }

    void OnCardClick(PointerDownEvent evt, VisualElement card, string word)
    {
        onCardUsed?.Invoke(word);
        StartCoroutine(RemoveCardRoutine(card));
    }

    IEnumerator RemoveCardRoutine(VisualElement card)
    {
        yield return new WaitForSeconds(0.01f);
        cards.Remove(card);
    }
 }
