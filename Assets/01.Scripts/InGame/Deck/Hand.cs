using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] public Transform deckSpawnPoint;

    [SerializeField] public Transform cardScoreArea;

    [Header("Button Information")]
    public Button playButton;
    public Button discardButton;
    private ColorBlock activeBlock;
    private ColorBlock inactiveBlock;



    private void Awake()
    {
        selectCardList.Clear();

        if(calculator == null) calculator = new CardCalculator();
        if(handEvaluator == null) handEvaluator = new HandEvaluator();
        handEvaluator.cardCalculator = calculator;

        SetupButtonColors();
    }

    private void Start()
    {
        UpdateButtonState();

        DG.Tweening.DOVirtual.DelayedCall(1.5f, () =>
        {
            DrawHands();
        });
    }

    public void DrawHands()
    {
        float currentDelay = 0f;
        float delayStep = 0.12f;

        List<PlayCard> newCards = new List<PlayCard>();

        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i] == null)
            {
                PlayCard drawnCard = deck.DrawCardFromDeck();

                if (drawnCard != null)
                {
                    hands[i] = drawnCard;

                    drawnCard.transform.SetParent(transform);
                    newCards.Add(drawnCard);

                }
            }
        }
        SetAllCardsInteraction(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        SortCardToRank();

        foreach (var card in newCards)
        {
            card.gameObject.SetActive(true);
            card.PlayDrawAnimation(deckSpawnPoint.position, currentDelay);
            currentDelay += delayStep;
        }
    }

    public bool SelectCard(PlayCard card)
    {
        //Debug.Log($"Selected Card - Rank: {card.rank}, Suit: {card.suit}");
        if (selectCardList.Count > 5) return false;
        
        if(selectCardList.Count == 5 && !selectCardList.Contains(card)) return false;


        if (selectCardList.Contains(card))
        {
            selectCardList.Remove(card);
        }
        else
        {
            if(selectCardList.Count < 5)
            {
                selectCardList.Add(card);
                if(selectSound != null) SoundManager.Instance.PlaySfxOneShot(selectSound, 0.3f);
            }
        }

        if (selectCardList.Count == 0) handEvaluator.CalculatePairCount(null);

        UpdateButtonState();

        handEvaluator.CalculatePairCount(selectCardList);

        return true;
    }

    public void ThrowAwayCard()
    {
        bool canDiscard = RoundManager.Instance != null && RoundManager.Instance.remainDiscard > 0;

        if (selectCardList.Count == 0 || !canDiscard)
        {
            PlayButtonErrorFeedback(discardButton.transform);
            return;
        }
        SetAllCardsInteraction(false);
        SetButtonsLoadingState();

        if (!RoundManager.Instance.ConsumeDiscardCount()) return;
        RoundManager.Instance.UpdateRecord("Discarded", selectCardList.Count);
        ExecuteCardRemoveAndDraw();
    }
    
    public void CalculateCard()
    {
        if (selectCardList.Count == 0)
        {
            PlayButtonErrorFeedback(playButton.transform);
            return;
        }
        SetAllCardsInteraction(false);
        SetButtonsLoadingState();

        calculator.CalculateScoreToSequence(selectCardList, handEvaluator, cardScoreArea.position, () => {
            RoundManager.Instance.UpdateRecord("Played", selectCardList.Count);
            ExecuteCardRemoveAndDraw();
        });
    }

    private void ExecuteCardRemoveAndDraw()
    {
        if (selectCardList == null) return;

        float delayOffset = 0f;

        foreach (PlayCard card in selectCardList)
        {
            card.VisualSelect();

            card.transform.DOMove(deck.transform.position, 0.4f)
            .SetDelay(delayOffset)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                // 애니메이션이 완전히 끝난 후 비활성화 처리
                deck.discardPack.Add(card);
                card.gameObject.SetActive(false);
            });

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] == card)
                {
                    hands[i] = null;
                    break;
                }
            }
            delayOffset += 0.05f;

        }
        selectCardList.Clear();
        UpdateButtonState();
        DOVirtual.DelayedCall(0.3f, () => DrawHands());
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

    public void SetupButtonColors()
    {
        activeBlock = ColorBlock.defaultColorBlock;
        activeBlock.normalColor = Color.white;
        activeBlock.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        activeBlock.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        activeBlock.selectedColor = Color.white;

        inactiveBlock = ColorBlock.defaultColorBlock;
        Color gray = new Color(0.4f, 0.4f, 0.4f, 1f);
        inactiveBlock.normalColor = gray;
        inactiveBlock.highlightedColor = gray;
        inactiveBlock.pressedColor = gray;
        inactiveBlock.selectedColor = gray;
    }

    public void UpdateButtonState()
    {
        bool hasSelection = selectCardList.Count > 0;
        bool canDiscard = RoundManager.Instance != null && RoundManager.Instance.remainDiscard > 0;

        playButton.colors = hasSelection ? activeBlock : inactiveBlock;

        discardButton.colors = (hasSelection && canDiscard) ? activeBlock : inactiveBlock;
    }

    private void SetButtonsLoadingState()
    {
        playButton.colors = inactiveBlock;
        discardButton.colors = inactiveBlock;
    }

    private void PlayButtonErrorFeedback(Transform target)
    {
        target.DOKill();
        target.DOPunchPosition(new Vector3(10, 0, 0), 0.3f, 20, 0.5f);
    }

    private void SetAllCardsInteraction(bool canInteract)
    {
        foreach (var card in hands)
        {
            if (card != null) card.SetInteraction(canInteract);
        }
    }

}
