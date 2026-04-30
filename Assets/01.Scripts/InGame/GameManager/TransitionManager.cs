using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("UI Objects")]
    public CanvasGroup transitionCanvasGroup;
    public RectTransform transitionCard;

    [Header("Settings")]
    public float fadeInDuration = 0.8f;
    public float fadeOutDuration = 0.8f;
    public Ease fadeInEase = Ease.OutBack;
    public Ease fadeOutEase = Ease.InBack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            ResetState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ResetState()
    {
        if (transitionCanvasGroup != null) transitionCanvasGroup.alpha = 0f;
        if (transitionCard != null) transitionCard.localScale = Vector3.zero;
    }

    public void PlayFadeIn(Action onCompleted)
    {
        ResetState();
        KillTweens();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transitionCanvasGroup.DOFade(1f, 0.2f));
        sequence.Join(transitionCard.DOScale(1.5f, fadeInDuration).SetEase(fadeInEase));

        sequence.OnComplete(() => onCompleted?.Invoke());
    }

    public void PlayFadeOut(Action onComplete = null)
    {
        DOTween.Kill(transitionCanvasGroup);
        DOTween.Kill(transitionCard);

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(0.15f);

        sequence.Append(transitionCanvasGroup.DOFade(0f, 0.2f));
        sequence.Join(transitionCard.DOScale(0f, fadeOutDuration).SetEase(fadeOutEase));

        sequence.OnComplete(() => {
            ResetState();
            onComplete?.Invoke();
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (transitionCanvasGroup != null && transitionCanvasGroup.alpha > 0.5f)
        {
            DOVirtual.DelayedCall(0.15f, () => PlayFadeOut());
        }
    }

    private void KillTweens()
    {
        transitionCanvasGroup.DOKill();
        transitionCard.DOKill();
    }
}