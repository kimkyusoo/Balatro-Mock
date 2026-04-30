using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayCard : BaseCard
{
    [Header("PlayingCard Infomation")]
    public Suit suit;
    public int rank;
    public int baseChip;

    public Image playCardImage;

    private Hand hand;

    private bool isSelected = false;

    private float cardYPosition;
   [SerializeField] private RectTransform visualRoot;

    private CanvasGroup canvasGroup;

    [SerializeField] private AudioClip cardCalculatorSound;

    private void Awake()
    {
        cardYPosition = transform.localPosition.y;

        canvasGroup = visualRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = visualRoot.gameObject.AddComponent<CanvasGroup>();

    }
    public void Initalize(string cardId, string cardName, string description, Suit suit, int rank, int baseChip)
    {
        base.Initalize(cardId, cardName, description);

        this.suit = suit;
        this.rank = rank;
        this.baseChip = baseChip;

        //Debug.Log($"[PlayerCard] Suit: {this.suit}, Rank: {this.rank}, BaseChip: {this.baseChip}");
    }

    public string GetCardName()
    {
        string suitSymbol = suit switch
        {
            Suit.Spade => "♠",
            Suit.Diamond => "◆",
            Suit.Heart => "♥",
            Suit.Club => "♣", // 현재 클로버가 나오지 않고 있으나 이 부분은 추후 수정될 예정
            _ => ""
        };

        string rankText = rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => rank.ToString()
        };

        return $"{suitSymbol}{rankText}";
    }

    public void SetSprite(Sprite sprite)
    {
        playCardImage.sprite = sprite;
    }

    //public void SetHandReference(Hand hand)
    //{
    //    this.hand = hand;
    //}

    public void OnClickCard()
    {
        Hand playerHand = GetComponentInParent<Hand>();

        if (playerHand != null)
        {
            bool isSelected = playerHand.SelectCard(this);
            if(isSelected) VisualSelect();
        }
       
    }

    // 카드 선택 애니메이션
    public void VisualSelect()
    {
        isSelected = !isSelected;

        transform.DOKill();

        if (isSelected)
        {
            visualRoot.DOLocalMoveY(35f, 0.2f).SetEase(Ease.OutBack);
            visualRoot.DOShakePosition(0.1f, 7f);
        }
        else
        {
            visualRoot.DOLocalMoveY(0f, 0.2f).SetEase(Ease.OutQuad);
            visualRoot.DOShakePosition(0.1f, 4f);
        }
    }

    // 카드 드로우 애니메이션
    public void PlayDrawAnimation(Vector3 startWorldPos, float delay)
    {
        if (visualRoot == null) return;
     
        visualRoot.DOKill();

        visualRoot.position = startWorldPos;
        visualRoot.localScale = Vector3.one * 0.2f;
        visualRoot.localRotation = Quaternion.Euler(0, 0, 30f);
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        Sequence drawSequence = DOTween.Sequence().SetDelay(delay);

        drawSequence.Join(visualRoot.DOLocalMove(Vector3.zero, 0.6f).SetEase(Ease.OutCubic));
        drawSequence.Join(visualRoot.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        drawSequence.Join(visualRoot.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack));
        
        if (canvasGroup != null) drawSequence.Join(canvasGroup.DOFade(1f, 0.3f));

    }
    
    private void OnDestroy()
    {
        if (visualRoot != null) visualRoot.DOKill();
    }

    public Sequence PlayFocusAnimation(Vector3 targetWorldPos, float delay)
    {
        if (visualRoot == null) return null;
        visualRoot.DOKill();

        Sequence sequence = DOTween.Sequence().SetDelay(delay);
        sequence.Join(visualRoot.DOMove(targetWorldPos, 0.5f).SetEase(Ease.OutCubic));
        sequence.Join(visualRoot.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack));

        return sequence;
    }

    public void PlayScoringPunch() 
    {
        if (visualRoot == null) return;

        visualRoot.DOKill();
        visualRoot.localScale = Vector3.one * 1.2f;
        visualRoot.DOPunchPosition(Vector3.up * 30f, 0.15f, 15, 0.5f);

        if(cardCalculatorSound != null)
        {
            SoundManager.Instance.PlaySfxOneShot(cardCalculatorSound, 0.3f);
        }
    }

    public void SetInteraction(bool canInteract)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = canInteract;
        }
    }



}
