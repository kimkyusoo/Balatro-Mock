
using UnityEngine;

public class BaseCard : MonoBehaviour
{
    [Header("CardInformation")]
    public string cardId;
    public string cardName;
    [TextArea(1, 3)] public string description;

    public CardType cardType;

    public void Initalize(string cardId, string cardName, string description)
    {
        this.cardId = cardId;
        this.cardName = cardName;
        this.description = description;
    }


    //public virtual void Add()
    //{
    //    Debug.Log($"{cardName} 카드를 추가하였습니다.");
    //}

    //public virtual void Remove()
    //{
    //    Debug.Log($"{cardName} 카드를 삭제하였습니다.");
    //}
}
