using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JokerSlot : MonoBehaviour
{
    public static JokerSlot Instance;

    [Header("JokerSlot")]
    public JokerCard[] hasJokerCards = new JokerCard[5];
    public Queue<JokerCard> jokerEffect = new Queue<JokerCard>();
    public int hasJokerCount;
    public TextMeshProUGUI hasJokerCountText;

    [Header("JokerImage")]
    public Sprite[] jokerSprites;
    public GameObject jokerPrefab;
    public Transform slotPosition;

    public Transform jokerSlotArea;

    private void Awake()
    {
        transform.SetParent(null);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetupSlotPosition(Transform anchor)
    {
        RectTransform myRect = GetComponent<RectTransform>();
        RectTransform anchorRect = anchor as RectTransform;


        if (myRect != null && anchorRect != null)
        {
            myRect.position = anchorRect.position;
            myRect.sizeDelta = anchorRect.sizeDelta;
            myRect.localScale = anchorRect.localScale;
        }

        gameObject.SetActive(true);
    }

    public void AddJoker(JokerCard joker)
    {
        //Debug.Log($"AddJoker, 동작확인");
        // 잘못된 구매
        if (joker == null) return;

        // 조커 슬롯 Full
        if (hasJokerCount == 5) return;

        for (int i = 0; i < hasJokerCards.Length; i++)
        {
            if (hasJokerCards[i] == null)
            {
                hasJokerCards[i] = joker;
                hasJokerCount++;

                hasJokerCards[i].transform.SetParent(this.jokerSlotArea);

                hasJokerCards[i].SetCardState(true);

                hasJokerCards[i].VisualSelect(false);

                hasJokerCards[i].transform.localPosition = Vector3.zero;
                hasJokerCards[i].transform.localScale = Vector3.one;

                break;
            }
        }
        UpdateHasJokerCount();
    }

    public void CalculateJokerEffect(JokerScoreRecord score)
    {
        //Debug.Log($"CalculateJokerEffect, 호출 확인");
        if (hasJokerCount == 0) return;
        jokerEffect.Clear();
        
        FillJokerEffect();
        //Debug.Log($"CalculateJokerEffect, jokerEffect.Count: {jokerEffect.Count}");
        if (jokerEffect.Count <= 0)
        {
            //Debug.Log($"ProcessJokerEffect, jockerEffect Queue Count 0: {jokerEffect.Count}");
            return;
        }

        UseJokerEffect(score);

    }

    private void FillJokerEffect()
    {
        for (int i = 0; i < hasJokerCards.Length; i++)
        {
            if (hasJokerCards[i] != null)
            {
                jokerEffect.Enqueue(hasJokerCards[i]);
            }
        }
    }

    private void UseJokerEffect(JokerScoreRecord score)
    {
        Hand hand = FindFirstObjectByType<Hand>();
        if (hand == null) return;

        HandRanking ranking = hand.handEvaluator.handRanking;
        List<PlayCard> cards = hand.handEvaluator.scoreCards;


        while (jokerEffect.Count > 0)
        {
            JokerCard jokerCard = jokerEffect.Dequeue();
            jokerCard.ProcessJokerEffect(hand, ranking, cards, score);
            //Debug.Log($"UseJokerEffect, ProcessJokerEffect 호출 확인");
            if (jokerCard.effectType != JokerEffectType.BuildMult)
            {
                jokerCard.addChip = 0;
                jokerCard.addMult = 0;
            }
        }
    }

    private void UpdateHasJokerCount()
    {
        if(hasJokerCountText != null)hasJokerCountText.text = $"{hasJokerCount}/5";
    }
}
