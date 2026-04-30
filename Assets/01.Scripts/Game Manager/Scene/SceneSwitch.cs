using UnityEngine;
using UnityEngine.UI;
using System;

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
            goPlayButton.onClick.AddListener(() => {
                StartTransition(() => {
                    GameSceneManager.Instance.LoadSceneByName("InGame");
                });
            });

        }

        if (goShopButton != null)
        {
            goShopButton.onClick.AddListener(() => {
                StartTransition(() => {
                    if (RoundManager.Instance != null) RoundManager.Instance.EarnPlayerCoin();
                    if (RoundManager.Instance != null) RoundManager.Instance.PrepareNextRound();
                    GameSceneManager.Instance.LoadSceneByName("Shop");
                });
            });
        }

        if (goNextRoundButton != null)
        {
            goNextRoundButton.onClick.AddListener(() => {
                StartTransition(() => {
                    GameSceneManager.Instance.LoadSceneByName("InGame");
                });

            });
        }

        if (goTitleButton != null)
        {
            goTitleButton.onClick.AddListener(() =>
            {
                GameSceneManager.Instance.LoadSceneByName("Title");
                RoundManager.Instance?.SetRoundInfo();
                JokerSlot.Instance?.ResetJokerSlot();
                ConsumableSlot.Instance?.ResetPlanetSlot();
            });
        }

        if (goRestartButton != null)
        {
            goRestartButton.onClick.AddListener(() => StartTransition(() => {
                RoundManager.Instance?.SetRoundInfo();
                JokerSlot.Instance?.ResetJokerSlot();
                ConsumableSlot.Instance?.ResetPlanetSlot();
                GameSceneManager.Instance.LoadSceneByName("InGame");
            }));
        }
    }

    private void StartTransition(Action onLoadAction)
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.PlayFadeIn(() => {
                onLoadAction?.Invoke();
            });
        }
        else
        {
            onLoadAction?.Invoke();
        }
    }
}
