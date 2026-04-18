using UnityEngine;
using UnityEngine.UI;

public class PlayCard : BaseCard
{
    [Header("PlayingCard Infomation")]
    public Suit suit;
    public int rank;
    public int baseChip;

    public Image playCardImage;

    private Hand hand;

    private bool isSelected = false;

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

    public void SetHandReference(Hand hand)
    {
        this.hand = hand;
    }

    public void OnClickCard()
    {
        Hand playerHand = GetComponentInParent<Hand>();

        if (playerHand != null)
        {
            playerHand.SelectCard(this);
            VisualSelect();
        }
       
    }

    public void VisualSelect()
    {
        isSelected = !isSelected;
        
        transform.localPosition += isSelected ? new Vector3(0, 50, 0) : new Vector3(0, -50, 0);
    }

}
