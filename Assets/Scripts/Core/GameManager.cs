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
        CurrentScore = 0; // Veya levela göre hesaplanabilir
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
        }
    }

    public void OnBlockMoved()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentMoves--;
        OnMovesUpdated?.Invoke(CurrentMoves);

        if (CurrentMoves <= 0)
        {
            Debug.Log("Level Failed - No moves left!");
            ChangeState(GameState.Failed);
        }
    }
}
