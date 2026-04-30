using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PlanetCard : BaseCard, IPointerEnterHandler, IPointerExitHandler
{
    [Header("PlanetCard Information")]
    public float addMult;
    public int addChip;
    public int sellPrice;
    public HandRanking enforceTarget;

    [Header("UI")]
    private bool isSelected = false;
    public RectTransform visualRoot;
    public GameObject buyButtonUI;
    public GameObject useButtonUI;
    public Image planetImage;

    [System.Serializable]
    public struct CardUIObjects
    {
        public GameObject cardRoot;
        public TextMeshProUGUI priceText;
        public Button actionButton;
    }

    public CardUIObjects shopUI;
    public CardUIObjects ownedUI;

    [SerializeField] private AudioClip enforceSound;

    public bool IsOwned { get; private set; }

    public string[] planetName =
    {
        "",
        "하이 카드 UP",
        "원 페어 UP",
        "투 페어 UP",
        "트리플 UP",
        "스트레이트 UP",
        "플러시 UP",
        "풀하우스 UP",
        "포카드 UP",
        "스티플 UP"
    };

    public TextMeshProUGUI planetNameText;

    public static Dictionary<HandRanking, int> targetRankLevels = new Dictionary<HandRanking, int>();

    public string[] enforceDescriptions =
    {
        "",
        "하이 카드를 강화합니다.",
        "원 페어를 강화합니다.",
        "투 페어를 강화합니다.",
        "트리플을 강화합니다.",
        "스트레이트를 강화합니다.",
        "플러시를 강화합니다.",
        "풀하우스를 강화합니다.",
        "포카드를 강화합니다.",
        "스트레이트 플러시를 강화합니다."
    };

    private void Awake()
    {
        if (buyButtonUI != null)
        {
            Button buyButton = buyButtonUI.GetComponent<Button>();
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    ShopManager manager = FindFirstObjectByType<ShopManager>();
                    if (manager != null) manager.BuyPlanetProduct();
                });
            }
        }

        if (useButtonUI != null)
        {
            Button useButton = useButtonUI.GetComponent<Button>();
            if (useButton != null)
            {
                useButton.onClick.AddListener(() =>
                {
                    UsePlanetCard();
                });
            }
        }
    }

    public static void SetupBaseRankLevel()
    {
        targetRankLevels.Clear();
        for(int i = 1; i <= 9; i++)
        {
            HandRanking rank = (HandRanking)i;
            targetRankLevels.Add(rank, 0);
        }
    }

    public void SetupCard(string id, int sellPrice, int targetRankIndex)
    {
        
        Initalize(id, sellPrice, targetRankIndex);
        gameObject.SetActive(false);
    }

    public void Initalize(string id, int sellPrice, int targetRankIndex)
    {
        if (targetRankIndex == 0) return;

        string name = (targetRankIndex < planetName.Length) ? planetName[targetRankIndex] : "";
        string description = (targetRankIndex < enforceDescriptions.Length) ? enforceDescriptions[targetRankIndex] : "";

        base.Initalize(id, name, description);

        this.sellPrice = sellPrice;
        this.enforceTarget = (HandRanking)targetRankIndex;

        if (planetNameText != null)
        {
            planetNameText.text = (targetRankIndex < planetName.Length) ? planetName[targetRankIndex] : "";
        }

        SetRankStat(this.enforceTarget, out this.addChip, out this.addMult);
    }

    public void UsePlanetCard()
    {
        int enforceChip;
        float enforceMult;
        //Debug.Log("UsePlanetCard 진입");
        if (targetRankLevels.ContainsKey(enforceTarget))
        {

            targetRankLevels[enforceTarget]++;
            
            SetRankStat(enforceTarget, out enforceChip, out enforceMult);

            PlayLevelUp(enforceTarget, enforceChip, enforceMult);
            //Debug.Log($"UsePlanetCard targetRankLevels: {targetRankLevels[enforceTarget]}");
            ConsumableSlot.Instance.RemovePlanetCard(this);

            ShopManager shopManager = Object.FindFirstObjectByType<ShopManager>();
            if (shopManager != null)
            {
                if (!shopManager.remainPlanetList.Contains(this))
                {
                    shopManager.remainPlanetList.Add(this);
                }
                transform.SetParent(shopManager.planetParent);
            }

            gameObject.SetActive(false);
        }
    }

    public static void ApplyEnforceHandRanking(HandRanking ranking, CardCalculator cardCalculator)
    {
        if (targetRankLevels.Count == 0) SetupBaseRankLevel();

        int enforceCount = targetRankLevels[ranking];

        if (enforceCount <= 0) return; // 아직 행성카드 미사용상태일 경우

        SetRankStat(ranking, out int chip, out float mult);

        cardCalculator.chip += chip * enforceCount; 
        cardCalculator.mult += mult * (float)enforceCount;
    }

    private static void SetRankStat(HandRanking ranking, out int chip, out float mult)
    {
        chip = 0;
        mult = 0;
        switch (ranking)
        {
            case HandRanking.HighCard: chip = 5; mult = 1; break;
            case HandRanking.OnePair: chip = 5; mult = 1; break;
            case HandRanking.TwoPair: chip = 10; mult = 1; break;
            case HandRanking.Triple: chip = 15; mult = 2; break;
            case HandRanking.Straight: chip = 25; mult = 2; break;
            case HandRanking.Flush: chip = 25; mult = 2; break;
            case HandRanking.FullHouse: chip = 45; mult = 2; break;
            case HandRanking.FourCard: chip = 65; mult = 3; break;
            case HandRanking.StraightFlush: chip = 85; mult = 3; break;
        }
        int level = targetRankLevels[ranking];

        chip *= level;
        mult *= level;
    }

    public void OnClickCard()
    {
        ShopManager shopManager = Object.FindFirstObjectByType<ShopManager>();
        if (shopManager != null)
        {
            shopManager.SelectPlanetProduct(this);
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
            if (shopUI.cardRoot != null) shopUI.cardRoot.SetActive(false);
            if (ownedUI.cardRoot != null) ownedUI.cardRoot.SetActive(false);
        }
    }

    public void SetCardState(bool owned)
    {
        IsOwned = owned;

        if (shopUI.cardRoot != null) shopUI.cardRoot.SetActive(false);
        if (ownedUI.cardRoot != null) ownedUI.cardRoot.SetActive(false);

        if (!IsOwned)
        {
            if(shopUI.priceText != null){
                shopUI.priceText.text = $"${sellPrice}";
            }
        }

    }

    public void SetButtonActive(bool active)
    {
        if (buyButtonUI != null)
        {
            buyButtonUI.SetActive(active);
        }
    }
    public void SetSprite(Sprite sprite)
    {
        planetImage.sprite = sprite;
    }

    public void PlayLevelUp(HandRanking ranking, int chip, float mult)
    {
        float punchDuration = 0.35f;

        GameObject scoreUI = GameObject.Find("ScoreUI");
        if (scoreUI == null) return;

        Transform rankObj = scoreUI.transform.Find("CardRankingUI");
        Transform chipObj = scoreUI.transform.Find("ChipUI");
        Transform multObj = scoreUI.transform.Find("MultUI");

        if (rankObj == null || chipObj == null || multObj == null) return;

        rankObj.localScale = Vector3.one; rankObj.localRotation = Quaternion.identity;
        chipObj.localScale = Vector3.one; chipObj.localRotation = Quaternion.identity;
        multObj.localScale = Vector3.one; multObj.localRotation = Quaternion.identity;

        TextMeshProUGUI rankTxt = rankObj.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI chipTxt = chipObj.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI multTxt = multObj.GetComponentInChildren<TextMeshProUGUI>();

        if (rankTxt != null) rankTxt.alpha = 1f; 
        if (chipTxt != null) chipTxt.alpha = 1f;
        if (multTxt != null) multTxt.alpha = 1f;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => { if (rankTxt != null) rankTxt.text = $"{ranking.ToString()} +lv"; PlaySound();}); 
        sequence.Append(rankObj.DOPunchScale(new Vector3(0.4f, 0.4f, 0), punchDuration, 12, 0.5f));
        sequence.Join(rankObj.DOPunchRotation(new Vector3(0, 0, 5f), punchDuration, 12, 0.5f));

        sequence.AppendInterval(-0.05f); 
        sequence.AppendCallback(() => { if (chipTxt != null) chipTxt.text = $"+{chip}"; PlaySound();});
        sequence.Append(chipObj.DOPunchScale(new Vector3(0.5f, 0.5f, 0), punchDuration, 12, 0.5f));
        sequence.Join(chipObj.DOPunchRotation(new Vector3(0, 0, -5f), punchDuration, 12, 0.5f));
        
        sequence.AppendInterval(-0.05f);
        sequence.AppendCallback(() => { if (multTxt != null) multTxt.text = $"+{mult}"; PlaySound();});
        sequence.Append(multObj.DOPunchScale(new Vector3(0.5f, 0.5f, 0), punchDuration, 12, 0.5f));
        sequence.Join(multObj.DOPunchRotation(new Vector3(0, 0, 5f), punchDuration, 12, 0.5f));
        
        sequence.AppendInterval(0.1f);
        sequence.Append(rankObj.DOPunchPosition(new Vector3(5f, 5f, 0), 0.4f, 20, 0.5f));
        sequence.Join(chipObj.DOPunchPosition(new Vector3(5f, 5f, 0), 0.4f, 20, 0.5f));
        sequence.Join(multObj.DOPunchPosition(new Vector3(5f, 5f, 0), 0.4f, 20, 0.5f));
        
        sequence.AppendInterval(0.8f);
        sequence.Append(rankTxt.DOFade(0, 0.3f));
        sequence.Join(chipTxt.DOFade(0, 0.3f));
        sequence.Join(multTxt.DOFade(0, 0.3f));
    }

    private void PlaySound()
    {
        if(enforceSound != null)
        {
            SoundManager.Instance.PlaySfxOneShot(enforceSound, 0.4f);
        }
    }
}
