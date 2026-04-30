using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("InGame UI References")]
    public Hand handObj;
    public TextMeshProUGUI remainHandText;
    public TextMeshProUGUI remainDiscardText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI playerTotalScoreText;
    public TextMeshProUGUI gameRoundText;
    public TextMeshProUGUI playChipText;
    public TextMeshProUGUI playMultText;
    public TextMeshProUGUI playRankingText;
    public TextMeshProUGUI playerCoinText;

    public Transform slotUIParent;

    [Header("Sound")]
    [SerializeField] private AudioClip gameStartMusic;

    private void Awake()
    {
        if (SoundManager.Instance != null && gameStartMusic != null)
        {
            SoundManager.Instance.PlayBgm(gameStartMusic, 0.3f, false);
        }
    }
    private void Start()
    {
        if (RoundManager.Instance != null)
        {
            // 인게임 씬이 시작되자마자 새로운 참조들을 매니저에게 밀어넣습니다.
            if (handObj == null) handObj = FindFirstObjectByType<Hand>();

            RoundManager.Instance.LinkInGameUI(
                handObj, remainHandText, remainDiscardText, targetScoreText,
                playerTotalScoreText, gameRoundText, playChipText,
                playMultText, playRankingText, playerCoinText
            );
        }

        if(JokerSlot.Instance != null)
        {
            JokerSlot.Instance.SetupSlotPosition(slotUIParent);
        }
        
    }

    
}