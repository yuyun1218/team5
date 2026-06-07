using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public GameObject tutorialPanel;

    public GameObject[] pages;

    public GameObject prevButton;
    public GameObject nextButton;
    public GameObject closeButton;

public PlayerController playerController;
    private int currentPage = 0;

   void Start()
{
    tutorialPanel.SetActive(true);

    // 只暫停遊戲
    Time.timeScale = 0f;

    ShowPage(0);
}    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);

        // 只有教學期間暫停
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentPage = 0;
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        prevButton.SetActive(index > 0);
        nextButton.SetActive(index < pages.Length - 1);
        closeButton.SetActive(index == pages.Length - 1);
    }

   

public void CloseTutorial()
{
    tutorialPanel.SetActive(false);

    // 恢復遊戲
    Time.timeScale = 1f;
}
}