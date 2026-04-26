using TMPro;
using UnityEngine;

public class EndingCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI panelText;

    [TextArea]
    [SerializeField] private string textToSet;

    public void SetText()
    {
        panelText.text = textToSet;
    }
}
