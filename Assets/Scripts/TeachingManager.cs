using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TeachingManager : MonoBehaviour
{
    [SerializeField] LessonData lessonData;
    PictureSelect pictureSelect;
    DragAndDrop dragAndDrop;
    private int itemIndex = 0;
    UIDocument uiDoc;
    VisualElement root;
    bool isActive = false;

    private void OnEnable()
    {
        Lesson.onFinish += GoToNext;
    }

    private void OnDisable()
    {
        Lesson.onFinish -= GoToNext;
    }

    private void Awake()
    {
        uiDoc = FindObjectOfType<UIDocument>();
        root = uiDoc.rootVisualElement;
    }

    public void Activate(LessonData lessonData)
    {
        if (isActive) return;

        BuildLessonContainers();

        this.lessonData = lessonData;
        LoadTask();

        isActive = true;
    }

    void BuildLessonContainers()
    {
        // Create the main container (equivalent to <ui:VisualElement class="main">)
        VisualElement main = new VisualElement();
        main.AddToClassList("main");

        // Create the sections container (equivalent to <ui:VisualElement class="sections">)
        VisualElement sections = new VisualElement();
        sections.AddToClassList("sections");

        // Create the header (equivalent to <ui:VisualElement class="header">)
        VisualElement header = new VisualElement();
        header.AddToClassList("header");

        // Create the body (equivalent to <ui:VisualElement class="body">)
        VisualElement body = new VisualElement();
        body.AddToClassList("body");

        // Create the footer (equivalent to <ui:VisualElement class="footer">)
        VisualElement footer = new VisualElement();
        footer.AddToClassList("footer");

        // Add header, body, and footer to the sections container
        sections.Add(header);
        sections.Add(body);
        sections.Add(footer);

        // Add sections to the main container
        main.Add(sections);

        // Add the main container to the root element
        root.Add(main);
    }

    public void Deactivate()
    {
        if (!isActive) return;

        root.Clear();

        isActive = false;
    }

    // make 'create' methods one generic method
    PictureSelect CreatePictureSelect()
    {
        Debug.Log("create picture select");
        var gameObject = new GameObject("Picture Select");

        gameObject.AddComponent<PictureSelect>();
        pictureSelect = gameObject.GetComponent<PictureSelect>();
        pictureSelect.Activate(lessonData.tasks[itemIndex]);

        return pictureSelect;
    }

    DragAndDrop CreateDragAndDrop()
    {
        Debug.Log("create drag and drop" + lessonData.tasks[itemIndex].elements[0].letters);
        var gameObject = new GameObject("Drag And Drop");

        gameObject.AddComponent<DragAndDrop>();
        dragAndDrop = gameObject.GetComponent<DragAndDrop>();

        dragAndDrop.Activate(lessonData.tasks[itemIndex]);

        return dragAndDrop;
    }

    Lesson GetLessonComponent(TaskType type)
    {
        switch(type)
        {
            case TaskType.PictureSelect:
                if (pictureSelect != null) return pictureSelect;
                else return CreatePictureSelect();
            case TaskType.DragAndDrop:
                if (dragAndDrop != null) return dragAndDrop;
                else return CreateDragAndDrop();
            default:
                return null; 
        }
    }

    void GoToNext()
    {
        Debug.Log("GO TO NEXT: " + itemIndex);
        if (itemIndex < lessonData.tasks.Length - 1) {
            itemIndex++;

            LoadTask();
        } else
        {
            Deactivate();
            FindObjectOfType<PlayerController>().Reset();
        }
    }

    void LoadTask()
    {
        var lessonComponent = GetLessonComponent(lessonData.tasks[itemIndex].taskType);
        Debug.Log(lessonData.tasks[itemIndex].taskType + " " + lessonComponent);
        lessonComponent.Activate(lessonData.tasks[itemIndex]);
    }
}
