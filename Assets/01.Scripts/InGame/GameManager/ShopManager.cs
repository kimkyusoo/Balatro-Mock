using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject jokerPrefab;
    public GameObject planetPrefab;
    public GameObject voucherPrefab;

    public Transform jokerParent;
    public Transform planetParent;
    public Transform voucherParent;

    [Header("Shop Data")]
    public List<JokerCard> remainJokerList = new List<JokerCard>();
    public List<PlanetCard> remainPlanetList = new List<PlanetCard>();
    public List<VoucherCard> remainVoucherList = new List<VoucherCard>();
    public JokerCard selectJoker;
    public PlanetCard selectPlanet;
    public VoucherCard selectVoucher;
    public VoucherCard currentVoucher;
    private int lastGeneratedRound = -1;
    public int rerollCost;
    public int rerollCount;

    [Header("UI Elements")]
    public ShopUI shopUI;

    public Sprite[] jokerSprites;
    public Sprite[] planetSprites;
    public Sprite[] voucherSprites;

    private void Awake()
    {
        SetUpShopData();
    }

    void Start()
    {
        CreateJokerCard(8);
        CreatePlanetCard(9);
        CreateVoucherCard(5);
        ShowItemList();

    }

    private void SetUpShopData()
    {
        remainJokerList.Clear();
        remainPlanetList.Clear();
        remainVoucherList.Clear();

        selectJoker = null;
        selectPlanet = null;
        selectVoucher = null;

        rerollCost = 3;
        rerollCount = 0;
    }

    private void CreateJokerCard(int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject joker = Instantiate(jokerPrefab, jokerParent);
            JokerCard card = joker.GetComponent<JokerCard>();

            card.SetUpCard(i, "Common", 0, 0, Random.Range(1, 4), (JokerEffectType)(i + 1));

            card.SetCardState(false);

            if (jokerSprites != null && i < jokerSprites.Length)
            {
                card.SetSprite(jokerSprites[i]);
            }

            remainJokerList.Add(card);
        }
    }

    private void CreatePlanetCard(int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject planet = Instantiate(planetPrefab, planetParent);
            PlanetCard card = planet.GetComponent<PlanetCard>();

            string id = i.ToString();
            card.SetupCard(id, 3, i + 1);

            card.SetCardState(false);

            if (planetSprites != null && i < planetSprites.Length)
            {
                card.SetSprite(planetSprites[i]);
            }
            remainPlanetList.Add(card);
        }
    }

    private void CreateVoucherCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject voucher = Instantiate(voucherPrefab, voucherParent);
            VoucherCard card = voucher.GetComponent<VoucherCard>();

            string id = i.ToString();
            card.SetupCard(id, 5, i + 1);

            if (voucherSprites != null && i < voucherSprites.Length)
            {
                card.SetSprite(voucherSprites[i]);
            }
            remainVoucherList.Add(card);
        }
    }

    public void ShowItemList()
    {
        ShowJokerCards();
        ShowPlanetCards();
        ShowVoucherCard();
    }

    // 1. 조커 카드리스트를 가져온다.
    // 2. 조커 슬롯의 존재하는 조커카드와 조커 카드리스트에 있는 조커카드를 비교. 존재하면 조커 카드리스크에서 제거.(조커 카드 판매시 다시 리스트에 들어오도록 하는 부분은 보류)
    // 3. 남은 카드리스트에서 랜덤으로 2장 조커카드를 추출
    // 4. 추출한 조커카드를 UI에 표시

    // 상황: 동일한 카드가 상점에 계속 나온다
    // 원인 분석: HashSet<JokerCard> 로 진행하였을때 문제 -> 객체참조 비교 방식의 오류. JokerCard로 진행할 경우 메모리 주소를 비교하게 된다.
    // JokerSlot에 특정 카드가 있어도 상점에 진입시 새로운 8장의 카드가 생성되고 객체참조가 다르기 때문에 다른것으로 판정됨.
    // 해결: JokerEffectType을 기준으로 비교하여 제거하는 방식으로 진행하여 문제 해결
    public void ShowJokerCards()
    {

        if (JokerSlot.Instance == null) return;
        if (JokerSlot.Instance.hasJokerCards == null) return;

        HashSet<JokerEffectType> hasJoker = new HashSet<JokerEffectType>();
        foreach (var joker in JokerSlot.Instance.hasJokerCards)
        {
            if (joker != null) hasJoker.Add(joker.effectType);
        }

        for (int i = 0; i < remainJokerList.Count; i++)
        {
            if (hasJoker.Contains(remainJokerList[i].effectType))
                remainJokerList[i].gameObject.SetActive(false);
            else
                remainJokerList[i].gameObject.SetActive(true);
        }
        ShuffleAndDrawJokerCards();
    }

    public void ShowPlanetCards()
    {
        if (ConsumableSlot.Instance == null) return;
        if (ConsumableSlot.Instance.hasPlanetCards == null) return;

        HashSet<HandRanking> hasPlanet = new HashSet<HandRanking>();
        foreach (PlanetCard planet in ConsumableSlot.Instance.hasPlanetCards)
        {
            if (planet != null) hasPlanet.Add(planet.enforceTarget);
        }

        for (int i = 0; i< remainPlanetList.Count; i++)
        {
            if (hasPlanet.Contains(remainPlanetList[i].enforceTarget))
                remainPlanetList[i].gameObject.SetActive(false);
            else
                remainPlanetList[i].gameObject.SetActive(true);
        }
        ShuffleAndDrawPlanetCards();
    }

    public void ShowVoucherCard()
    {
        if (RoundManager.Instance == null) return;
        int currentRound = (RoundManager.Instance.gameRound - 1) / 3;

        if (currentRound > lastGeneratedRound || currentVoucher == null)
        {
            lastGeneratedRound = currentRound;
            RefreshVoucherCard();
            ShuffleAndDrawVoucherCards();
        }
        else
        {
            if(currentVoucher != null)
            {
                List<VoucherCard> keepVoucherList = new List<VoucherCard> { currentVoucher };
                shopUI.RefreshVoucherDisplay(keepVoucherList);
            }
        }


        //for (int i = 0; i < remainVoucherList.Count; i++)
        //{
        //    if (usedVoucher.Contains(remainVoucherList[i].voucherEffect))
        //        remainVoucherList[i].gameObject.SetActive(false);
        //    else
        //        remainVoucherList[i].gameObject.SetActive(true);
        //}
        //ShuffleAndDrawVoucherCards();
    }

    public void RefreshVoucherCard()
    {
        if (RoundManager.Instance == null) return;
        List<VoucherEffect> usedEffects = RoundManager.Instance.voucherEffects;

        for (int i = remainVoucherList.Count - 1; i >= 0; i--)
        {
            if (usedEffects.Contains(remainVoucherList[i].voucherEffect))
            {
                Destroy(remainVoucherList[i].gameObject);
                remainVoucherList.RemoveAt(i);
            }
        }
    }

    public void ShuffleAndDrawJokerCards()
    {
        List<JokerCard> availableJoker = remainJokerList.FindAll(joker => joker.gameObject.activeSelf);

        if (availableJoker.Count < 1) return;

        List<JokerCard> showingJokerList = new List<JokerCard>();
        List<JokerCard> tempList = new List<JokerCard>(availableJoker);

        int drawCount = Mathf.Min(2, tempList.Count);

        for (int i = 0; i < drawCount; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            showingJokerList.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        shopUI.RefreshJokerDisplay(showingJokerList);
    }

    public void ShuffleAndDrawPlanetCards()
    {
        List<PlanetCard> availablePlanet = remainPlanetList.FindAll(planet => planet.gameObject.activeSelf);

        if (availablePlanet.Count < 1) return;

        List<PlanetCard> showingPlanetList = new List<PlanetCard>();
        List<PlanetCard> tempList = new List<PlanetCard>(availablePlanet);

        int drawCount = Mathf.Min(2, tempList.Count);

        for (int i = 0; i < drawCount; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            showingPlanetList.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        shopUI.RefreshPlanetDisplay(showingPlanetList);
    }

    public void ShuffleAndDrawVoucherCards()
    {
        if (remainVoucherList.Count < 1) return;

        List<VoucherCard> availableVoucherList = remainVoucherList.FindAll(voucher => voucher.gameObject != null);

        int randomIndex = Random.Range(0, availableVoucherList.Count);
        currentVoucher = availableVoucherList[randomIndex];

        List<VoucherCard> showingVoucherList = new List<VoucherCard> { currentVoucher };
        shopUI.RefreshVoucherDisplay(showingVoucherList);
    }

    public void SelectJokerProduct(JokerCard selectCard)
    {
        if (selectJoker != null && selectJoker != selectCard)
        {
            selectJoker.VisualSelect(false);
            selectJoker.SetButtonActive(false);
        }
        selectJoker = (selectJoker == selectCard) ? null : selectCard;

        bool isSelected = (selectJoker != null);
        selectCard.VisualSelect(isSelected);
        selectCard.SetButtonActive(isSelected);
    }

    public void SelectPlanetProduct(PlanetCard selectCard)
    {
        if (selectPlanet != null && selectPlanet != selectCard)
        {
            selectPlanet.VisualSelect(false);
            selectPlanet.SetButtonActive(false);
        }
        selectPlanet = (selectPlanet == selectCard) ? null : selectCard;

        bool isSelected = (selectPlanet != null);
        selectCard.VisualSelect(isSelected);
        selectCard.SetButtonActive(isSelected);
    }

    public void SelectVoucherProduct(VoucherCard voucherCard)
    {
        if (selectVoucher != null && selectVoucher == voucherCard)
        {
            selectVoucher.VisualSelect(false);
            selectVoucher.SetButtonActive(false);
            selectVoucher = null;
        }
        else
        {
            selectVoucher = voucherCard;
            selectVoucher.VisualSelect(true);
            selectVoucher.SetButtonActive(true);
        }
    }

    public void BuyJokerProduct()
    {
        if (selectJoker == null) return;

        if (!RoundManager.Instance.CheckPlayerCoin(selectJoker.sellPrice)) return;

        if(JokerSlot.Instance.hasJokerCount >= 5) return;

        RoundManager.Instance.ConsumePlayerCoin(selectJoker.sellPrice);
        RoundManager.Instance.UpdateRecord("Purchased", selectJoker.sellPrice);

        if(remainJokerList.Contains(selectJoker)) remainJokerList.Remove(selectJoker);

        selectJoker.VisualSelect(false);
        selectJoker.SetButtonActive(false);

        if (JokerSlot.Instance != null)
        {
            JokerSlot.Instance.AddJoker(selectJoker);
        }

        selectJoker = null;
    }

    public void BuyPlanetProduct()
    {
        if (selectPlanet == null) return;

        if (!RoundManager.Instance.CheckPlayerCoin(selectPlanet.sellPrice)) return;

        if (ConsumableSlot.Instance.hasPlanetCardCount >= 2) return;

        RoundManager.Instance.ConsumePlayerCoin(selectPlanet.sellPrice);

        if (remainPlanetList.Contains(selectPlanet)) remainPlanetList.Remove(selectPlanet);

        selectPlanet.VisualSelect(false);
        //selectPlanet.SetCardState(true);
        selectPlanet.SetButtonActive(false);

        if (ConsumableSlot.Instance != null)
        {
            ConsumableSlot.Instance.PlacePlanetCard(selectPlanet);
        }

        selectPlanet = null;
    }

    public void BuyVoucherProduct()
    {
        if (selectVoucher == null) return;
        if (!RoundManager.Instance.CheckPlayerCoin(selectVoucher.sellPrice)) return;

        VoucherCard boughtVoucher = selectVoucher;

        boughtVoucher.PunchVoucher(() => {
            RoundManager.Instance.ConsumePlayerCoin(boughtVoucher.sellPrice);
            boughtVoucher.UseVoucheerEffect(boughtVoucher.voucherEffect);

            if (remainVoucherList.Contains(boughtVoucher))
            {
                remainVoucherList.Remove(boughtVoucher);
            }

            boughtVoucher.gameObject.SetActive(false);
            boughtVoucher.transform.SetParent(null);

            currentVoucher = null;
            selectVoucher = null;

            shopUI.UpdateShopInfo();
        });
    }

    public void RerollProduct()
    {
        if (rerollCost == 0) return;

        rerollCost += rerollCount;
        if (!RoundManager.Instance.CheckPlayerCoin(rerollCost)) return;

        RoundManager.Instance.ConsumePlayerCoin(rerollCost);
        rerollCount++;
        RoundManager.Instance.UpdateRecord("Rerolled", rerollCount);
        shopUI.UpdateShopInfo();


        if (selectJoker != null) selectJoker.VisualSelect(false);
        selectJoker = null;

        ShowJokerCards();
    }

    public void SellProduct()
    {
        if (selectJoker == null) return;
        if(selectJoker.sellPrice == 0) return;
        if (JokerSlot.Instance.hasJokerCount <= 0) return;

        RoundManager.Instance.SavePlayerCoin(selectJoker.sellPrice);

        if (!remainJokerList.Contains(selectJoker)) remainJokerList.Add(selectJoker);

        if (JokerSlot.Instance != null)
        {
            JokerSlot.Instance.RemoveJoker(selectJoker);
            selectJoker.transform.SetParent(jokerParent); 
            selectJoker.gameObject.SetActive(false);      
        }

        selectJoker.SetButtonActive(true);

        if (JokerSlot.Instance != null)
        {
            JokerSlot.Instance.RemoveJoker(selectJoker);
        }

        selectJoker = null;
        shopUI.UpdateShopInfo();
    }
}
