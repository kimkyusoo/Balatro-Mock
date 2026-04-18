using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    }

    private void OnDisable()
    {
        CardCalculator.scoreChanged -= SetScore;
        CardCalculator.chipChanged -= SetChipAndMult;
        HandEvaluator.rankingChanged -= SetRanking;
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

        UpdateGameInfo();
    }

    private int SetGoalScore()
    {
        int baseScore = 300 * (gameRound + 1);

        if(gameRound % 3 == 0)
        {
            baseScore *= 2;
        }

        return baseScore;
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
        playChip = chip;
        playMult = mult;
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
        if(remainHandText != null) remainHandText.text = $"{remainHand}";
        if(remainDiscardText !=null) remainDiscardText.text = $"{remainDiscard}";
        if (targetScoreText != null) targetScoreText.text = $"{targetScore}";
        if (playerTotalScoreText != null) playerTotalScoreText.text = $"{playerTotalScore}";
        if (gameRoundText != null) gameRoundText.text = $"{gameRound}";
        if (playChipText != null) playChipText.text = $"{playChip}";
        if (playMultText != null) playMultText.text = $"{playMult}";
        if (playerCoinText != null) playerCoinText.text = $"{playerCoin}";
        if(playRanking == HandRanking.None)
        {
            if (playRankingText != null) playRankingText.text = $"";
        }
        else
        {
            if (playRankingText != null) playRankingText.text = $"{playRanking}";
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
        if (handPlayHistory == null) return "" ;
        if (handPlayHistory.Count == 0) return "";

        HandRanking mostPlayedHand = HandRanking.None;
        int playCount = 0;
        
        foreach (KeyValuePair<HandRanking, int> hand in handPlayHistory)
        {
            if (hand.Value > playCount)
            {
                mostPlayedHand = hand.Key;
                playCount = hand.Value;
            }
        }
        return $"{mostPlayedHand} ({playCount})";
    }
}
