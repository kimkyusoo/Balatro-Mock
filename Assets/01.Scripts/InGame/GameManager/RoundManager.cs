using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class RoundManager : MonoBehaviour
{
    public Hand hand;
    public static RoundManager Instance;

    [Header("InGame Data")]
    public int remainHand;
    public int remainDiscard;
    public int playChip;
    public float playMult;
    public int targetScore;
    public int playerTotalScore;
    public int gameRound;
    public int playerCoin;
    public HandRanking playRanking;
    [SerializeField] private int[] baseScores = { 300, 800, 2000, 5000, 11000, 20000, 30000, 50000 };
    public List<VoucherEffect> voucherEffects = new List<VoucherEffect>();

    [Header("Game Round Info Text")]
    public TextMeshProUGUI remainHandText;
    public TextMeshProUGUI remainDiscardText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI playerTotalScoreText;
    public TextMeshProUGUI gameRoundText;
    public TextMeshProUGUI playChipText;
    public TextMeshProUGUI playMultText;
    public TextMeshProUGUI playRankingText;
    public TextMeshProUGUI playerCoinText;

    [Header("ResultRecord")]
    public int mostPlayScore;
    public int mostPlayHand;
    public int playCount;
    public int discardCount;
    public int purchaseCoin;
    public int rerollCount;
    public int roundCount;
    public Dictionary<HandRanking, int> handPlayHistory = new Dictionary<HandRanking, int>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SetRoundInfo();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (hand == null) hand = FindFirstObjectByType<Hand>();

        UpdateGameInfo();
    }

    private void OnEnable()
    {
        CardCalculator.scoreChanged += SetScore;
        CardCalculator.chipChanged += SetChipAndMult;
        HandEvaluator.rankingChanged += SetRanking;
        JokerSlot.jokerChipChanged += SetChipAndMult;
    }

    private void OnDisable()
    {
        CardCalculator.scoreChanged -= SetScore;
        CardCalculator.chipChanged -= SetChipAndMult;
        HandEvaluator.rankingChanged -= SetRanking;
        JokerSlot.jokerChipChanged -= SetChipAndMult;
    }

    public void SetRoundInfo()
    {
        remainHand = 4;
        remainDiscard = 3;
        targetScore = 300;
        playerTotalScore = 0;
        gameRound = 1;
        playerCoin = 5;
        playChip = 0;
        playMult = 0;
        playRanking = HandRanking.None;

        mostPlayScore = 0;
        mostPlayHand = 0;
        playCount = 0; 
        discardCount = 0;
        purchaseCoin = 0;
        rerollCount = 0;

        handPlayHistory.Clear();
    }

    public void PrepareNextRound()
    {
        gameRound++;
        roundCount = gameRound;
        targetScore = SetGoalScore();

        playerTotalScore = 0;
        remainHand = 4;
        remainDiscard = 3;
        playRanking = HandRanking.None;
        playChip = 0;
        playMult = 0;

        ApplyRoundChanges();

        UpdateGameInfo();
    }

    private int SetGoalScore()
    {
        int anteIndex = (gameRound - 1) / 3;

        int roundIndex = ((gameRound - 1) % 3) + 1;

        if (anteIndex >= baseScores.Length)
        {
            anteIndex = baseScores.Length - 1;
        }

        int baseScore = baseScores[anteIndex];

        int finalScore = baseScore * roundIndex;

        return finalScore;
    }
    public void SetRanking(HandRanking ranking)
    {
        playRanking = ranking;
        UpdateGameInfo();
    }

    public void SetScore(int score)
    {
        if (score <= 0) return;

        if (hand.selectCardList.Count <= 0) return;

        playerTotalScore = score;
        UpdateGameInfo();
    }

    public void SetChipAndMult(int chip, float mult)
    {
        if (chip == 0 && mult == 0)
        {
            playChip = 0;
            playMult = 0;
        }

        if (chip > 0)
        {
            playChip = chip;
        }

        if(mult > 0)
        {
            playMult = mult;
        }

        UpdateGameInfo();
    }

    public bool ConsumeHandCount()
    {
        if (remainHand == 0) return false;

        if (hand.selectCardList.Count <= 0) return false;

        remainHand--;

        //Debug.Log($"남은 핸드 횟수: {remainHand}");
        UpdateGameInfo();
        return true;
    }

    public bool ConsumeDiscardCount()
    {
        if(remainDiscard == 0)
        {
            // 진동 애니메이션 추가 예정
            Debug.Log("남은 버리기 횟수가 없습니다.");
            return false;
        }

        if (hand.selectCardList.Count <= 0) 
        {
            Debug.Log("카드를 선택해야합니다.");
            return false; 
        } 
        remainDiscard--;

        //Debug.Log($"남은 버리기 횟수: {remainDiscard}");

        UpdateGameInfo();
        return true;
    }

    public void IsReachedTargetScore()
    {
        playChip = 0;
        playMult = 0;
        playRanking = HandRanking.None;

        UpdateGameInfo();

        if (playerTotalScore < targetScore)
        {
            if(remainHand > 0)
            {
                return;
            }
            else
            {
                GameSceneManager.Instance.LoadSceneByName("GameOver");
            }
        }
        else
        {
            GameSceneManager.Instance.LoadSceneByName("Clear");
            UpdateGameInfo();
        }
    }

    public void UpdateGameInfo()
    {
        if (remainHandText != null) UpdateTextWithJuice(remainHandText, $"{remainHand}");
        if (remainDiscardText !=null) UpdateTextWithJuice(remainDiscardText, $"{remainDiscard}");
        if (targetScoreText != null) targetScoreText.text = $"{targetScore}";
        if (playerTotalScoreText != null) UpdateTextWithJuice(playerTotalScoreText, $"{playerTotalScore}");
        if (gameRoundText != null) UpdateTextWithJuice(gameRoundText, $"{gameRound}");
        if (playChipText != null) UpdateTextWithJuice(playChipText, $"{playChip}", true);
        if (playMultText != null) UpdateTextWithJuice(playMultText, $"{playMult}", true);
        if (playerCoinText != null) UpdateTextWithJuice(playerCoinText,$"{playerCoin}");

        string rankString = (playRanking == HandRanking.None) ? "" : $"{playRanking}";

        if(playRankingText != null)
        {
            if (playRanking != HandRanking.None && playRankingText.text != rankString)
            {
                playRankingText.text = rankString;

                playRankingText.transform.DOKill();

                playRankingText.transform.DOPunchPosition(Vector3.up * 10f, 0.3f);
                playRankingText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            }
            else
            {
                playRankingText.text = rankString;

                if (playRanking == HandRanking.None)
                {
                    playRankingText.transform.localScale = Vector3.one;
                }
            }
        }
    }

    public bool CheckPlayerCoin(int requireCoin)
    {
        if(playerCoin < requireCoin)
        {
            Debug.Log("코인이 부족합니다.");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ConsumePlayerCoin(int requireCoin)
    {
        if(requireCoin <= 0) return;
        if (playerCoin < requireCoin) return;

        playerCoin -= requireCoin;
        UpdateGameInfo();
    }

    public void SavePlayerCoin(int requireCoin)
    {
        if (requireCoin <= 0) return;

        playerCoin += requireCoin;
        UpdateGameInfo();
    }

    public void EarnPlayerCoin()
    {
        int earnCoin = RewardCalculator.CalculateCoin();

        if (earnCoin <= 0) return;

        playerCoin += earnCoin;
        UpdateGameInfo();
    }

    public void LinkInGameUI(Hand hand, TextMeshProUGUI remainHandText, TextMeshProUGUI remainDiscardText, TextMeshProUGUI targetScoreText, TextMeshProUGUI playerTotalScoreText, TextMeshProUGUI gameRoundText, TextMeshProUGUI playChipText, TextMeshProUGUI playMultText, TextMeshProUGUI playRankingText, TextMeshProUGUI playerCoinText)
    {
        this.hand = hand;
        this.remainHandText = remainHandText;
        this.remainDiscardText = remainDiscardText;
        this.targetScoreText = targetScoreText;
        this.playerTotalScoreText = playerTotalScoreText;
        this.gameRoundText = gameRoundText;
        this.playChipText = playChipText;
        this.playMultText = playMultText;
        this.playRankingText = playRankingText;
        this.playerCoinText = playerCoinText;

        UpdateGameInfo();
    }

    // 2. 다른 씬(Shop, Result)에서 특정 텍스트만 연결할 때 호출
    public void LinkSimpleUI(TextMeshProUGUI playerCoinText, TextMeshProUGUI gameRoundText = null)
    {
        this.playerCoinText = playerCoinText;
        if (gameRoundText != null)
        {
            this.gameRoundText = gameRoundText;
        }

        UpdateGameInfo();
    }

    public void CheckMostScore(int score)
    {
        if( score <= 0) return;
        
        if (mostPlayScore == 0) mostPlayScore = score;

        mostPlayScore = Mathf.Max(mostPlayScore, score);
    }

    public void UpdateRecord(string recordType, int score)
    {
        switch (recordType)
        {
            case "Played":
                playCount += score;
                break;
            case "Discarded":
                discardCount += score;
                break;
            case "Purchased":
                purchaseCoin += score;
                break;
            case "Rerolled":
                rerollCount += score;
                break;
            default:
                Debug.Log("잘못된 기록 유형입니다.");
                break;
        }
    }

    public void CheckPlayeRanking(HandRanking ranking)
    {
        if(ranking == HandRanking.None) return;

        if(handPlayHistory.ContainsKey(ranking))
        {
            handPlayHistory[ranking]++;
        }
        else
        {
            handPlayHistory[ranking] = 1;
        }
    }

    public string GetMostPlayedHandName()
    {
        int playCount = 0;
        string mostHandText = "";
        if (handPlayHistory == null) return "" ;
        if (handPlayHistory.Count == 0) return "";

        HandRanking mostPlayedHand = HandRanking.None;
        
        foreach (KeyValuePair<HandRanking, int> hand in handPlayHistory)
        {
            if (hand.Value > playCount)
            {
                mostPlayedHand = hand.Key;
                playCount = hand.Value;
            }
        }
        switch (mostPlayedHand)
        {
            case HandRanking.HighCard: mostHandText = "하이카드"; break;
            case HandRanking.OnePair: mostHandText = "원페어"; break;
            case HandRanking.TwoPair: mostHandText = "투페어"; break;
            case HandRanking.Triple: mostHandText = "트리플"; break;
            case HandRanking.FourCard: mostHandText = "포카드"; break;
            case HandRanking.FullHouse: mostHandText = "풀하우스"; break;
            case HandRanking.Straight: mostHandText = "스트레이트"; break;
            case HandRanking.Flush: mostHandText = "플러시"; break;
            case HandRanking.StraightFlush: mostHandText = "스트레이트 플러시"; break;
        }
        return $"{mostHandText} ({playCount})";
    }

    public void UpdateTextWithJuice(TextMeshProUGUI targetText, string newText, bool isCritical = false)
    {
        if(targetText.text == newText) return;

        targetText.transform.DOKill();

        targetText.transform.localScale = Vector3.one;
        targetText.text = newText;

        if (isCritical)
        {
            targetText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f, 10, 1f);
        }
        else
        {
            targetText.transform.DOShakePosition(0.2f, 5f, 20);
        }
    }
    public void AddVoucherEffect(VoucherEffect voucherEffect)
    {
        if (voucherEffect == VoucherEffect.TwiceCoin)
        {
            playerCoin += 15;
            UpdateGameInfo();
            return; 
        }

        if (!voucherEffects.Contains(voucherEffect))
        {
            voucherEffects.Add(voucherEffect);
        }
        ApplyRoundChanges();
    }

    public void ApplyRoundChanges()
    {
        foreach (VoucherEffect effect in voucherEffects)
        {
            switch (effect)
            {
                case VoucherEffect.AddHands: remainHand += 1; break;
                case VoucherEffect.AddDiscards: remainDiscard += 1; break;
                case VoucherEffect.AddHandsAndReduceDiscards: remainHand += 1; remainDiscard -= 1; break;
                case VoucherEffect.AddDiscardsAndReduceHands: remainHand -= 1; remainDiscard += 1; break;
            }
        }
        UpdateGameInfo();
    }

}
