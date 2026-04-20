using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance)
                {
                    return _instance;
                }
                GameObject gameObject = new GameObject(typeof(T).Name + " (Singleton");
                _instance = gameObject.AddComponent<T>();
            }
            return _instance;
        }
    }
    
    public static bool Exists =>  instance != null;

    [field: SerializeField] private bool shouldNotDestroyOnLoad = true;

    protected virtual void Awake()
    {
        if (!Application.isPlaying) return;

        foreach (var obj in FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                     .Where(t => t != this))
        {
            Destroy(obj.gameObject);
        }

        if (shouldNotDestroyOnLoad)
        {
            if (!_instance)
            {
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            } else if (this != _instance)
            {
                Destroy(this);
            }
        }
        else
        {
            _instance = this as T;
        }
        
    }
}
