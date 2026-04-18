using UnityEngine;

public class PlanetCard : BaseCard
{
    [Header("PlanetCard Information")]
    public int sellPrice;
    public float addMult;
    public int addChip;

    [Header("Enforce HandRanking e.g. TwoPair")]
    public HandRanking enforceTarget;
}
