using UnityEngine;
using TMPro;

public class StorePanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI totalBoxCoinsText;

    [Header("Power-Up Counts")]
    public TextMeshProUGUI timePUCountText;
    public TextMeshProUGUI doublePUCountText;
    public TextMeshProUGUI removerPUCountText;

    [Header("Power-Up Costs")]
    public int timePUCost = 500;
    public int doublePUCost = 1000;
    public int removerPUCost = 3000;

    private void OnEnable()
    {
        UpdateTotalCoinsDisplay();
        UpdatePowerUpCounts();
    }

    public void UpdatePowerUpCounts()
    {
        if (DataManager.Instance != null)
        {
            if (timePUCountText != null) timePUCountText.text = DataManager.Instance.GetPowerUpCount(DataManager.KEY_PU_TIME).ToString();
            if (doublePUCountText != null) doublePUCountText.text = DataManager.Instance.GetPowerUpCount(DataManager.KEY_PU_DOUBLE).ToString();
            if (removerPUCountText != null) removerPUCountText.text = DataManager.Instance.GetPowerUpCount(DataManager.KEY_PU_REMOVER).ToString();
        }
    }

    public void UpdateTotalCoinsDisplay()
    {
        if (totalBoxCoinsText != null)
        {
            // Hafızadaki toplam coinleri al
            if (DataManager.Instance != null)
            {
                totalBoxCoinsText.text = DataManager.Instance.GetTotalCoins().ToString();
            }
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
        if (DataManager.Instance != null)
        {
            DataManager.Instance.AddCoins(coinAmount);
        }

        // 2. Sol üstteki sayacı hemen güncelle
        UpdateTotalCoinsDisplay();

        // 3. Pop-up'ı kapat
        CloseAllPopups();
        
        Debug.Log("Satın alma başarılı: " + coinAmount + " Box Coin eklendi!");
    }

    // --- POWER-UP SATIN ALMA METOTLARI ---

    public void BuyTimePowerUp()
    {
        if (DataManager.Instance != null && DataManager.Instance.GetTotalCoins() >= timePUCost)
        {
            DataManager.Instance.SpendCoins(timePUCost);
            DataManager.Instance.AddPowerUp(DataManager.KEY_PU_TIME, 1);
            UpdateTotalCoinsDisplay();
            UpdatePowerUpCounts();
        }
    }

    public void BuyDoublePowerUp()
    {
        if (DataManager.Instance != null && DataManager.Instance.GetTotalCoins() >= doublePUCost)
        {
            DataManager.Instance.SpendCoins(doublePUCost);
            DataManager.Instance.AddPowerUp(DataManager.KEY_PU_DOUBLE, 1);
            UpdateTotalCoinsDisplay();
            UpdatePowerUpCounts();
        }
    }

    public void BuyRemoverPowerUp()
    {
        if (DataManager.Instance != null && DataManager.Instance.GetTotalCoins() >= removerPUCost)
        {
            DataManager.Instance.SpendCoins(removerPUCost);
            DataManager.Instance.AddPowerUp(DataManager.KEY_PU_REMOVER, 1);
            UpdateTotalCoinsDisplay();
            UpdatePowerUpCounts();
        }
    }
}
