using UnityEngine;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    [Header("Title -> InGame")]
    [SerializeField] private Button goPlayButton;

    [Header("Result -> Shop")]
    [SerializeField] private Button goShopButton;

    [Header("Shop -> InGame")]
    [SerializeField] private Button goNextRoundButton;

    [Header("GameOver -> Title")]
    [SerializeField] private Button goTitleButton;

    [Header("GameOver -> InGame")]
    [SerializeField] private Button goRestartButton;

    void Start()
    {
        if (goPlayButton != null)
        {
            goPlayButton.onClick.AddListener(() => GameSceneManager.Instance.LoadSceneByName("InGame"));
        }

        if (goShopButton != null)
        {
            goShopButton.onClick.AddListener(() => {
                if (RoundManager.Instance != null) RoundManager.Instance.EarnPlayerCoin();
                GameSceneManager.Instance.LoadSceneByName("Shop");
            });
        }

        if (goNextRoundButton != null)
        {
            goNextRoundButton.onClick.AddListener(() => {
                if (RoundManager.Instance != null)
                {
                    RoundManager.Instance.PrepareNextRound();
                }
                GameSceneManager.Instance.LoadSceneByName("InGame"); 

            });
        }

        if (goTitleButton != null)
        {
            goTitleButton.onClick.AddListener(() => GameSceneManager.Instance.LoadSceneByName("Title"));
        }

        if (goRestartButton != null)
        {
            goRestartButton.onClick.AddListener(() => GameSceneManager.Instance.LoadSceneByName("InGame"));
        }
    }
}
