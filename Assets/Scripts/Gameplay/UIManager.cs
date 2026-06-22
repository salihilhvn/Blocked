using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject failedPanel;

    [Header("Texts")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI levelText;
    // Puan sistemi eklemek istersen:
    // public TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleGameStateChanged;
        GameManager.OnMovesUpdated += UpdateMovesText;
        LevelManager.OnLevelLoaded += UpdateLevelText;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleGameStateChanged;
        GameManager.OnMovesUpdated -= UpdateMovesText;
        LevelManager.OnLevelLoaded -= UpdateLevelText;
    }

    private void Start()
    {
        victoryPanel.SetActive(false);
        failedPanel.SetActive(false);
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Playing:
                victoryPanel.SetActive(false);
                failedPanel.SetActive(false);
                break;
            case GameManager.GameState.LevelComplete:
                victoryPanel.SetActive(true);
                break;
            case GameManager.GameState.Failed:
                failedPanel.SetActive(true);
                break;
        }
    }

    private void UpdateMovesText(int currentMoves)
    {
        if (movesText != null)
        {
            movesText.text = currentMoves.ToString();
        }
    }

    private void UpdateLevelText(int levelIndex)
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL " + levelIndex;
        }
    }

    // Butonlar için metodlar
    public void OnNextLevelClicked()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    public void OnRetryClicked()
    {
        LevelManager.Instance.ReloadCurrentLevel();
    }
}
