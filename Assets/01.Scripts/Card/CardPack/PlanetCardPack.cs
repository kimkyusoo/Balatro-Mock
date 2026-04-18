using UnityEngine;

public class PlanetCardPack : MonoBehaviour
{
    public string cardPackName;
    public Sprite cardPackImage;

    public PlanetCard targetPlanetCard;

    public ConsumableSlot consumableSlot;

    private void Awake()
    {
        if (targetPlanetCard == null) targetPlanetCard = new PlanetCard();
        //if (consumableSlot == null) consumableSlot = FindObjectOfType<ConsumableSlot>();

    }

    public void BuyTaroCardPack()
    {
        if (targetPlanetCard == null)
        {
            return;
        }
    }
}
