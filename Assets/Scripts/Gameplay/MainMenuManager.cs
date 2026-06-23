using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject storePanel;  // Şimdilik boş kalabilir
    public GameObject levelsPanel; // Şimdilik boş kalabilir

    [Header("Bottom Bar Tabs")]
    public Image storeTabImage;
    public Image homeTabImage;
    public Image levelsTabImage;

    [Header("Tab Sprites (Active/Passive)")]
    public Sprite storeActiveSprite;
    public Sprite storePassiveSprite;
    public Sprite homeActiveSprite;
    public Sprite homePassiveSprite;
    public Sprite levelsActiveSprite;
    public Sprite levelsPassiveSprite;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        // Varsayılan olarak HOME tab'ini aktif yap
        OnHomeTabClicked();
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.MainMenu)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            
            // Menüdeyken alt sekmeleri göster
            if (storeTabImage != null) storeTabImage.gameObject.SetActive(true);
            if (homeTabImage != null) homeTabImage.gameObject.SetActive(true);
            if (levelsTabImage != null) levelsTabImage.gameObject.SetActive(true);

            OnHomeTabClicked(); // Menüye dönünce Home sekmesini aç
        }
        else
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (storePanel != null) storePanel.SetActive(false);
            if (levelsPanel != null) levelsPanel.SetActive(false);
            
            // Oyundayken alt sekmeleri gizle
            if (storeTabImage != null) storeTabImage.gameObject.SetActive(false);
            if (homeTabImage != null) homeTabImage.gameObject.SetActive(false);
            if (levelsTabImage != null) levelsTabImage.gameObject.SetActive(false);
        }
    }

    public void OnPlayClicked()
    {
        // Play'e basınca son kalınan bölümü yükle
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelIndex);
    }

    public void OnStoreTabClicked()
    {
        UpdateTabVisuals(storeTabImage);
        
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(false);
        if (storePanel != null) storePanel.SetActive(true);
        
        Debug.Log("Store Tab Opened");
    }

    public void OnHomeTabClicked()
    {
        UpdateTabVisuals(homeTabImage);

        if (storePanel != null) storePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        Debug.Log("Home Tab Opened");
    }

    public void OnLevelsTabClicked()
    {
        UpdateTabVisuals(levelsTabImage);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (storePanel != null) storePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(true);

        Debug.Log("Levels Tab Opened");
    }

    private void UpdateTabVisuals(Image activeImage)
    {
        // Önce hepsini pasif sprite yap
        if (storeTabImage != null && storePassiveSprite != null) storeTabImage.sprite = storePassiveSprite;
        if (homeTabImage != null && homePassiveSprite != null) homeTabImage.sprite = homePassiveSprite;
        if (levelsTabImage != null && levelsPassiveSprite != null) levelsTabImage.sprite = levelsPassiveSprite;

        // Tıklananı aktif sprite yap
        if (activeImage == storeTabImage && storeTabImage != null && storeActiveSprite != null)
            storeTabImage.sprite = storeActiveSprite;
        else if (activeImage == homeTabImage && homeTabImage != null && homeActiveSprite != null)
            homeTabImage.sprite = homeActiveSprite;
        else if (activeImage == levelsTabImage && levelsTabImage != null && levelsActiveSprite != null)
            levelsTabImage.sprite = levelsActiveSprite;
    }

    public void ResetAllData()
    {
        // Tüm kayıtları (Coinler, Açılan Leveller vs.) sil
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Yeniden 1. bölüme dön
        if (LevelManager.Instance != null) 
        {
            LevelManager.Instance.currentLevelIndex = 1;
        }

        // Levels panelindeki butonları da güncelle ki hepsi tekrar kilitlensin
        LevelsPanelManager levelsPanelManager = Object.FindFirstObjectByType<LevelsPanelManager>();
        if (levelsPanelManager != null) 
        {
            levelsPanelManager.UpdateButtons();
        }

        Debug.Log("Tüm veriler (Coinler, Leveller) sıfırlandı!");
    }
}
