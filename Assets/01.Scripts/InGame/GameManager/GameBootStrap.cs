using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    //public GameObject roundManagerPrefab;
    public GameObject jokerSlotPrefab;

    private void Awake()
    {
        // 게임 시작 시 딱 한 번만 생성합니다.
        //if (RoundManager.Instance == null) Instantiate(roundManagerPrefab);
        if (JokerSlot.Instance == null) Instantiate(jokerSlotPrefab);

    }
}