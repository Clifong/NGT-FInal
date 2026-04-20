using System;
using SaintsField;
using UnityEngine;

public class RememberChoiceManager : Singleton<RememberChoiceManager>, IDataPersistence
{
    [SerializeField] private RememberChoiceData _rememberChoiceData =  new RememberChoiceData();
    private FileDataHandler<RememberChoiceData>  _fileDataHandler = new FileDataHandler<RememberChoiceData>("remember_choices");

    public void Initialise()
    {
        Action action1 = () => RememberChoiceDatabase.instance.Initialize(() => LoadData());
        ChoiceDatabase.instance.Initialize(action1);
    }
    
    protected override void Awake()
    {
        base.Awake();
        Initialise();
    }
    
    public void LoadData()
    {
        DataPersistenceManager.instance.LoadGame(ref _rememberChoiceData, _fileDataHandler);
    }

    public void SaveData()
    {
        DataPersistenceManager.instance.SaveGame(_rememberChoiceData, _fileDataHandler);
    }

    public void RememberChoice(RememberDialogueChoicesSO rememberSo, ChoiceSO choiceSo)
    {
        _rememberChoiceData.AddRememberDataAndChoice(rememberSo, choiceSo);
    }

    public ChoiceSO GetRememberedChoice(RememberDialogueChoicesSO rememberSo)
    {
        return _rememberChoiceData.GetRememberedChoice(rememberSo);
    }
}
