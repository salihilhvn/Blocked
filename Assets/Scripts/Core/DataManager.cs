using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public const string KEY_UNLOCKED_LEVEL = "UnlockedLevel";
    public const string KEY_TOTAL_COINS = "TotalBoxCoins";
    public const string KEY_SOUND = "Setting_Sound";
    public const string KEY_MUSIC = "Setting_Music";
    public const string KEY_VIBRATION = "Setting_Vibration";
    public const string KEY_PARENTAL = "Setting_Parental";
    public const string KEY_PU_TIME = "PU_Time";
    public const string KEY_PU_REMOVER = "PU_Remover";
    public const string KEY_PU_DOUBLE = "PU_Double";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Opsiyonel, ana ekrandan geçiş varsa açılabilir.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(KEY_UNLOCKED_LEVEL, 1);
    }

    public void SaveUnlockedLevel(int level)
    {
        if (level > GetUnlockedLevel())
        {
            PlayerPrefs.SetInt(KEY_UNLOCKED_LEVEL, level);
            PlayerPrefs.Save();
        }
    }

    public int GetTotalCoins()
    {
        return PlayerPrefs.GetInt(KEY_TOTAL_COINS, 0);
    }

    public void AddCoins(int amount)
    {
        int currentCoins = GetTotalCoins();
        currentCoins += amount;
        PlayerPrefs.SetInt(KEY_TOTAL_COINS, currentCoins);
        PlayerPrefs.Save();
    }

    public bool SpendCoins(int amount)
    {
        int currentCoins = GetTotalCoins();
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            PlayerPrefs.SetInt(KEY_TOTAL_COINS, currentCoins);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // --- POWER UPS ---
    public int GetPowerUpCount(string key) => PlayerPrefs.GetInt(key, 0);
    public void AddPowerUp(string key, int amount) 
    {
        int current = GetPowerUpCount(key);
        PlayerPrefs.SetInt(key, current + amount);
        PlayerPrefs.Save();
    }
    public bool UsePowerUp(string key)
    {
        int current = GetPowerUpCount(key);
        if (current > 0)
        {
            PlayerPrefs.SetInt(key, current - 1);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // --- SETTINGS ---
    public bool GetSound() => PlayerPrefs.GetInt(KEY_SOUND, 1) == 1;
    public void SetSound(bool isOn) { PlayerPrefs.SetInt(KEY_SOUND, isOn ? 1 : 0); PlayerPrefs.Save(); }

    public bool GetMusic() => PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
    public void SetMusic(bool isOn) { PlayerPrefs.SetInt(KEY_MUSIC, isOn ? 1 : 0); PlayerPrefs.Save(); }

    public bool GetVibration() => PlayerPrefs.GetInt(KEY_VIBRATION, 1) == 1;
    public void SetVibration(bool isOn) { PlayerPrefs.SetInt(KEY_VIBRATION, isOn ? 1 : 0); PlayerPrefs.Save(); }

    public bool GetParentalControl() => PlayerPrefs.GetInt(KEY_PARENTAL, 0) == 1; // Default OFF
    public void SetParentalControl(bool isOn) { PlayerPrefs.SetInt(KEY_PARENTAL, isOn ? 1 : 0); PlayerPrefs.Save(); }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(KEY_UNLOCKED_LEVEL);
        PlayerPrefs.DeleteKey(KEY_TOTAL_COINS);
        PlayerPrefs.Save();
        Debug.Log("Progress Reset! Unlocked levels and coins cleared.");
    }
}
