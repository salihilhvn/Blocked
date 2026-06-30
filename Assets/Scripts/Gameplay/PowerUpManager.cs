using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    [Header("Main Container")]
    public GameObject powerUpContainer; // Güçlendiricilerin ve Exit butonunun bulunduğu ana obje

    [Header("UI Counts (TextMeshPro)")]
    public TextMeshProUGUI timeCountText;
    public TextMeshProUGUI removerCountText;
    public TextMeshProUGUI doubleCountText;

    [Header("Purchase Popups")]
    public GameObject purchaseTimePopup;
    public GameObject purchaseRemoverPopup;
    public GameObject purchaseDoublePopup;

    [Header("Not Enough Blocks Popup")]
    public GameObject notEnoughBlocksPopup;

    [Header("Exit Popup")]
    public GameObject exitConfirmPopup;

    [Header("Costs")]
    public int timeCost = 500;
    public int removerCost = 3000;
    public int doubleCost = 1000;

    public enum PowerUpType { None, Time, Remover, Double }
    private PowerUpType currentPendingPurchase = PowerUpType.None;

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
        UpdateUI();
        CloseAllPopups();
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (powerUpContainer != null)
        {
            // Sadece oyun oynanırken veya duraklatıldığında power-up ikonlarını göster
            bool shouldBeVisible = (newState == GameManager.GameState.Playing || newState == GameManager.GameState.Paused);
            powerUpContainer.SetActive(shouldBeVisible);
            
            // Eğer oyun ekranına (veya duraklatmaya) geçildiyse arayüzü yenile
            if (shouldBeVisible)
            {
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        if (DataManager.Instance == null) return;

        UpdateCountText(timeCountText, DataManager.KEY_PU_TIME);
        UpdateCountText(removerCountText, DataManager.KEY_PU_REMOVER);
        UpdateCountText(doubleCountText, DataManager.KEY_PU_DOUBLE);
    }

    private void UpdateCountText(TextMeshProUGUI textElement, string key)
    {
        if (textElement != null)
        {
            int count = DataManager.Instance.GetPowerUpCount(key);
            if (count > 0)
            {
                textElement.text = count.ToString();
                textElement.fontSize = 35;
            }
            else
            {
                textElement.text = "+";
                textElement.fontSize = 50;
            }
        }
    }

    private void CloseAllPopups()
    {
        if (purchaseTimePopup != null) purchaseTimePopup.SetActive(false);
        if (purchaseRemoverPopup != null) purchaseRemoverPopup.SetActive(false);
        if (purchaseDoublePopup != null) purchaseDoublePopup.SetActive(false);
        if (exitConfirmPopup != null) exitConfirmPopup.SetActive(false);
        if (notEnoughBlocksPopup != null) notEnoughBlocksPopup.SetActive(false);
    }

    // --- BUTTON CLICKS (MAIN ICONS) ---

    public void OnTimeButtonClicked()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        if (DataManager.Instance.GetPowerUpCount(DataManager.KEY_PU_TIME) > 0)
        {
            // Kullan
            DataManager.Instance.UsePowerUp(DataManager.KEY_PU_TIME);
            GameManager.Instance.ActivateTimeFreeze(10f); // 10 saniye dondur
            UpdateUI();
        }
        else
        {
            // Satın alma pop-up'ını aç
            GameManager.Instance.ChangeState(GameManager.GameState.Paused);
            currentPendingPurchase = PowerUpType.Time;
            if (purchaseTimePopup != null) purchaseTimePopup.SetActive(true);
        }
    }

    public void OnRemoverButtonClicked()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        if (DataManager.Instance.GetPowerUpCount(DataManager.KEY_PU_REMOVER) > 0)
        {
            // Kullan
            bool success = LevelManager.Instance.RemoveRandomBlock();
            if (success)
            {
                DataManager.Instance.UsePowerUp(DataManager.KEY_PU_REMOVER);
                UpdateUI();
            }
        }
        else
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Paused);
            currentPendingPurchase = PowerUpType.Remover;
            if (purchaseRemoverPopup != null) purchaseRemoverPopup.SetActive(true);
        }
    }

    public void OnDoubleButtonClicked()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        if (DataManager.Instance.GetPowerUpCount(DataManager.KEY_PU_DOUBLE) > 0)
        {
            // Kullan
            DataManager.Instance.UsePowerUp(DataManager.KEY_PU_DOUBLE);
            GameManager.Instance.ActivateDoubleScore(20f); // 20 saniye x2
            UpdateUI();
        }
        else
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Paused);
            currentPendingPurchase = PowerUpType.Double;
            if (purchaseDoublePopup != null) purchaseDoublePopup.SetActive(true);
        }
    }

    public void OnExitButtonClicked()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        GameManager.Instance.ChangeState(GameManager.GameState.Paused);
        if (exitConfirmPopup != null) exitConfirmPopup.SetActive(true);
    }

    // --- PURCHASE POPUP CONFIRM/CANCEL ACTIONS ---

    public void ConfirmPurchaseTime()
    {
        if (DataManager.Instance.GetTotalCoins() >= timeCost)
        {
            DataManager.Instance.SpendCoins(timeCost);
            DataManager.Instance.AddPowerUp(DataManager.KEY_PU_TIME, 1);
            UpdateUI();
            CloseAllPopups();
            GameManager.Instance.ChangeState(GameManager.GameState.Playing); // Resume
        }
        else
        {
            if (purchaseTimePopup != null) purchaseTimePopup.SetActive(false);
            if (notEnoughBlocksPopup != null) notEnoughBlocksPopup.SetActive(true);
        }
    }

    public void ConfirmPurchaseRemover()
    {
        if (DataManager.Instance.GetTotalCoins() >= removerCost)
        {
            DataManager.Instance.SpendCoins(removerCost);
            DataManager.Instance.AddPowerUp(DataManager.KEY_PU_REMOVER, 1);
            UpdateUI();
            CloseAllPopups();
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        }
        else
        {
            if (purchaseRemoverPopup != null) purchaseRemoverPopup.SetActive(false);
            if (notEnoughBlocksPopup != null) notEnoughBlocksPopup.SetActive(true);
        }
    }

    public void ConfirmPurchaseDouble()
    {
        if (DataManager.Instance.GetTotalCoins() >= doubleCost)
        {
            DataManager.Instance.SpendCoins(doubleCost);
            DataManager.Instance.AddPowerUp(DataManager.KEY_PU_DOUBLE, 1);
            UpdateUI();
            CloseAllPopups();
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        }
        else
        {
            if (purchaseDoublePopup != null) purchaseDoublePopup.SetActive(false);
            if (notEnoughBlocksPopup != null) notEnoughBlocksPopup.SetActive(true);
        }
    }

    public void CancelPurchase()
    {
        CloseAllPopups();
        currentPendingPurchase = PowerUpType.None;
        GameManager.Instance.ChangeState(GameManager.GameState.Playing); // Oyuna geri dön
    }

    // --- NOT ENOUGH BLOCKS POPUP ACTIONS ---

    public void CancelNotEnoughPopup()
    {
        if (notEnoughBlocksPopup != null) notEnoughBlocksPopup.SetActive(false);

        // Hangi güçlendiricideysek onun pop-up'ına geri dön
        switch (currentPendingPurchase)
        {
            case PowerUpType.Time:
                if (purchaseTimePopup != null) purchaseTimePopup.SetActive(true);
                break;
            case PowerUpType.Remover:
                if (purchaseRemoverPopup != null) purchaseRemoverPopup.SetActive(true);
                break;
            case PowerUpType.Double:
                if (purchaseDoublePopup != null) purchaseDoublePopup.SetActive(true);
                break;
        }
    }

    public void BuyNowInAppPurchase()
    {
        // Önce gerekli bakiye kadar parayı yatıralım (Sanki In-App Purchase yapılmış gibi)
        switch (currentPendingPurchase)
        {
            case PowerUpType.Time:
                DataManager.Instance.AddCoins(timeCost);
                break;
            case PowerUpType.Remover:
                DataManager.Instance.AddCoins(removerCost);
                break;
            case PowerUpType.Double:
                DataManager.Instance.AddCoins(doubleCost);
                break;
        }

        // Total Text'in güncellenmesi için UIManager'ı yenile
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.RefreshScoreUI();
        }

        // Pop-up'ı kapat ve öncekine dön
        CancelNotEnoughPopup();
    }

    // --- EXIT POPUP ACTIONS ---

    public void ConfirmExitToMainMenu()
    {
        CloseAllPopups();
        LevelManager.Instance.ClearLevel();
        GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
    }

    public void CancelExitBackToGame()
    {
        CloseAllPopups();
        GameManager.Instance.ChangeState(GameManager.GameState.Playing); // Oyuna geri dön
    }
}
