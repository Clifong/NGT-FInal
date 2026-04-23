using UnityEngine;
using UnityEngine.UI;

public class ExampleIcons : MonoBehaviour
{
    [SerializeField] private IconsSO _iconsSO;
    [SerializeField] private Image _iconImage;
    [SerializeField] private ExampleManager _exampleManager;

    public void SetImage(Sprite iconImage)
    {
        _iconImage.sprite = iconImage;
    }

    public void SetIconSO(ExampleManager exampleManager, IconsSO iconsSo)
    {
        _iconsSO = iconsSo;
        _exampleManager = exampleManager;
    }

    public void SetIconDetail()
    {
        _iconsSO.SetDetails(_exampleManager);
    }
}
