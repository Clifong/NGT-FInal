using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LoadingScene : MonoBehaviour
{
    private static Queue<IEnumerator> allTasks = new Queue<IEnumerator>();
    public TextMeshProUGUI loadingProgressText;
    public Image screen;
    public float fadeDuration = 0.2f;

    void Awake()
    {
        StartCoroutine(LoadingScreenFadeIn());
    }

    public static void AddTask(IEnumerator task)
    {
        allTasks.Enqueue(task);
    }

    private void CheckForNextTask()
    {
        if (allTasks.TryDequeue(out IEnumerator nextTask))
            StartCoroutine(RunTask(nextTask));
        else
            StartCoroutine(LoadingScreenFadeOut());
    }

    private IEnumerator RunTask(IEnumerator task)
    {
        while (task.MoveNext())
        {
            object current = task.Current;

            Debug.Log(current);

            yield return current;
        }

        CheckForNextTask();
    }

    private  IEnumerator LoadingScreenFadeIn()
    {
        screen.gameObject.SetActive(true);
        yield return StartCoroutine(LoadingScreenFade(0, 0));
        CheckForNextTask();
    }

    private IEnumerator LoadingScreenFadeOut()
    {
        yield return StartCoroutine(LoadingScreenFade(0, 0));
        CheckForNextTask();
    }


    private IEnumerator LoadingScreenFade(float startValue, float targetValue)
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            screen.color = new Color(screen.color.r, screen.color.b, screen.color.g, Mathf.Lerp(startValue, targetValue, time / fadeDuration));
            yield return null;
        }
        if (screen.gameObject.activeSelf)
        {
            screen.gameObject.SetActive(false);
        }
        yield return null;
    }
}
