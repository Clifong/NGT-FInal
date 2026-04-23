using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public static class GameLevelManager 
{
    public static void LoadLevel(string level)
    {
        LoadingScene.AddTask(LoadLevelIEnumerator(level));
    }

    public static IEnumerator LoadLevelIEnumerator(string level)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        while (!op.isDone)
        {
            yield return op.progress;
        }
    }
}
