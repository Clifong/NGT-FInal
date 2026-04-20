using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogDialogueManager : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private TextMeshProUGUI dialogueSentencePrefab;
    private List<string> allSentences = new List<string>();
    private List<TextMeshProUGUI> allSpawnedSentences = new List<TextMeshProUGUI>();
    private int lastSpawnedIndex = 0;

    public void AddSentence(string sentence)
    {
        allSentences.Add(sentence);
    }

    public void Refresh()
    {
        foreach (var spawnedSentence in allSpawnedSentences)
        {
            Destroy(spawnedSentence.gameObject);
        }

        lastSpawnedIndex = 0;
        allSpawnedSentences.Clear();
        allSentences.Clear();
    }

    public void SpawnText()
    {
        for (int i = lastSpawnedIndex; i < allSentences.Count; i++)
        {
            TextMeshProUGUI spawnedText = Instantiate(dialogueSentencePrefab, spawnTransform);
            spawnedText.text = allSentences[i];
            allSpawnedSentences.Add(spawnedText);
        }
        lastSpawnedIndex = allSpawnedSentences.Count;
    }
}



