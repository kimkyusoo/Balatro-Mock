using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuPopup : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject menuContent; 
    [SerializeField] private CardRankPopup cardRankPopup; 
    [SerializeField] private SoundSettingsPopup soundSettingsPopup; 

    [Header("Menu Buttons")]
    [SerializeField] private Button rankInfoButton;
    [SerializeField] private Button volumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;

    private bool isMenuOpen = false;

    private void Awake()
    {
        if (cardRankPopup == null)
        {
            cardRankPopup = FindFirstObjectByType<CardRankPopup>(FindObjectsInactive.Include);
        }

        if (soundSettingsPopup == null)
        {
            soundSettingsPopup = FindFirstObjectByType<SoundSettingsPopup>(FindObjectsInactive.Include);
        }

        if (rankInfoButton != null && cardRankPopup != null)
        {
            rankInfoButton.onClick.AddListener(() => cardRankPopup.Open());
        }

        if (volumeButton != null && soundSettingsPopup != null)
        {
            volumeButton.onClick.AddListener(() => soundSettingsPopup.Open());
        }

        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (returnButton != null) returnButton.onClick.AddListener(CloseMenu);

        CloseMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscInput();
        }
    }

    private void HandleEscInput()
    {
        Debug.Log("HandleEscInput ¡¯¿‘");
        if (cardRankPopup.gameObject.activeSelf)
        {
            cardRankPopup.Close();
            return;
        }

        if (soundSettingsPopup.gameObject.activeSelf)
        {
            soundSettingsPopup.gameObject.SetActive(false);
            return;
        }
        Debug.Log($"HandleEscInput, isMenuOpen: {isMenuOpen}");
        if (isMenuOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenu()
    {
        Debug.Log($"OpenMenu");
        isMenuOpen = true;
        menuContent.SetActive(true);
    }

    public void CloseMenu()
    {
        Debug.Log($"CloseMenu");
        isMenuOpen = false;
        menuContent.SetActive(false);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(1);
        RoundManager.Instance?.SetRoundInfo();
        JokerSlot.Instance?.ResetJokerSlot();
        ConsumableSlot.Instance?.ResetPlanetSlot();
    }
}