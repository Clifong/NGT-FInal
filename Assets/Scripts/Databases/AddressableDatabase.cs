using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public abstract class AddressableDatabase<T> : Singleton<T> where T : AddressableDatabase<T>
{
    [SerializeField] private string label;
    [SerializeField] private SaintsDictionary<string, IDatabaseObject> _mapping = new SaintsDictionary<string, IDatabaseObject>();
    private AsyncOperationHandle<IList<IDatabaseObject>> handle;
    private Action loadDataOperation;
        
    public void Initialize() {}
    
    public void Initialize(Action loadDataOperation)
    {
        this.loadDataOperation = loadDataOperation;
    }
    
    protected override void Awake()
    {
        base.Awake();
        handle = Addressables.LoadAssetsAsync<IDatabaseObject>(label);
        handle.Completed += HandleInstantiation;  
    }
    
    public IDatabaseObject RetrieveScriptableObject(string id)
    {
        _mapping.TryGetValue(id, out IDatabaseObject databaseObject);
        return databaseObject;
    }

    protected void SetLabel(string label)
    {
        this.label = label;
    }
    
    private void HandleInstantiation(AsyncOperationHandle<IList<IDatabaseObject>> task)
    {
        if (task.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var k in task.Result)
            {
                k.AddThingToDatabase(_mapping);   
            }
            loadDataOperation?.Invoke();
        }
        else
        {
            Debug.Log("Missing label probably");
        }
    }
}
