using System;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [Header("Reward Texts")]
    public TextMeshProUGUI roundWinText;
    public TextMeshProUGUI handBonusText;
    public TextMeshProUGUI discardBonusText;
    public TextMeshProUGUI interestText;
    public TextMeshProUGUI totalEarnedText;

    public Transform slotUIParent;

    public AudioClip resultMusic;

    void Start()
    {
        if (JokerSlot.Instance != null)
        {
            JokerSlot.Instance.SetupSlotPosition(slotUIParent);
        }

        if (SoundManager.Instance != null && resultMusic != null)
        {
            SoundManager.Instance.PlayBgm(resultMusic, 0.5f, false);
        }

        DisplayRewards();
    }

    void DisplayRewards()
    {
        int roundReward = RoundManager.Instance.gameRound / 3 + 1;
        int handReward = Math.Max(0, RoundManager.Instance.remainHand - 3);
        int discardReward = Math.Max(0, RoundManager.Instance.remainDiscard - 3);
        int interestReward = RoundManager.Instance.playerCoin / 10;
        int total = roundReward + handReward + interestReward;

        roundWinText.text = $"+${roundReward}";
        handBonusText.text = $"+${handReward}";
        discardBonusText.text = $"+${discardReward}";
        interestText.text = $"+${interestReward}";
        totalEarnedText.text = $"{total}";
    }
}