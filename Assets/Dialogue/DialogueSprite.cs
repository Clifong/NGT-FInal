using UnityEngine;
using UnityEngine.UI;

public class DialogueSprite : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    public void SetDialogueSprite(Sprite sprite = null)
    {
        _image.sprite = sprite;
    }
}
