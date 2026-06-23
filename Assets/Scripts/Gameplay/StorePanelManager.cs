using UnityEngine;
using TMPro;

public class StorePanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI totalBoxCoinsText;

    private void OnEnable()
    {
        UpdateTotalCoinsDisplay();
    }

    public void UpdateTotalCoinsDisplay()
    {
        if (totalBoxCoinsText != null)
        {
            // Hafızadaki toplam coinleri al
            int totalCoins = PlayerPrefs.GetInt("TotalBoxCoins", 0);
            totalBoxCoinsText.text = totalCoins.ToString();
        }
    }

    [Header("Popups")]
    public GameObject purchase1000Popup;
    public GameObject purchase15000Popup;
    public GameObject purchase50000Popup;

    // Pakete tıklanınca çalışacak
    public void OpenPopup(int packageId)
    {
        CloseAllPopups(); // Önce açık olanları kapat (önlem olarak)
        
        if (packageId == 1000 && purchase1000Popup != null) purchase1000Popup.SetActive(true);
        else if (packageId == 15000 && purchase15000Popup != null) purchase15000Popup.SetActive(true);
        else if (packageId == 50000 && purchase50000Popup != null) purchase50000Popup.SetActive(true);
    }

    // Cancel veya başarılı satın alma sonrası tüm popupları kapatacak
    public void CloseAllPopups()
    {
        if (purchase1000Popup != null) purchase1000Popup.SetActive(false);
        if (purchase15000Popup != null) purchase15000Popup.SetActive(false);
        if (purchase50000Popup != null) purchase50000Popup.SetActive(false);
    }

    // Pop-up içindeki Confirm butonuna basınca çalışacak
    public void ConfirmPurchase(int coinAmount)
    {
        // 1. Coinleri bankaya ekle
        int totalCoins = PlayerPrefs.GetInt("TotalBoxCoins", 0);
        totalCoins += coinAmount;
        PlayerPrefs.SetInt("TotalBoxCoins", totalCoins);
        PlayerPrefs.Save();

        // 2. Sol üstteki sayacı hemen güncelle
        UpdateTotalCoinsDisplay();

        // 3. Pop-up'ı kapat
        CloseAllPopups();
        
        Debug.Log("Satın alma başarılı: " + coinAmount + " Box Coin eklendi!");
    }
}
