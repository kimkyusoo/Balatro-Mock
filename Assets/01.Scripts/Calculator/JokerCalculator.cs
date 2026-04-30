using System.Collections.Generic;

public static class JokerCalculator
{
    public static bool AddTwicePictureRank(JokerCard joker, List<PlayCard> selectCard)
    {
        if (selectCard == null) return false;

        bool isTriggered = false;
        // Debug.Log($"그림카드 두번 동작확인"); 

        foreach (PlayCard card in selectCard)
        {
            if (card.rank > 10)
            {
                joker.addChip += card.rank;
                isTriggered = true;
            }
        }

        if (isTriggered)
        {
            //Debug.Log($"그림카드 두번 동작확인, 얻게된 총 Chip : {joker.addChip}");
        }

        return isTriggered;
    }

    public static bool AddMultCaseRanking(JokerCard joker, List<PlayCard> selectCard, HandRanking ranking)
    {
        if (selectCard == null || ranking == HandRanking.None) return false;

        // Debug.Log($"랭크별 보너스 동작확인");
        bool isTriggered = true;

        switch (ranking)
        {
            case HandRanking.HighCard:
                joker.addMult += 0; isTriggered = false; break;
            case HandRanking.OnePair: joker.addMult += 1; break;
            case HandRanking.TwoPair: joker.addMult += 1; break;
            case HandRanking.Triple: joker.addMult += 2; break;
            case HandRanking.Straight: joker.addMult += 3; break;
            case HandRanking.Flush: joker.addMult += 3; break;
            case HandRanking.FullHouse: joker.addMult += 4; break;
            case HandRanking.FourCard: joker.addMult += 5; break;
            case HandRanking.StraightFlush: joker.addMult += 7; break;
        }

        if (isTriggered)
        {
            //Debug.Log($"랭크별 보너스 동작확인, 얻게된 총 Mult : {joker.addMult}");
        }

        return isTriggered;
    }

    public static bool UseUnderThree(JokerCard joker, List<PlayCard> selectCard, Hand hand)
    {
        if (selectCard == null) return false;
        if (selectCard.Count > 3 || hand.selectCardList.Count > 3) return false;

        joker.addMult += 10;
        //Debug.Log($"3장이하 사용 동작 확인, 얻게된 총 Mult : {joker.addMult}");
        return true;
    }

    public static bool AddSpecificSuit(JokerCard joker, List<PlayCard> selectCard, Suit suit)
    {
        if (selectCard == null) return false;

        bool isTriggered = false;
        foreach (PlayCard card in selectCard)
        {
            if (card.suit == suit)
            {
                joker.addChip += 50;
                // Debug.Log($"{suit} 문양 동작확인"); // 구체적인 문양 로그
                isTriggered = true;
            }
        }

        if (isTriggered)
        {
            //Debug.Log($"같은 문양 동작확인, 얻게된 총 칩: {joker.addChip}");
        }

        return isTriggered;
    }

    public static bool BuildMult(JokerCard joker, List<PlayCard> selectCard, Hand hand, int count)
    {
        if (selectCard == null) return false;

        if (hand.selectCardList.Count <= 4)
        {
            joker.executeCount++;
        }

        // Debug.Log($"4장 이하 동작 확인");
        joker.addMult = 1 + count;

        //Debug.Log($"4장 이하 동작 확인, 얻게된 배수: {joker.addMult}");
        return true;
    }
}