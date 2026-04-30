using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button closeButton;

    private int currentPage = 0;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        currentPage = 0;
        UpdatePage();
    }

    private void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    private void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            bool isCurrentPage = (i == currentPage);
            pages[i].SetActive(isCurrentPage);
        }

        prevButton.interactable = (currentPage > 0);
        nextButton.interactable = (currentPage < pages.Length - 1);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
