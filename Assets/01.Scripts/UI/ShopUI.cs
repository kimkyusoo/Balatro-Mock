using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public ShopManager shopManager;

    public Transform jokerProductArea;
    //public Transform[] consumableShelf;

    //[Header("Card Pack Buttons")]
    //public Button tarotPackButton;
    //public Button planetPackButton;

    public TextMeshProUGUI shopCoinText;

    [Header("Control Buttons")]
    public Button rerollButton;
    public Button buyButton;

    [Header("Text Info")]
    public TextMeshProUGUI rerollCostText;

    public Transform slotUIParent;

    public AudioClip shopMusic;

    public void Awake()
    {
        rerollButton.onClick.AddListener(() => shopManager.RerollProduct()); 
    }

    private void Start()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.LinkSimpleUI(shopCoinText);
        }

        if (JokerSlot.Instance != null)
        {
            JokerSlot.Instance.SetupSlotPosition(slotUIParent);
        }

        if (SoundManager.Instance != null && shopMusic != null)
        {
            SoundManager.Instance.PlayBgm(shopMusic, 0.3f);
        }
        UpdateShopInfo();
    }


    public void RefreshJokerDisplay(List<JokerCard> jokerCards)
    {
        ClearJokerSlots();

        foreach (JokerCard card in jokerCards)
        {
            card.gameObject.SetActive(true);
            card.transform.SetParent(jokerProductArea);
            card.transform.localScale = Vector3.one;
            card.transform.localPosition = Vector3.zero;
        }
        UpdateShopInfo();
    }

    private void ClearJokerSlots()
    {
        foreach (Transform child in jokerProductArea)
        {
            child.gameObject.SetActive(false);
            child.SetParent(shopManager.jokerParent);
        }
    }

    public void UpdateShopInfo()
    {
        if (rerollCostText != null) rerollCostText.text = $"$ {shopManager.rerollCost + shopManager.rerollCount}";
    }
}
