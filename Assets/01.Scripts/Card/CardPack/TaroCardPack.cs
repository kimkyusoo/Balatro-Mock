using UnityEngine;

public class TaroCardPack : MonoBehaviour
{
    public string cardPackName;
    public Sprite cardPackImage;

    public TaroCard targetTaroCard;

    public ConsumableSlot consumableSlot;

    private void Awake()
    {
        if(targetTaroCard == null) targetTaroCard = new TaroCard(); 
        if(consumableSlot == null) consumableSlot = FindObjectOfType<ConsumableSlot>();

    }

    // 타로카드팩 구매 시 랜덤한 타로카드가 생성되도록 설정.
    public void BuyTaroCardPack()
    {
        if(targetTaroCard == null) 
        {
            return;
        }
    }
    
}
