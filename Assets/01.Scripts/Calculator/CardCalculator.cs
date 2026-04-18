using System;
using UnityEngine;

public class CardCalculator
{
    public int chip;
    public float mult;
    public int currentScore;
    public int totalScore = 0;

    public static event Action<int> scoreChanged;
    public static event Action<int, float> chipChanged;

    public void CalculateScore(HandEvaluator handEvaluator)
    {
        //Debug.Log($"CalculateScore, chip: {chip}, mult: {mult}");
        JokerScoreRecord score = new JokerScoreRecord(chip, mult);
        currentScore = 0;
       
        if (handEvaluator == null) {
            Debug.Log("handEvaluator가 없습니다.");
            return;
        }

        if (!RoundManager.Instance.ConsumeHandCount()) return;

        SetHandBaseScore(handEvaluator.handRanking);
        RoundManager.Instance.CheckPlayeRanking(handEvaluator.handRanking);


        CheckCardChip(handEvaluator);

        JokerSlot jokerSlot = UnityEngine.Object.FindAnyObjectByType<JokerSlot>();
        if(jokerSlot != null ) jokerSlot.CalculateJokerEffect(score);


        float calculateScore = (float)((score.totalChip + chip) * score.totalMult);
        //Debug.Log($"CalculateScore, calculateScore : {calculateScore}");
        currentScore = Mathf.RoundToInt(calculateScore);
        RoundManager.Instance.CheckMostScore(currentScore);
        //Debug.Log($"CalculateScore, currentScore : {totalScore}");
        totalScore += currentScore;
        //Debug.Log($"CalculateScore, totalScore : {totalScore}");
        UpdateScoreUI();

        RoundManager.Instance.IsReachedTargetScore();
    }

    public void CheckCardChip(HandEvaluator handEvaluator)
    {
        if (handEvaluator == null)
        {
            Debug.Log("CehckCardChip, handEvaluator가 존재하지 않음");
            return;
        }

        if (handEvaluator.scoreCards == null)
        {
            Debug.Log("CehckCardChip, scoreCard가 존재하지 않음"); 
            return;
        }

        for (int i = 0; i < handEvaluator.scoreCards.Count; i++)
        {
            chip += handEvaluator.scoreCards[i].baseChip;
        }
    }

    public void SetHandBaseScore(HandRanking cardRanking)
    {
        chip = 0;
        mult = 0;

        if (cardRanking == HandRanking.None) return;
        switch(cardRanking){
            case HandRanking.HighCard: chip = chip + 5; mult = 1; break;
            case HandRanking.OnePair: chip = chip + 10; mult = 2; break;
            case HandRanking.TwoPair: chip = chip + 20; mult = 2; break;
            case HandRanking.Triple: chip = chip + 30; mult = 3; break;
            case HandRanking.Straight: chip = chip + 30; mult = 4; break;
            case HandRanking.Flush: chip = chip + 35; mult = 4; break;
            case HandRanking.FullHouse: chip = chip + 40; mult = 4; break;
            case HandRanking.FourCard: chip = chip + 60; mult = 7; break;
            case HandRanking.StraightFlush: chip = chip + 100; mult = 8; break;
        }
        UpdateChipAndMult();
        //Debug.Log($" SetHandBaseScore: Ranking: {cardRanking}, Chip: {chip}, Mult: {mult}");
        // 족보별 애니메이션 처리(chip, mult 세팅)
    }

    private void UpdateScoreUI()
    {
        scoreChanged?.Invoke(totalScore);

    }

    private void UpdateChipAndMult()
    {
        chipChanged?.Invoke(chip, mult);
    }
}
