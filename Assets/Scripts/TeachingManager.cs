using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeachingManager : MonoBehaviour
{
    [SerializeField] LessonData lessonData;
    PictureSelect pictureSelect;
    DragAndDrop dragAndDrop;
    private int itemIndex = 0;

    private void OnEnable()
    {
        Lesson.onFinish += GoToNext;
    }

    private void OnDisable()
    {
        Lesson.onFinish -= GoToNext;
    }

    private void Start()
    {
        LoadTask();
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
        }
    }

    void LoadTask()
    {
        var lessonComponent = GetLessonComponent(lessonData.tasks[itemIndex].taskType);
        Debug.Log(lessonData.tasks[itemIndex].taskType + " " + lessonComponent);
        lessonComponent.Activate(lessonData.tasks[itemIndex]);
    }
}
