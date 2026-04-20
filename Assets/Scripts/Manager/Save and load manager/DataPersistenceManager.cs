using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    private List<IDataPersistence> allDataPersistenceObjects =  new List<IDataPersistence>();
    
    private void Start()
    {
        MonoBehaviour[] allGameObject = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var obj in allGameObject)
        {
            if (obj is IDataPersistence)
            {
                allDataPersistenceObjects.Add(obj as IDataPersistence);
            }
        }
        LoadGame();
    }

    //To tell everyone to use me to save game
    public void SaveGame()
    {
        foreach (var dataPersistemceObject in allDataPersistenceObjects)
        {
            dataPersistemceObject.SaveData();
        }
    }
    
    //To tell everyone to use me to save game
    public void LoadGame()
    {
        foreach (var dataPersistemceObject in allDataPersistenceObjects)
        {
            dataPersistemceObject.LoadData();
        }
    }

    public void SaveGame<T>(T data, FileDataHandler<T> fileDataHandler) where T : GameData
    {
        fileDataHandler.SaveData(data);   
    }

    public void LoadGame<T>(ref T data, FileDataHandler<T> fileDataHandler) where T : GameData
    {
        fileDataHandler.LoadData(ref data);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Save game on close");
        SaveGame();
    }
    
}
