using DG.Tweening;
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
    public GameObject sellButtonUI;

    [Header("JokerCard Sprite")]
    public Image jokerImage;

    private bool isSelected = false;
    public TextMeshProUGUI jokerNameText;

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

    public string[] jokerNames =
    {
        "그림 카드 트리거",
        "랭크별 배수",
        "3장 이하 핸드 보너스",
        "스페이드 보너스",
        "다이아몬드 보너스",
        "하트 보너스",
        "클럽 보너스",
        "핸드 크기 배수"
    };

    public string[] jokerDescription =
    {
        "그림 카드가 두 번 발동됩니다.",
        "카드의 숫자(랭크)에 따라 배수(Mult)를 제공합니다.",
        "플레이한 카드가 3장 이하일 경우, 배수(Mult) +10을 제공합니다.",
        "플레이한 핸드에 포함된 스페이드 카드 한 장당 칩 +50을 제공합니다.",
        "플레이한 핸드에 포함된 다이아몬드 카드 한 장당 칩 +50을 제공합니다.",
        "플레이한 핸드에 포함된 하트 카드 한 장당 칩 +50을 제공합니다.",
        "플레이한 핸드에 포함된 클럽 카드 한 장당 칩 +50을 제공합니다.",
        "4장 이하의 카드를 플레이할 때마다 배수(Mult) +1을 얻습니다."
    };

    public bool IsOwned { get; private set; }

    [SerializeField] private AudioClip jokerCalculatorSound;

    void Awake()
    {
        if (buyButtonUI != null)
        {
            Button buyButton = buyButtonUI.GetComponent<Button>();
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    ShopManager manager = FindFirstObjectByType<ShopManager>();
                    if (manager != null) manager.BuyJokerProduct();
                });
            }
        }

        if (sellButtonUI != null)
        {
            Button sellButton = sellButtonUI.GetComponent<Button>();
            if (sellButton != null)
            {
                sellButton.onClick.AddListener(() =>
                {
                    ShopManager manager = FindFirstObjectByType<ShopManager>();
                    if (manager != null) manager.SellProduct();
                });
            }
        }
    }

    public void Initalize(int i, string rarity,  float addmult, int addChip, int sellPrice, JokerEffectType effectType)
    {
        this.rarity = rarity;
        this.addMult = addmult;
        this.addChip = addChip;
        this.sellPrice = sellPrice;
        this.effectType = effectType;

        string generateId = $"JokerCard_{i}";

        string name = (i < jokerNames.Length) ? jokerNames[i] : "";
        string descrition = (i < jokerDescription.Length) ? jokerDescription[i] : "";

        base.Initalize(generateId, name, descrition);

        if (jokerNameText != null)
        {
            jokerNameText.text = name;
        }


        //Debug.Log($"[JokerCard] Rarity: {this.rarity}, AddMult: {this.addMult}, AddChip: {addChip}, SellPrice: {this.sellPrice}, EffectType: {effectType}");
    }

    public void SetSprite(Sprite sprite)
    {
        jokerImage.sprite = sprite;
    }

    public void SetUpCard(int i, string rarity, float addMult, int addChip, int sellPrice, JokerEffectType effectType)
    {
        Initalize(i, rarity, addMult, addChip, sellPrice, effectType);
        gameObject.SetActive(false);
    }

    public void OnClickCard()
    {
        ShopManager shopManager = Object.FindFirstObjectByType<ShopManager>();
        if (shopManager != null)
        {
            shopManager.SelectJokerProduct(this);
        }

    }
    public void VisualSelect(bool select)
    {
        if (isSelected == select) return;

        isSelected = select;

        visualRoot.anchoredPosition += isSelected ? new Vector2(0, 50) : new Vector2(0, -50);

        if (isSelected)
        {
            if (!IsOwned) shopUI.cardRoot.SetActive(true);  
            else ownedUI.cardRoot.SetActive(true);         
        }
        else
        {
            shopUI.cardRoot.SetActive(false);
            ownedUI.cardRoot.SetActive(false);
        }
    }

    public void SetButtonActive(bool active)
    {
        if (buyButtonUI != null)
        {
            buyButtonUI.SetActive(active);
        }
    }
    public void SetCardState(bool owned)
    {
        IsOwned = owned;

        shopUI.cardRoot.SetActive(false);
        ownedUI.cardRoot.SetActive(false);

        shopUI.cardRoot.SetActive(false);
        if (!IsOwned) shopUI.priceText.text = $"${sellPrice}"; 

        ownedUI.cardRoot.SetActive(false);
        if (IsOwned) ownedUI.priceText.text = $"${sellPrice}";
    }

    public bool CheckAndCalculate(out int bonusChip, out float bonusMult)
    {
        bonusChip = 0;
        bonusMult = 0;

        this.addChip = 0;
        this.addMult = 0;

        Hand hand = FindFirstObjectByType<Hand>();
        if (hand == null || hand.handEvaluator == null) return false;

        HandRanking ranking = hand.handEvaluator.handRanking;
        List<PlayCard> scoreCards = hand.handEvaluator.scoreCards;

        switch (effectType)
        {
            case JokerEffectType.AddTwicePictureRank:
                JokerCalculator.AddTwicePictureRank(this, scoreCards); break;
            case JokerEffectType.AddMultCaseRanking:
                JokerCalculator.AddMultCaseRanking(this, scoreCards, ranking); break;
            case JokerEffectType.UseUnderThree:
                JokerCalculator.UseUnderThree(this, scoreCards, hand); break;
            case JokerEffectType.SpadeBonus:
                JokerCalculator.AddSpecificSuit(this, scoreCards, Suit.Spade); break;
            case JokerEffectType.DiamondBonus:
                JokerCalculator.AddSpecificSuit(this, scoreCards, Suit.Diamond); break;
            case JokerEffectType.HeartBonus:
                JokerCalculator.AddSpecificSuit(this, scoreCards, Suit.Heart); break;
            case JokerEffectType.ClubBonus:
                JokerCalculator.AddSpecificSuit(this, scoreCards, Suit.Club); break;
            case JokerEffectType.BuildMult:
                JokerCalculator.BuildMult(this, scoreCards, hand, executeCount); break;
        }

        bonusChip = this.addChip;
        bonusMult = this.addMult;

        return (bonusChip > 0 || bonusMult > 0);
    }

    public void PunchJokerCard()
    {
        if (visualRoot == null) return;

        visualRoot.DOKill();
        visualRoot.localScale = Vector3.one;

        visualRoot.DOPunchPosition(new Vector3(40f, 0, 0), 0.15f, 20, 0.5f);

        if(jokerCalculatorSound != null)
        {
            SoundManager.Instance.PlaySfxOneShot(jokerCalculatorSound, 0.3f);
        }
    }
}
