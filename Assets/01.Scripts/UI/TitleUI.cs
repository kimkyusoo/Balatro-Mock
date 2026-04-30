using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;   

public class TitleUI : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip titleMusic;

    [Header("Animation Targets")]
    
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private Image backgroundImage;

    [Header("Animation Settings")]
    [SerializeField] float speed = 15f;
    [SerializeField] float imageScale = 1.1f;
    [SerializeField] float sizeDuration = 8f;

    [Header("ButtonUI")]
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject soundSetButton;

    [Header("Popups")]
    [SerializeField] private TutorialPopup tutorialPopup; 
    [SerializeField] private SoundSettingsPopup soundSettingsPopup;

    private void Awake()
    {
        // 버튼 리스너 연결
        if (tutorialButton != null)
        {
            tutorialButton.GetComponent<Button>().onClick.AddListener(OpenTutorial);
        }

        if (soundSetButton != null)
        {
            soundSetButton.GetComponent<Button>().onClick.AddListener(OpenSoundSettings);
        }
    }
    private void Start()
    {
        if (SoundManager.Instance != null && titleMusic != null)
        {
            SoundManager.Instance.PlayBgm(titleMusic, 0.3f);
        }

        AnimateBalatroBackground();
    }

    private void AnimateBalatroBackground()
    {
        if (backgroundRect == null) return;
        
        backgroundRect.DOKill();

        // 백그라운드 스케일, 회전률 초기화시키기
        backgroundRect.localScale = Vector3.one;
        backgroundRect.localRotation = Quaternion.identity;

        // -360도 회전하는 시간을 speed로 나누어 속도 조절.
        // RotateMode
        // 1. Fast: 가장 빠른 경로로 회전. ==> 0, 360도의 경우 같기 대문에 회전x or 미세하게 움직임
        // 2. FastBeyond360: 최단거리 무시, 입력한 각도 수치 그대로 회전 ==> 360도일 경우 한바퀴, 720도일 경우 두바퀴 회전

        // Ease enum
        // 가속도 곡선을 결정하는 설정값.
        // ex. In - 처음에 느리게 시작해서 목표속도까지 점차 상승
        //     Out - 처음에 빠르게 시작해서 목표속도까지 점차 하락
        //     InOut - 처음에 느리게 시작해서 목표속도까지 점차 상승, 그리고 다시 느리게 하락
        //     Linear - 일정한 속도
        float durationPerRotation = 360f / speed;
        backgroundRect.DORotate(new Vector3(0, 0, -360), durationPerRotation, RotateMode.FastBeyond360)
                      .SetEase(Ease.Linear)
                      .SetLoops(-1, LoopType.Restart); // 무한반복
    }

    private void OnDestroy()
    {
        if (backgroundRect != null) backgroundRect.DOKill();
        if (backgroundImage != null) backgroundImage.DOKill();
    }

    private void OpenTutorial()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.Open();
        }
    }

    private void OpenSoundSettings()
    {
        if (soundSettingsPopup != null)
        {
            soundSettingsPopup.Open();
        }
    }
}