using UnityEngine;
using UnityEngine.UI;

public class CardRankPopup : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject[] rankPages;
    private int currentIndex = 0;

    [Header("Button UI")]
    [SerializeField] private Button nextButtonn;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        nextButtonn.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        currentIndex = 0;
        RefreshUI();
    }

    private void NextPage()
    {
        if (currentIndex < rankPages.Length - 1)
        {
            currentIndex++;
            RefreshUI();
        }
    }

    private void PrevPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        for (int i = 0; i < rankPages.Length; i++)
        {
            rankPages[i].SetActive(i == currentIndex);
        }

        prevButton.interactable = (currentIndex > 0);
        nextButtonn.interactable = (currentIndex < rankPages.Length - 1);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}