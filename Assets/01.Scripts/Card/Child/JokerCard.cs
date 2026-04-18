using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JokerCard : BaseCard, IPointerEnterHandler, IPointerExitHandler
{
    [Header("JokerCard Information")]
    public string rarity;
    public float addMult;
    public int addChip;
    public int sellPrice;
    public JokerEffectType effectType;
    public int executeCount;

    public GameObject buyButtonUI;

    [Header("JokerCard Sprite")]
    public Image jokerImage;

    private bool isSelected = false;

    public RectTransform visualRoot;

    [System.Serializable]
    public struct CardUIObjects
    {
        public GameObject cardRoot;         
        public TextMeshProUGUI priceText; 
        public Button actionButton;      
    }

    [Header("UI State Elements")]
    public CardUIObjects shopUI;    
    public CardUIObjects ownedUI;   

    [Header("Tooltip UI")]
    public GameObject descriptionPanel;    
    public TextMeshProUGUI descriptionText;

    public bool isOwned { get; private set; }

    void Awake()
    {
        if (buyButtonUI != null)
        {
            Button buyButton = buyButtonUI.GetComponent<Button>();
            if (buyButton != null)
            {
                // 버튼을 누르면 매니저의 BuyProduct를 실행해라!
                buyButton.onClick.AddListener(() => {
                    ShopManager manager = FindFirstObjectByType<ShopManager>();
                    if (manager != null) manager.BuyProduct();
                });
            }
        }
    }

    public void Initalize(string rarity,  float addmult, int addChip, int sellPrice, JokerEffectType effectType)
    {
        this.rarity = rarity;
        this.addMult = addmult;
        this.addChip = addChip;
        this.sellPrice = sellPrice;
        this.effectType = effectType;

        //Debug.Log($"[JokerCard] Rarity: {this.rarity}, AddMult: {this.addMult}, AddChip: {addChip}, SellPrice: {this.sellPrice}, EffectType: {effectType}");
    }

    public void ProcessJokerEffect(Hand hand, HandRanking ranking, List<PlayCard> card, JokerScoreRecord score)
    {
        switch (effectType) {
            case JokerEffectType.AddTwicePictureRank:
                JokerCalculator.AddTwicePictureRank(this, card); break;
            case JokerEffectType.AddMultCaseRanking:
                JokerCalculator.AddMultCaseRanking(this, card, ranking); break;
            case JokerEffectType.UseUnderThree:
                JokerCalculator.UseUnderThree(this, card, hand); break;
            case JokerEffectType.SpadeBonus:
                JokerCalculator.AddSpecificSuit(this, card, Suit.Spade); break;
            case JokerEffectType.DiamondBonus:
                JokerCalculator.AddSpecificSuit(this, card, Suit.Diamond); break;
            case JokerEffectType.HeartBonus:
                JokerCalculator.AddSpecificSuit(this, card, Suit.Heart); break;
            case JokerEffectType.ClubBonus:
                JokerCalculator.AddSpecificSuit(this, card, Suit.Club); break;
            case JokerEffectType.BuildMult:
                JokerCalculator.BuildMult(this, card, hand, executeCount); break;
        }
        score.totalChip += addChip;
        score.totalMult += addMult;
    }

    public void SetSprite(Sprite sprite)
    {
        jokerImage.sprite = sprite;
    }

    public void SetUpCard(string rarity, float addMult, int addChip, int sellPrice, JokerEffectType effectType)
    {
        Initalize(rarity, addMult, addChip, sellPrice, effectType);
        gameObject.SetActive(false);
    }

    public void OnClickCard()
    {
        ShopManager shopManager = Object.FindFirstObjectByType<ShopManager>();
        if (shopManager != null)
        {
            shopManager.SelectProduct(this);
        }

    }
    public void VisualSelect(bool select)
    {
        if (isSelected == select) return;

        isSelected = select;

        visualRoot.anchoredPosition += isSelected ? new Vector2(0, 50) : new Vector2(0, -50);

        if (isSelected)
        {
            if (!isOwned) shopUI.cardRoot.SetActive(true);  
            else ownedUI.cardRoot.SetActive(true);         
        }
        else
        {
            shopUI.cardRoot.SetActive(false);
            ownedUI.cardRoot.SetActive(false);
        }
    }

    public void SetBuyButtonActive(bool active)
    {
        if (buyButtonUI != null)
        {
            buyButtonUI.SetActive(active);
        }
    }
    public void SetCardState(bool owned)
    {
        isOwned = owned;

        shopUI.cardRoot.SetActive(false);
        ownedUI.cardRoot.SetActive(false);

        shopUI.cardRoot.SetActive(false);
        if (!isOwned) shopUI.priceText.text = $"$ {sellPrice}"; 

        ownedUI.cardRoot.SetActive(false);
        if (isOwned) ownedUI.priceText.text = $"$ {sellPrice}";
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
