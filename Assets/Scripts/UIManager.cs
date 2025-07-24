using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI levelText;

    public void ShowMainScreen()
    {
        mainScreen.SetActive(true);
        gameScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    public void ShowGameScreen()
    {
        mainScreen.SetActive(false);
        gameScreen.SetActive(true);
        winScreen.SetActive(false);
        UpdateLevelText();
    }

    public void ShowWinScreen()
    {
        mainScreen.SetActive(false);
        gameScreen.SetActive(false);
        winScreen.SetActive(true);
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {GameManager.Instance.GetCurrentLevel()}";
        }
        else
        {
            Debug.LogWarning("Level TextMeshProUGUI not assigned in UIManager");
        }
    }

    public void OnStartButton()
    {
        GameManager.Instance.LoadLevel(1);
    }

    public void OnResetButton()
    {
        GameManager.Instance.ResetLevel();
    }

    public void OnNextLevelButton()
    {
        GameManager.Instance.NextLevel();
    }
}