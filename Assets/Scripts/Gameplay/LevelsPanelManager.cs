using UnityEngine;
using UnityEngine.UI;

public class LevelsPanelManager : MonoBehaviour
{
    [Header("Level Buttons")]
    // Sahnede oluşturduğun 1, 2, 3 butonlarını buraya sürükleyeceksin.
    // 0. eleman Level 1 butonu, 1. eleman Level 2 butonu olacak şekilde sırayla.
    public Button[] levelButtons; 

    private void OnEnable()
    {
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        // Kaydedilmiş en yüksek açık bölümü al (Varsayılan 1)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // 0. index = 1. Level

            if (levelIndex <= unlockedLevel)
            {
                // Bölüm açık! Butona tıklanabilir.
                levelButtons[i].interactable = true;
                
                // Opaklığını %100 yap
                CanvasGroup cg = levelButtons[i].GetComponent<CanvasGroup>();
                if(cg != null) cg.alpha = 1f;
            }
            else
            {
                // Bölüm kilitli! Tıklanamaz.
                levelButtons[i].interactable = false;
                
                // Yarı saydam (soluk) yap
                CanvasGroup cg = levelButtons[i].GetComponent<CanvasGroup>();
                if(cg != null) cg.alpha = 0.5f;
            }
        }
    }

    // Butonların OnClick eventine bağlanacak metot
    public void OnLevelButtonClicked(int levelNumber)
    {
        Debug.Log("Loading Level: " + levelNumber);
        LevelManager.Instance.LoadLevel(levelNumber);
    }
    
    // Geliştirici kolaylığı: Tüm levelleri açmak istersen kullanabileceğin metod
    [ContextMenu("Unlock All Levels")]
    public void UnlockAllLevels()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 999);
        PlayerPrefs.Save();
        UpdateButtons();
    }

    [ContextMenu("Reset Levels")]
    public void ResetLevels()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        UpdateButtons();
    }
}
