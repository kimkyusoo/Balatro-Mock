using System.Collections.Generic;
using UnityEngine;

public static class JokerCalculator
{

    public static void AddTwicePictureRank(JokerCard joker, List<PlayCard> selectCard)
    {
        if (selectCard == null) return;
        //Debug.Log($"그림카드 두번 동작확인");
        foreach (var card in selectCard)
        {
            if (card.rank > 10)
            {
                joker.addChip += card.rank;
            }        
        }
        //Debug.Log($"그림카드 두번 동작, 얻게된 총 Chip : {joker.addChip}");
    }

    public static void AddMultCaseRanking(JokerCard joker, List<PlayCard> selectCard, HandRanking ranking)
    {
        if (selectCard == null) return;

        if (ranking == HandRanking.None) return;

        //Debug.Log($"랭크별 보너스 동작확인");
        switch (ranking)
        {
            case HandRanking.HighCard:
                joker.addMult += 0; break;
            case HandRanking.OnePair:
                joker.addMult += 1; break;
            case HandRanking.TwoPair:
                joker.addMult += 3; break;
            case HandRanking.Triple:
                joker.addMult += 4; break;
            case HandRanking.Straight:
                joker.addMult += 7; break;
            case HandRanking.Flush:
                joker.addMult += 7; break;
            case HandRanking.FullHouse:
                joker.addMult += 9; break;
            case HandRanking.FourCard:
                joker.addMult += 9; break;
            case HandRanking.StraightFlush:
                joker.addMult += + 15; break;
        }
        //Debug.Log($"랭크별 보너스 동작확인, 얻게된 총 Mult : {joker.addMult}");
    }

    public static void UseUnderThree(JokerCard joker, List<PlayCard> selectCard, Hand hand)
    {
        if (selectCard == null) return;

        if (selectCard.Count > 3 || hand.selectCardList.Count > 3) return;

        joker.addMult += 10;

        //Debug.Log($"3장이하 사용 동작 확인, 얻게된 총 Mult : {joker.addMult}");
    }

    public static void AddSpecificSuit(JokerCard joker, List<PlayCard> selectCard, Suit suit)
    {
        if (selectCard == null) return;

        foreach(PlayCard card in selectCard)
        {
            if(card.suit == suit)
            {
                joker.addChip += 50;
                //Debug.Log($"같은 문양 동작확인");
            }
        }
        //Debug.Log($"같은 문양 동작확인, 얻게된 칩: {joker.addChip}");

    }

    public static void BuildMult(JokerCard joker, List<PlayCard> selectCard, Hand hand, int count)
    {
        if (selectCard == null) return;

        if (hand.selectCardList.Count > 4) return;

        //Debug.Log($"4장 이하 동작 확인");

        joker.addMult = 1 * count;

        joker.executeCount++;

        //Debug.Log($"4장 이하 동작 확인, 얻게된 배수: {joker.addMult}");
    }
}
