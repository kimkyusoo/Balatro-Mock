using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject jokerPrefab;
    public Transform jokerParent;

    [Header("Shop Data")]
    public List<JokerCard> remainJokerList = new List<JokerCard>();
    public JokerCard selectJoker = null;
    public int rerollCost;
    public int rerollCount;

    [Header("UI Elements")]
    public ShopUI shopUI;

    public Sprite[] jokerSprites;

    private void Awake()
    {
        SetUpShopData();
    }

    void Start()
    {
        CreateJokerCard(8);
        ShowItemList();

    }

    private void SetUpShopData()
    {
        remainJokerList.Clear();
        selectJoker = null;
        rerollCost = 3;
        rerollCount = 0;
    }

    private void CreateJokerCard(int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject joker = Instantiate(jokerPrefab, jokerParent);
            JokerCard card = joker.GetComponent<JokerCard>();

            card.SetUpCard("Common", 1, 1, Random.Range(1, 4), (JokerEffectType)(i + 1));

            card.SetCardState(false);

            if (jokerSprites != null && i < jokerSprites.Length)
            {
                card.SetSprite(jokerSprites[i]);
            }

            remainJokerList.Add(card);
        }
    }

    public void ShowItemList()
    {
        
        ShowJokerCards();

    }

    public void ShowJokerCards()
    {
        // 1. 조커 카드리스트를 가져온다.
        // 2. 조커 슬롯의 존재하는 조커카드와 조커 카드리스트에 있는 조커카드를 비교. 존재하면 조커 카드리스크에서 제거.(조커 카드 판매시 다시 리스트에 들어오도록 하는 부분은 보류)
        // 3. 남은 카드리스트에서 랜덤으로 2장 조커카드를 추출
        // 4. 추출한 조커카드를 UI에 표시

        if (JokerSlot.Instance == null) return;
        if (JokerSlot.Instance.hasJokerCards == null) return;

        HashSet<JokerCard> hasJoker = new HashSet<JokerCard>();
        foreach (var joker in JokerSlot.Instance.hasJokerCards)
        {
            if (joker != null) hasJoker.Add(joker);
        }

        for (int i = 0; i< remainJokerList.Count; i++)
        {
            if (hasJoker.Contains(remainJokerList[i]))
            {
                remainJokerList.RemoveAt(i);
                i--;
            }
        }
        ShuffleAndDrawJokerCards();
    }

    public void ShuffleAndDrawJokerCards()
    {
        if(remainJokerList.Count < 2) return;

        List<JokerCard> showingJokerList = new List<JokerCard>();
        List<JokerCard> tempList = new List<JokerCard>(remainJokerList);

        int drawCount = Mathf.Min(2, tempList.Count);

        for( int i = 0; i < drawCount; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            showingJokerList.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        shopUI.RefreshJokerDisplay(showingJokerList);
    }

    public void SelectProduct(JokerCard selectCard)
    {
        if (selectJoker != null && selectJoker != selectCard)
        {
            selectJoker.VisualSelect(false);
            selectJoker.SetBuyButtonActive(false);
        }
        selectJoker = (selectJoker == selectCard) ? null : selectCard;

        bool isSelected = (selectJoker != null);
        selectCard.VisualSelect(isSelected);
        selectCard.SetBuyButtonActive(isSelected);
    }

    public void BuyProduct()
    {
        if (selectJoker == null) return;

        if (!RoundManager.Instance.CheckPlayerCoin(selectJoker.sellPrice)) return;

        if(JokerSlot.Instance.hasJokerCount >= 5) return;

        RoundManager.Instance.ConsumePlayerCoin(selectJoker.sellPrice);
        RoundManager.Instance.UpdateRecord("Purchased", selectJoker.sellPrice);
        shopUI.UpdateShopInfo();

        if(remainJokerList.Contains(selectJoker)) remainJokerList.Remove(selectJoker);

        selectJoker.VisualSelect(false);
        selectJoker.SetBuyButtonActive(false);

        if (JokerSlot.Instance != null)
        {
            JokerSlot.Instance.AddJoker(selectJoker);
        }

        selectJoker = null;
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
}
