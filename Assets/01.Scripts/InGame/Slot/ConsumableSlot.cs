using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ConsumableSlot : MonoBehaviour
{
    public static ConsumableSlot Instance;

    [Header("Slot data")]
    public PlanetCard[] hasPlanetCards = new PlanetCard[2];
    public int hasPlanetCardCount = 0;
    public TextMeshProUGUI hasPlanetCountText;

    [Header("UI")]
    public Sprite[] planetSprites;
    public GameObject planetPrefab;
    public Transform consumableSlotArea;
    public Transform slotPosition;

    private void Awake()
    {
        transform.SetParent(null);
        if (Instance == null)
        {
            Instance = this;
            PlanetCard.SetupBaseRankLevel();
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool PlacePlanetCard(PlanetCard planetCard)
    {
        if(hasPlanetCardCount == 2) return false;

        for (int i = 0; i < hasPlanetCards.Length; i++)
        {
            if (hasPlanetCards[i] == null)
            {
                hasPlanetCards[i] = planetCard;
                hasPlanetCardCount++;


                hasPlanetCards[i].transform.SetParent(consumableSlotArea);

                hasPlanetCards[i].SetCardState(true);
                hasPlanetCards[i].VisualSelect(false);
                hasPlanetCards[i].transform.localScale = Vector3.one;
                hasPlanetCards[i].transform.localPosition = Vector3.zero;

                UpdateHasPlanetCount();
                return true;
            }
        }
        return false;
    }

    public void RemovePlanetCard(PlanetCard planetCard)
    {
        for (int i = 0; i < hasPlanetCards.Length; i++)
        {
            if (hasPlanetCards[i] == planetCard)
            {
                planetCard.gameObject.SetActive(false);
                planetCard.transform.SetParent(null);
                hasPlanetCards[i] = null;
                hasPlanetCardCount--;
                break;
            }
        }
        UpdateHasPlanetCount();
    }

    private void UpdateHasPlanetCount()
    {
        if (hasPlanetCountText != null)
        {
            if (hasPlanetCardCount == 0) hasPlanetCountText.text = $"0";

            hasPlanetCountText.text = $"{hasPlanetCardCount}/5";
        }
    }

    public void ResetPlanetSlot()
    {
        hasPlanetCardCount = 0;
        hasPlanetCards = new PlanetCard[2];

        if (consumableSlotArea != null)
        {
            foreach (Transform child in consumableSlotArea)
            {
                Destroy(child.gameObject);
            }
        }
        UpdateHasPlanetCount();
    }
}
