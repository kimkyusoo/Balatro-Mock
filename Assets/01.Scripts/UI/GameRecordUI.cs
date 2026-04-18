using TMPro;
using UnityEngine;

public class GameRecordUI : MonoBehaviour
{
    public TextMeshProUGUI mostPlayScoreText;
    public TextMeshProUGUI mostPlayHandText;
    public TextMeshProUGUI playCountText;
    public TextMeshProUGUI discardCountText;
    public TextMeshProUGUI purchaseCoinText;
    public TextMeshProUGUI rerollCountText;
    public TextMeshProUGUI roundCountText;

    public AudioClip gameOverMusic;

    private void Start()
    {
        if (RoundManager.Instance != null)
        {
            SetRecordDisplay();
        }

        if (SoundManager.Instance != null && gameOverMusic != null)
        {
            SoundManager.Instance.PlayBgm(gameOverMusic, 0.5f, false);
        }
    }

    public void SetRecordDisplay()
    {
        if (mostPlayScoreText != null) mostPlayScoreText.text = $"{RoundManager.Instance.mostPlayScore}";
        if (mostPlayHandText != null) mostPlayHandText.text = RoundManager.Instance.GetMostPlayedHandName();
        if (playCountText != null) playCountText.text = $"{RoundManager.Instance.playCount}";
        if (discardCountText != null) discardCountText.text = $"{RoundManager.Instance.discardCount}";
        if (purchaseCoinText != null) purchaseCoinText.text = $"{RoundManager.Instance.purchaseCoin}";
        if (rerollCountText != null) rerollCountText.text = $"{RoundManager.Instance.rerollCount}";
        if (roundCountText != null) roundCountText.text = $"{RoundManager.Instance.roundCount}";
    }
}
