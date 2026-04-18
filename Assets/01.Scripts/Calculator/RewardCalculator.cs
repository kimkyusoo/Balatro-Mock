using System;

public static class RewardCalculator
{
    public static int CalculateCoin()
    {
        int interestCoin = RoundManager.Instance.playerCoin / 10;
        int roundCoin = RoundManager.Instance.gameRound / 3 + 1;
        int handCoin = Math.Max(0, RoundManager.Instance.remainHand - 3);
        int discardCoin = Math.Max(0, RoundManager.Instance.remainDiscard - 3);
        int earnCoin = interestCoin + roundCoin + handCoin + discardCoin;

        return earnCoin;
    }
}
