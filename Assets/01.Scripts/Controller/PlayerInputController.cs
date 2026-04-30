using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerInputReader playerInputReader;
    public Hand hand;
    public LayerMask cardLayer;


    //private void Awake()
    //{
    //    if(playerInputReader == null)
    //    {
    //        playerInputReader = GetComponent<PlayerInputReader>();
    //    }

    //    if(hand == null)
    //    {
    //        hand = GetComponent<Hand>();
    //    }
    //}

    //private void Update()
    //{
    //    if (playerInputReader == null || hand == null) return;

    //    if (playerInputReader.ThrowAwayCardPressedThisFrame)
    //    {
    //        hand.ThrowAwayCard();
    //    }

    //    if (playerInputReader.CalculateCardPressedThisFrame)
    //    {
    //        hand.CalculateCard();
    //    }
    //}
}
