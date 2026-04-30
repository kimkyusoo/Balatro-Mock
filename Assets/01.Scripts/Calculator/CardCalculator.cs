using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardCalculator
{
    public int chip;
    public float mult;
    public int currentScore;
    public int totalScore = 0;

    public static event Action<int> scoreChanged;
    public static event Action<int, float> chipChanged;

    public void CalculateScoreToSequence(List<PlayCard> selectedCards, HandEvaluator handEvaluator, Vector3 centerPos, Action onCompleted)
    {
        if (handEvaluator == null) return;
        if (!RoundManager.Instance.ConsumeHandCount()) return;

        SetHandBaseScore(handEvaluator.handRanking);
        RoundManager.Instance.CheckPlayeRanking(handEvaluator.handRanking);

        JokerScoreRecord scoreRecord = new JokerScoreRecord(chip, mult);

        Sequence sequence = DOTween.Sequence();

        float spacing = 240f;
        float startX = -(selectedCards.Count - 1) * spacing * 0.5f;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            PlayCard card = selectedCards[i];
            Vector3 targetPos = centerPos + new Vector3(startX + (i * spacing), 0, 0);
            sequence.Join(card.PlayFocusAnimation(targetPos, i * 0.05f));
        }

        sequence.AppendInterval(0.3f);

        // 족보기여 카드별 계산
        foreach (PlayCard card in handEvaluator.scoreCards)
        {
            sequence.AppendCallback(() =>
            {
                card.PlayScoringPunch();


                scoreRecord.totalChip += card.baseChip;
                chip = scoreRecord.totalChip;
                UpdateChipAndMult();
            });
            sequence.AppendInterval(0.25f);
        }

        // 조커 효과 계산
        JokerSlot jokerSlot = UnityEngine.Object.FindAnyObjectByType<JokerSlot>();
        if (jokerSlot != null && jokerSlot.hasJokerCount > 0)
        {
            jokerSlot.AddJokerSequence(sequence, scoreRecord, () => {
                chip = scoreRecord.totalChip;
                mult = scoreRecord.totalMult;
                UpdateChipAndMult();
            });

            sequence.AppendInterval(0.2f);
        }

        // 최종 Score 계산
        sequence.AppendCallback(() =>
        {
            float calculateScore = (float)(scoreRecord.totalChip * scoreRecord.totalMult);
            currentScore = Mathf.RoundToInt(calculateScore);

            RoundManager.Instance.CheckMostScore(currentScore);
            totalScore += currentScore;

            UpdateScoreUI();

            RoundManager.Instance.IsReachedTargetScore();
        });

        sequence.OnComplete(() =>
        {
            onCompleted?.Invoke();
        });
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

        if (cardRanking == HandRanking.None)
        {
            UpdateChipAndMult(); 
            return;
        }
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

        PlanetCard.ApplyEnforceHandRanking(cardRanking, this);
        UpdateChipAndMult();
        //Debug.Log($" SetHandBaseScore: Ranking: {cardRanking}, Chip: {chip}, Mult: {mult}");
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
