using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ResultUI : MonoBehaviour
{
    [Header("UI Groups")]
    public CanvasGroup resultCoinTextsGroup; 

    [Header("Desc Part (Labels)")]
    public CanvasGroup[] descriptionGroups; 

    [Header("Coin Part (Values)")]
    public CanvasGroup[] coinGroups;

    public TextMeshProUGUI roundCoinText;
    public TextMeshProUGUI handCoinText;
    public TextMeshProUGUI discardCoinText;
    public TextMeshProUGUI interestCoinText;

    public GameObject cashOutImage;
    public GameObject backgroundImage;
    public TextMeshProUGUI totalCoinText;
    public GameObject goButton;

    public AudioClip countSound;
    public AudioClip totalCoinSound;

    void Start()
    {
        ResetUI();
        StartResultSequence();
    }

    void ResetUI()
    {
        if (backgroundImage != null) backgroundImage.transform.localScale = Vector3.zero;

        if (resultCoinTextsGroup != null) resultCoinTextsGroup.alpha = 0;

        foreach (var cg in descriptionGroups) if (cg != null) cg.alpha = 0;
        foreach (var cg in coinGroups) if (cg != null) cg.alpha = 0;

        if (cashOutImage != null) cashOutImage.transform.localScale = Vector3.zero;
        if (totalCoinText != null)
        {
            totalCoinText.text = "$ 0";
            totalCoinText.GetComponent<CanvasGroup>().alpha = 0;
        }

        if (goButton != null) goButton.SetActive(false);
    }

    void StartResultSequence()
    {
        int roundReward = RoundManager.Instance.gameRound / 3 + 1;
        int handReward = Math.Max(0, RoundManager.Instance.remainHand - 3);
        int discardReward = Math.Max(0, RoundManager.Instance.remainDiscard - 3);
        int interestReward = RoundManager.Instance.playerCoin / 10;
        int totalReward = roundReward + handReward + discardReward + interestReward;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);

        if (backgroundImage != null)
        {
            backgroundImage.transform.SetAsFirstSibling();
            sequence.Append(backgroundImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        }

        sequence.AppendCallback(() => resultCoinTextsGroup.DOFade(1f, 0.2f));

        AddPairToSequence(sequence, descriptionGroups[0], coinGroups[0], roundCoinText, roundReward);
        AddPairToSequence(sequence, descriptionGroups[1], coinGroups[1], handCoinText, handReward);
        AddPairToSequence(sequence, descriptionGroups[2], coinGroups[2], discardCoinText, discardReward);
        AddPairToSequence(sequence, descriptionGroups[3], coinGroups[3], interestCoinText, interestReward);

        sequence.AppendInterval(0.1f);

        sequence.AppendCallback(() => {
            cashOutImage.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            totalCoinText.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
            totalCoinText.transform.localScale = Vector3.one;
        });

        sequence.AppendInterval(0.2f);

        sequence.AppendCallback(() => {
            int displayTotal = 0;
            DOTween.To(() => displayTotal, x => {
                displayTotal = x;
                totalCoinText.text = $" {displayTotal}";
            }, totalReward, 0.8f).SetEase(Ease.OutQuad).OnComplete(() => {
                ApplyJuice(totalCoinText.gameObject, 1.5f);
                if(totalCoinSound != null)
                {
                    SoundManager.Instance.PlaySfxOneShot(totalCoinSound, 0.5f);
                }
                if (goButton != null) goButton.SetActive(true);
            });
        });
    }

    void AddPairToSequence(Sequence sequence, CanvasGroup descCG, CanvasGroup coinCG, TextMeshProUGUI coinText, int coin)
    {
        sequence.AppendCallback(() => {
            coinText.text = $"+${coin}";
            descCG.DOFade(1f, 0.2f);
            coinCG.DOFade(1f, 0.2f);

            ApplyJuice(descCG.gameObject);
            ApplyJuice(coinCG.gameObject);

            if (countSound != null)
            {
                if(coin != 0)
                {
                    SoundManager.Instance.PlaySfxOneShot(countSound, 0.5f);
                }
            }
        });
        sequence.AppendInterval(0.4f);
    }

    void ApplyJuice(GameObject obj, float intensity = 1f)
    {
        obj.transform.DOKill();
        obj.transform.DOPunchScale(Vector3.one * 0.15f * intensity, 0.3f, 10, 1f);
    }
}