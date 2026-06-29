using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject inGamePanel; // Oyun içi UI (Moves, Level Text vb.)
    public GameObject victoryPanel;
    public GameObject failedPanel;

    [Header("Texts")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText; // Sağ üstteki Box Coin sayacı
    public TextMeshProUGUI totalScoreText; // Altındaki Total Block sayacı
    public TextMeshProUGUI timeText; // Zaman sayacı

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleGameStateChanged;
        GameManager.OnMovesUpdated += UpdateMovesText;
        GameManager.OnScoreUpdated += UpdateScoreText;
        GameManager.OnTimeUpdated += UpdateTimeText;
        LevelManager.OnLevelLoaded += UpdateLevelText;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleGameStateChanged;
        GameManager.OnMovesUpdated -= UpdateMovesText;
        GameManager.OnScoreUpdated -= UpdateScoreText;
        GameManager.OnTimeUpdated -= UpdateTimeText;
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
            case GameManager.GameState.MainMenu:
                if (inGamePanel != null) inGamePanel.SetActive(false);
                if (victoryPanel != null) victoryPanel.SetActive(false);
                if (failedPanel != null) failedPanel.SetActive(false);
                break;
            case GameManager.GameState.Playing:
                if (inGamePanel != null) inGamePanel.SetActive(true);
                if (victoryPanel != null) victoryPanel.SetActive(false);
                if (failedPanel != null) failedPanel.SetActive(false);
                
                // Oyun başlarken total score'u güncelle
                UpdateScoreText(GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0);
                
                if (GameManager.Instance != null)
                {
                    UpdateTimeText(GameManager.Instance.CurrentTime);
                }
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

    private void UpdateTimeText(float currentTime)
    {
        if (timeText != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsTimeUnlimited)
            {
                timeText.text = "∞";
                return;
            }

            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateScoreText(int currentScore)
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
        
        if (totalScoreText != null && DataManager.Instance != null)
        {
            // O ana kadarki kalıcı coinlerin üzerine o levelde kastığı güncel coin'i anlık ekleyerek göster
            int total = DataManager.Instance.GetTotalCoins() + currentScore;
            totalScoreText.text = total.ToString();
        }
    }

    public void RefreshScoreUI()
    {
        if (GameManager.Instance != null)
            UpdateScoreText(GameManager.Instance.CurrentScore);
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

    public void OnMainMenuClicked()
    {
        LevelManager.Instance.ClearLevel();
        GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
    }
}
