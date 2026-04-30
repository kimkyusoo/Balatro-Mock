
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseCard : MonoBehaviour
{
    [Header("CardInformation")]
    public string cardId;
    public string cardName;
    [TextArea(1, 3)] public string description;

    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;

    public void Initalize(string cardId, string cardName, string description)
    {
        this.cardId = cardId;
        this.cardName = cardName;
        this.description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionText.text = description;
            descriptionPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}
