using UnityEngine;

public class GoToNewLevel : MonoBehaviour
{
    [SerializeField] private string newLevel;

    public void LoadLevel()
    {
        GameLevelManager.LoadLevel(newLevel);
    }
}
