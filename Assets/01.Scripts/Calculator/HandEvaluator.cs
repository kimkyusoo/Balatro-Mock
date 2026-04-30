using System;
using System.Collections.Generic;
using UnityEngine;

public class HandEvaluator
{
    //bool hasFourJoker = false;

    public HandRanking handRanking = HandRanking.None;
    public List<PlayCard> scoreCards = new List<PlayCard>();
    public CardCalculator cardCalculator;

    public static event Action<HandRanking> rankingChanged;


    public void CalculatePairCount(List<PlayCard> selectCardList)
    {
        //Debug.Log($"CalculatePairCount, {selectCardList}");
        if (selectCardList == null) 
        { 
            handRanking = HandRanking.None;
            cardCalculator.SetHandBaseScore(handRanking);
            UpdateRankingUI();
            return; 
        }

        scoreCards.Clear();
        int pairCount = 0;
        List<PlayCard> card = selectCardList;
        //Debug.Log($"CalculatePairCount, {card}");

        if (card == null || card.Count == 0)
        {
            handRanking = HandRanking.None;
            return;
        }

        for (int i = 0; i < card.Count; i++)
        {
            for (int j = i + 1; j < card.Count; j++)
            {
                if (card[i].rank == card[j].rank)
                {
                    pairCount++;
                    if (!scoreCards.Contains(card[i])) scoreCards.Add(card[i]);
                    if (!scoreCards.Contains(card[j])) scoreCards.Add(card[j]);

                }
            }
        }
        //Debug.Log($"CalculatePairCount, pairCount: {pairCount}");

        if (pairCount == 0)
        {
            EvaluateNotPairRanking(card);
        }
        else
        {
            EvaluatePairRanking(pairCount);
        }
        UpdateRankingUI();
        cardCalculator.SetHandBaseScore(handRanking);

    }

    public void EvaluateNotPairRanking(List<PlayCard> card)
    {
        scoreCards.Clear();
        card.Sort((a, b) => a.rank.CompareTo(b.rank));
        bool isStraight = false;
        bool isMountain = false;
        bool isFlush = false;

        // Straight, Flush 충족 여부
        if (card.Count == 5)
        {
            isStraight = card[card.Count - 1].rank - card[0].rank == 4 ? true : false;
            isMountain = (card[0].rank == 1 && card[1].rank == 10 && card[4].rank == 13);
            if (isMountain) isStraight = true;
            isFlush = (card[0].suit == card[1].suit && card[0].suit == card[2].suit && card[0].suit == card[3].suit && card[0].suit == card[4].suit) ? true : false;
        }

        if (isStraight)
        {
            // 스티플
            if (isFlush)
            {
                handRanking = HandRanking.StraightFlush;
            }
            else
            {
                handRanking = HandRanking.Straight;
            }
        }
        else if (isFlush)
        {
            handRanking = HandRanking.Flush;

        }
        // 하이카드
        else
        {
            handRanking = HandRanking.HighCard;
            scoreCards.Add(card[card.Count - 1]);
        }


        if (isStraight || isFlush)
        {
            foreach(PlayCard c in card)
            {
                scoreCards.Add(c);
            }
        }
        //Debug.Log($"EvaluateNotPairRanking, handRanking: {handRanking}");

    }

    public void EvaluatePairRanking(int count)
    {
        switch (count)
        {
            case 1: handRanking = HandRanking.OnePair; break;
            case 2: handRanking = HandRanking.TwoPair; break;
            case 3: handRanking = HandRanking.Triple; break;
            case 4: handRanking = HandRanking.FullHouse; break;
            case 6: handRanking = HandRanking.FourCard; break;
        }
        //Debug.Log($"EvaluatePairRanking, handRanking: {handRanking}");

    }

    private void UpdateRankingUI()
    {
        rankingChanged?.Invoke(handRanking);
    }
}
