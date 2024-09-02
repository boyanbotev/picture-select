using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeachingManager : MonoBehaviour
{
    [SerializeField] LessonData lessonData;
    PictureSelect pictureSelect;
    private int itemIndex = 0;

    private void OnEnable()
    {
        PictureSelect.onFinish += GoToNext;
    }

    private void OnDisable()
    {
        PictureSelect.onFinish -= GoToNext;
    }

    private void Start()
    {
        LoadTask();
    }

    PictureSelect CreatePictureSelect()
    {
        var gameObject = new GameObject("Picture Select");

        gameObject.AddComponent<PictureSelect>();
        pictureSelect = gameObject.GetComponent<PictureSelect>();
        pictureSelect.Activate(lessonData.tasks[itemIndex]);

        return pictureSelect;
    }

    Lesson GetLessonComponent(TaskType type)
    {
        switch(type)
        {
            case TaskType.PictureSelect:
                if (pictureSelect != null) return pictureSelect;
                else return CreatePictureSelect();
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
        lessonComponent.Activate(lessonData.tasks[itemIndex]);
    }
}
