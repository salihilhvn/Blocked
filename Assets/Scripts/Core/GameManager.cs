using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, LevelComplete, Paused }
    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnStateChanged;

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

    public void CheckLevelComplete(BlockController targetBlock)
    {
        if (CurrentState != GameState.Playing) return;

        // Klasik Unblock Me'de sağdaki son hücreye gelmek veya grid dışına çıkmak leveli bitirir.
        // Bizim 6x6 gridimizde sağ çıkış x = 5 veya x = 4'ten başlar.
        // Eğer target bloğun başlangıç X noktası 4 ise (2 uzunluklu olduğu için 4,5'i kaplar) 
        // Dışarıya sürükleme animasyonunu başlatabilir veya direkt bitirebiliriz.
        if (targetBlock.GridPosition.x >= GridManager.Width - targetBlock.Length)
        {
            Debug.Log("Level Complete!");
            ChangeState(GameState.LevelComplete);
        }
    }
}
