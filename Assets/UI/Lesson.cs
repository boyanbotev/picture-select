using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lesson : MonoBehaviour
{
    public abstract void Activate(TaskData taskData);
    public abstract void Deactivate();
}

public enum TaskType
{
    PictureSelect,
    DragAndDrop,
}


[System.Serializable]
public class TaskElementData
{
    public ImageData[] images;
    public string[] words;
}

[System.Serializable]
public class TaskData
{
    public TaskElementData[] elements;
    public TaskType taskType;
}

[System.Serializable]
public class LessonData
{
    public TaskData[] tasks;
}

