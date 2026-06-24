using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    [Header("Sound Toggle")]
    public Button soundButton;
    public Image soundImage;

    [Header("Vibration Toggle")]
    public Button vibrationButton;
    public Image vibrationImage;

    [Header("Music Toggle")]
    public Button musicButton;
    public Image musicImage;

    [Header("Parental Control Toggle")]
    public Button parentalButton;
    public Image parentalImage;

    [Header("Toggle Sprites")]
    public Sprite spriteOn;
    public Sprite spriteOff;

    [Header("Panels & Navigation")]
    public GameObject resetConfirmationPopup; // Sıfırlama için "Emin misin?" ekranı
    public Button backButton; // Ana menüye dönmek için

    private void OnEnable()
    {
        UpdateAllUI();
    }

    private void Start()
    {
        // Buton dinleyicilerini (listener) bağlıyoruz
        if (soundButton != null) soundButton.onClick.AddListener(ToggleSound);
        if (vibrationButton != null) vibrationButton.onClick.AddListener(ToggleVibration);
        if (musicButton != null) musicButton.onClick.AddListener(ToggleMusic);
        if (parentalButton != null) parentalButton.onClick.AddListener(ToggleParental);
        
        if (backButton != null) backButton.onClick.AddListener(OnBackButtonClicked);

        if (resetConfirmationPopup != null) resetConfirmationPopup.SetActive(false);
    }

    private void UpdateAllUI()
    {
        if (DataManager.Instance == null) return;

        UpdateToggleVisual(soundImage, DataManager.Instance.GetSound());
        UpdateToggleVisual(vibrationImage, DataManager.Instance.GetVibration());
        UpdateToggleVisual(musicImage, DataManager.Instance.GetMusic());
        UpdateToggleVisual(parentalImage, DataManager.Instance.GetParentalControl());
    }

    private void UpdateToggleVisual(Image img, bool isOn)
    {
        if (img != null)
        {
            img.sprite = isOn ? spriteOn : spriteOff;
        }
    }

    private void ToggleSound()
    {
        bool newState = !DataManager.Instance.GetSound();
        DataManager.Instance.SetSound(newState);
        UpdateToggleVisual(soundImage, newState);
        // TODO: AudioListener volume ayarlaması eklenebilir
    }

    private void ToggleVibration()
    {
        bool newState = !DataManager.Instance.GetVibration();
        DataManager.Instance.SetVibration(newState);
        UpdateToggleVisual(vibrationImage, newState);

        if (newState)
        {
            // Titreşim test
            Handheld.Vibrate();
        }
    }

    private void ToggleMusic()
    {
        bool newState = !DataManager.Instance.GetMusic();
        DataManager.Instance.SetMusic(newState);
        UpdateToggleVisual(musicImage, newState);
        // TODO: Müzik durdur/başlat
    }

    private void ToggleParental()
    {
        bool newState = !DataManager.Instance.GetParentalControl();
        DataManager.Instance.SetParentalControl(newState);
        UpdateToggleVisual(parentalImage, newState);
    }

    // --- RESET MANTIĞI ---

    public void OnResetButtonClicked()
    {
        if (resetConfirmationPopup != null)
        {
            resetConfirmationPopup.SetActive(true);
        }
        else
        {
            ConfirmReset();
        }
    }

    public void ConfirmReset()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetProgress();
        }
        
        if (resetConfirmationPopup != null) resetConfirmationPopup.SetActive(false);
    }

    public void CancelReset()
    {
        if (resetConfirmationPopup != null) resetConfirmationPopup.SetActive(false);
    }

    // --- NAVIGATION ---

    public void OnBackButtonClicked()
    {
        // Settings panelini kapat
        gameObject.SetActive(false);

        // Eğer bir GameManager veya UI Manager varsa, Home/MainMenu ekranını açmasını söyleyebiliriz.
        // GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
    }
}
