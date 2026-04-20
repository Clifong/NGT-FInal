using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class FileDataHandler<T> where T : GameData
{
    private string dataDirPath;
    private string fileName = "";
    private readonly string encryptionKey = "test";
    private readonly bool useEncryption = false;

    public FileDataHandler(string fileName)
    {
        this.fileName = fileName;
    }

    public void LoadData(ref T data)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(fullPath);
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        dataToLoad = sr.ReadToEnd();
                    }
                }
                
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                
                // data = JsonUtility.FromJson<T>(dataToLoad);
                data = JsonConvert.DeserializeObject<T>(dataToLoad, data.GetCustomSerializer());
            }
            catch (Exception e)
            {
                Debug.LogError($"Cannot load file for path: {fullPath}\nERROR: {e}");
            }
        }
    }

    public void SaveData(T data)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            // string dataToStore = JsonUtility.ToJson(data, true);
            string dataToStore = JsonConvert.SerializeObject(data, data.GetCustomSerializer());

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(dataToStore);
                }   
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Cannot save file to {fullPath}\nERROR: {e}");
        }
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        int i = 0;
        foreach (char c in data)
        {
            modifiedData += (c ^  encryptionKey[i % encryptionKey.Length]);
            i++;
        }
        return modifiedData;
    }
    
}
