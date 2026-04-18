using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("Ref Object, Component")]
    public Deck deck;
    private CardCalculator calculator;
    public HandEvaluator handEvaluator;

    [Header("Hand Information")]
    public PlayCard[] hands = new PlayCard[8];
    public List<PlayCard> selectCardList = new List<PlayCard>();

    [SerializeField] private AudioClip selectSound;



    private void Awake()
    {
        selectCardList.Clear();

        if(calculator == null) calculator = new CardCalculator();
        if(handEvaluator == null) handEvaluator = new HandEvaluator();
        handEvaluator.cardCalculator = calculator;
    }

    private void Start()
    {
        DrawHands();
    }

    public void DrawHands()
    {
        // TODO :: 카드 드로우하는 애니메이션 필요
        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i] == null)
            {
                PlayCard drawnCard = deck.DrawCardFromDeck();

                if (drawnCard != null)
                {
                    hands[i] = drawnCard;

                    drawnCard.transform.SetParent(this.transform);
                    drawnCard.gameObject.SetActive(true);

                }
            }
        }
        SortCardToRank();
    }

    public void SelectCard(PlayCard card)
    {
        //Debug.Log($"Selected Card - Rank: {card.rank}, Suit: {card.suit}");
        //TODO :: 카드 선택 및 취소시마다 UI에서 카드가 올라갔다 내려갔다 하는 애니메이션 필요.
        if (selectCardList.Count > 5) return;
        
        if(selectCardList.Count == 5 && !selectCardList.Contains(card)) return;


        if (selectCardList.Contains(card))
        {
            selectCardList.Remove(card);
        }
        else
        {
            if(selectCardList.Count < 5)
            {
                selectCardList.Add(card);
            }
        }
        SoundManager.Instance.PlaySfxOneShot(selectSound, 0.5f);
        handEvaluator.CalculatePairCount(selectCardList);
    }

    public void ThrowAwayCard()
    {
        if (!RoundManager.Instance.ConsumeDiscardCount()) return;

        if (selectCardList == null) return;

        RoundManager.Instance.UpdateRecord("Discarded", selectCardList.Count);
        ExecuteCardRemoveAndDraw();
    }
    
    public void CalculateCard()
    {
        if (selectCardList == null) return;

        calculator.CalculateScore(handEvaluator);
        RoundManager.Instance.UpdateRecord("Played", selectCardList.Count);
        ExecuteCardRemoveAndDraw();
    }

    private void ExecuteCardRemoveAndDraw()
    {
        if (selectCardList == null)
        {
            return;
        }

        foreach (PlayCard card in selectCardList)
        {
            deck.discardPack.Add(card);

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] == card)
                {
                    hands[i] = null;
                    break;
                }
            }

            card.gameObject.SetActive(false);
        }
        selectCardList.Clear();
        DrawHands();
    }

    private void SortCardToRank()
    {
        hands = hands.OrderBy(card => card == null)           
             .ThenByDescending(card => card != null ? card.rank : -1)
             .ThenBy(card => card != null ? card.suit :Suit.None)
             .ToArray();

        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i] != null)
            {
                hands[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
