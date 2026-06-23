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
}
