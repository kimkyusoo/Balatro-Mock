using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public ShopManager shopManager;

    public Transform jokerProductArea;
    public Transform planetProductArea;
    public Transform voucherProductArea;

    public TextMeshProUGUI shopCoinText;

    [Header("Control Buttons")]
    public Button rerollButton;

    [Header("Text Info")]
    public TextMeshProUGUI rerollCostText;

    public Transform slotUIParent;


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

    public void RefreshPlanetDisplay(List<PlanetCard> planetCards)
    {
        ClearPlanetSlots();

        foreach (PlanetCard card in planetCards)
        {
            card.gameObject.SetActive(true);
            card.transform.SetParent(planetProductArea);
            card.transform.localScale = Vector3.one;
            card.transform.localPosition = Vector3.zero;
        }
    }

    public void RefreshVoucherDisplay(List<VoucherCard> voucherCards)
    {
        ClearVoucherSlots();

        foreach (VoucherCard card in voucherCards)
        {
            card.gameObject.SetActive(true);
            card.transform.SetParent(voucherProductArea);
            card.transform.localScale = Vector3.one;
            card.transform.localPosition = Vector3.zero;
        }
    }

    public void ClearJokerSlots()
    {
        foreach (Transform child in jokerProductArea)
        {
            child.gameObject.SetActive(false);
            child.SetParent(shopManager.jokerParent);
        }
    }

    public void ClearPlanetSlots()
    {
        foreach (Transform child in planetProductArea)
        {
            child.gameObject.SetActive(false);
            child.SetParent(shopManager.planetParent);
        }
    }

    public void ClearVoucherSlots()
    {
        foreach (Transform child in voucherProductArea)
        {
            child.gameObject.SetActive(false);
            child.SetParent(shopManager.voucherParent);
        }
    }

    public void UpdateShopInfo()
    {
        if (rerollCostText != null) rerollCostText.text = $"$ {shopManager.rerollCost + shopManager.rerollCount}";
    }
}
