using UnityEngine;

[CreateAssetMenu(fileName = "IconsSO", menuName = "Game icons/Icons SO", order = 1)]
public class IconsSO : ScriptableObject
{
    [SerializeField] private Sprite iconImage;
    [TextArea]
    [SerializeField] private string description;

    public void SetExampleIcon(ExampleIcons exampleIcon)
    {
        exampleIcon.SetImage(iconImage);
    }

    public void SetDetails(ExampleManager manager)
    {
        manager.SetDetails(iconImage, description);
    }
}
