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
    public static event Action<float> OnTimeUpdated;

    public int MaxMoves { get; private set; }
    public int CurrentMoves { get; private set; }
    public int CurrentScore { get; private set; }
    
    public float TimeLimit { get; private set; }
    public float CurrentTime { get; private set; }
    public bool IsTimeUnlimited { get; private set; }
    
    private float timeFreezeDuration = 0f;
    private float doubleScoreDuration = 0f;

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

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            // Double Score sayacı arka planda hep akar
            if (doubleScoreDuration > 0)
            {
                doubleScoreDuration -= Time.deltaTime;
            }

            // Time Freeze varsa ana süre durur, freeze süresi azalır
            if (timeFreezeDuration > 0)
            {
                timeFreezeDuration -= Time.deltaTime;
                return;
            }

            if (!IsTimeUnlimited)
            {
                CurrentTime -= Time.deltaTime;
            
                if (CurrentTime <= 0)
                {
                    CurrentTime = 0;
                    Debug.Log("Level Failed - Time's up!");
                    ChangeState(GameState.Failed);
                }
                
                OnTimeUpdated?.Invoke(CurrentTime);
            }
        }
    }

    public void ActivateTimeFreeze(float duration)
    {
        timeFreezeDuration = duration;
        Debug.Log("Time Freeze Activated for " + duration + " seconds!");
    }

    public void ActivateDoubleScore(float duration)
    {
        doubleScoreDuration = duration;
        Debug.Log("Double Score Activated for " + duration + " seconds!");
    }

    public void StartLevel(int maxMoves, float timeLimit)
    {
        timeFreezeDuration = 0f;
        doubleScoreDuration = 0f;
        MaxMoves = maxMoves;
        CurrentMoves = maxMoves;
        CurrentScore = 0; 
        
        TimeLimit = timeLimit;
        CurrentTime = timeLimit;
        IsTimeUnlimited = (timeLimit <= 0f);

        OnMovesUpdated?.Invoke(CurrentMoves);
        OnScoreUpdated?.Invoke(CurrentScore);
        if (!IsTimeUnlimited) OnTimeUpdated?.Invoke(CurrentTime);
        
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
            if (DataManager.Instance != null)
            {
                if (nextLevel > DataManager.Instance.GetUnlockedLevel())
                {
                    DataManager.Instance.SaveUnlockedLevel(nextLevel);
                    // İlk defa geçiliyorsa kazandığı coinleri bankaya ekle
                    DataManager.Instance.AddCoins(CurrentScore);
                }
            }

            if (ObjectPooler.Instance != null && ObjectPooler.Instance.confettiPrefab != null)
            {
                Vector3 spawnPos = new Vector3(0, 3f, -2f);
                ObjectPooler.Instance.SpawnConfetti(spawnPos);
            }
        }
    }

    public void OnBlockMoved(int distance, Vector3 position)
    {
        if (CurrentState != GameState.Playing && CurrentState != GameState.LevelComplete) return;

        CurrentMoves--;
        
        int earnedScore = distance;
        if (doubleScoreDuration > 0) 
        {
            earnedScore *= 2; // x2 Power-Up aktif
        }
        CurrentScore += earnedScore;
        
        // Sadece skoru günceller, level bitmeden kaydetmez
        OnMovesUpdated?.Invoke(CurrentMoves);
        OnScoreUpdated?.Invoke(CurrentScore);

        if (ObjectPooler.Instance != null && ObjectPooler.Instance.floatingTextPrefab != null && distance > 0)
        {
            Vector3 spawnPos = position + new Vector3(0, 0.5f, -1f); 
            GameObject floatingTextObj = ObjectPooler.Instance.SpawnFloatingText(spawnPos);
            
            if (floatingTextObj != null)
            {
                FloatingText floatingTextScript = floatingTextObj.GetComponent<FloatingText>();
                if (floatingTextScript != null)
                {
                    floatingTextScript.Setup(earnedScore);
                }
            }
        }

        if (CurrentMoves <= 0 && CurrentState != GameState.LevelComplete)
        {
            Debug.Log("Level Failed - No moves left!");
            ChangeState(GameState.Failed);
        }
    }
}
