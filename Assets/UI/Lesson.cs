using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lesson : MonoBehaviour
{
    public static event Action onFinish;
    public static event Action<string> onCorrect;
    public int itemIndex = 0;
    public abstract void Activate(TaskData taskData);
    public abstract void Deactivate();
    protected abstract void BuildChallenge();

    protected virtual void OnCorrectAnswer(string word)
    {
        onCorrect?.Invoke(word);
        // Should call OnFinishSequence when appropriate
    }

    protected void Reset()
    {
        itemIndex = 0;
    }

    protected virtual void OnFinishSequence()
    {
        Deactivate();
        Reset();
        onFinish?.Invoke();
    }

    protected virtual IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        itemIndex++;
        BuildChallenge();
    }

    protected virtual IEnumerator FinishRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        OnFinishSequence();
    }
}

public enum TaskType
{
    PictureSelect,
    DragAndDrop,
}

public enum LessonType {
    Lesson,
    Fight,
}


[System.Serializable]
public class TaskElementData
{
    public ImageData[] images;
    public string[] words;
    public string[] letters;
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
    public LessonType lessonType;
}

