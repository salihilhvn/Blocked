using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, LevelComplete, Paused, Failed }
    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnStateChanged;
    public static event Action<int> OnMovesUpdated;
    public static event Action<int> OnScoreUpdated;

    public int MaxMoves { get; private set; }
    public int CurrentMoves { get; private set; }
    public int CurrentScore { get; private set; }

    [Header("Juice Effects")]
    public GameObject floatingTextPrefab;
    public GameObject confettiPrefab;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Sadece başlangıç durumu atıyoruz, eğer level yüklenmemişse.
        if (CurrentState != GameState.Playing)
        {
            ChangeState(GameState.MainMenu);
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game State Changed: " + newState);
        OnStateChanged?.Invoke(newState);
    }

    public void StartLevel(int maxMoves)
    {
        MaxMoves = maxMoves;
        CurrentMoves = maxMoves;
        CurrentScore = 0; // Bu levelde kazanılan coinler
        OnMovesUpdated?.Invoke(CurrentMoves);
        OnScoreUpdated?.Invoke(CurrentScore);
        ChangeState(GameState.Playing);
    }

    public void CheckLevelComplete(BlockController targetBlock)
    {
        if (CurrentState != GameState.Playing) return;

        if (targetBlock.GridPosition.x >= GridManager.Instance.Width - targetBlock.Length)
        {
            Debug.Log("Level Complete!");
            ChangeState(GameState.LevelComplete);
            
            // Sıradaki bölümü aç ve kaydet
            int nextLevel = LevelManager.Instance.currentLevelIndex + 1;
            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
            if (nextLevel > unlockedLevel)
            {
                PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
                PlayerPrefs.Save();
            }

            // Bu levelde kazandığı coinleri bankaya (TotalBoxCoins) ekle
            int totalCoins = PlayerPrefs.GetInt("TotalBoxCoins", 0);
            totalCoins += CurrentScore;
            PlayerPrefs.SetInt("TotalBoxCoins", totalCoins);
            PlayerPrefs.Save();

            if (confettiPrefab != null)
            {
                // Ekranın üstünden patlayıp aşağı dökülmesi için Y'yi biraz yüksek veriyoruz.
                Vector3 spawnPos = new Vector3(0, 3f, -2f);
                Instantiate(confettiPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    public void OnBlockMoved(int distance, Vector3 position)
    {
        if (CurrentState != GameState.Playing && CurrentState != GameState.LevelComplete) return;

        CurrentMoves--;
        CurrentScore += distance;
        
        // Sadece skoru günceller, level bitmeden kaydetmez
        OnMovesUpdated?.Invoke(CurrentMoves);
        OnScoreUpdated?.Invoke(CurrentScore);

        if (floatingTextPrefab != null && distance > 0)
        {
            // Yazıyı bloğun biraz üstünde doğur
            Vector3 spawnPos = position + new Vector3(0, 0.5f, -1f); 
            GameObject floatingTextObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            
            FloatingText floatingTextScript = floatingTextObj.GetComponent<FloatingText>();
            if (floatingTextScript != null)
            {
                floatingTextScript.Setup(distance);
            }
        }

        if (CurrentMoves <= 0 && CurrentState != GameState.LevelComplete)
        {
            Debug.Log("Level Failed - No moves left!");
            ChangeState(GameState.Failed);
        }
    }
}
