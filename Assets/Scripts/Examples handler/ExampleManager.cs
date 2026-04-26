using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExampleManager : MonoBehaviour
{
    [SerializeField] private List<IconsSO> iconsToShowOnLeft;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private ExampleIcons _exampleIconsPrefab;
    [SerializeField] private Transform content;
    
    [SerializeField] private Image image;

    void Start()
    {
        bool isFirst = true;
        foreach (var iconsSo in iconsToShowOnLeft)
        {
            ExampleIcons icon = Instantiate(_exampleIconsPrefab, content);
            iconsSo.SetExampleIcon(icon);
            icon.SetIconSO(this, iconsSo);
            if (isFirst)
            {
                icon.SetIconDetail();
                isFirst = false;
            }
        }
        
    }

    public void SetDetails(Sprite sprite, string description)
    {
        image.sprite = sprite;
        descriptionText.text = description;
    }
}
