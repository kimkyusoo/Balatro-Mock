using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Play Deck Information")]
    public List<PlayCard> fullDeck = new List<PlayCard>();
    public List<PlayCard> discardPack = new List<PlayCard>();

    [Header("UI Reference")]
    public TextMeshProUGUI fullDeckText;
    public TextMeshProUGUI discardText;

    [Header("Ref Object, Component")]
    public GameObject cardPrefab;

    [Header("Sprite")]
    public Sprite[] cardSprites;

    public void Awake()
    {
        CreateDeck();
        ShuffleDeck();
        UpdateDeckUI();
    }

    public void CreateDeck()
    {
        for (int i = 0; i < 52; i++)
        {
            Suit assignedSuit = (Suit)(i / 13);
            int assignedRank = (i % 13) + 2;
            if(assignedRank == 14) assignedRank = 1;
            int chip = CalculateBaseChip(assignedRank);

            PlayCard newCard = Instantiate(cardPrefab, this.transform).GetComponent<PlayCard>();

            newCard.Initalize(i.ToString(), "Playing Card", " Chip: " + chip, assignedSuit, assignedRank, chip);
            newCard.SetSprite(cardSprites[i]);
            fullDeck.Add(newCard);

            newCard.name = newCard.GetCardName();
            newCard.gameObject.SetActive(false);
        }
    }

    public void ShuffleDeck()
    {
        for (int i = fullDeck.Count - 1; i >= 1; i--)
        {
            int randomCard = Random.Range(0, i + 1);
            PlayCard temp = fullDeck[i];
            fullDeck[i] = fullDeck[randomCard];
            fullDeck[randomCard] = temp;
        }
    }

    public PlayCard DrawCardFromDeck()
    {
        if (fullDeck.Count > 0)
        {
            PlayCard card = fullDeck[0];
            fullDeck.RemoveAt(0);
            UpdateDeckUI();
            return card;
        }
        return null; 
    }

    public int CalculateBaseChip(int rank)
    {
        if (rank == 1) return 10;
        return rank;
    }

    public void UpdateDeckUI()
    {
        if (fullDeckText != null) fullDeckText.text = $"{fullDeck.Count} / 52";
        if (discardText != null) discardText.text = $"{discardPack.Count}";
    }
}
