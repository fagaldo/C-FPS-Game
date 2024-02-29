using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
/// <summary>
/// Klasa odpowiedzialna za prawidłowe załadowanie preferowanych i wcześniej zapisanych przez użytkownika ustawień gry.
/// Następuje to przy starcie gry i ustawienia ładowane są do menu opcji dostępnego z menu głównego.
/// </summary>
public class LoadPrefs : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy będącego kontrolerem funkcjonalności menu głównego.
    /// </summary>
    [SerializeField] private MenuController menuController;
    /// <summary>
    /// Poler zawierające referencje do pola tekstowego wyświetlającego ogólny poziom głośności.
    /// </summary>
    [SerializeField] private TMP_Text masterVolumeTextValue = null;
    /// <summary>
    /// Poler zawierające referencje do pola tekstowego wyświetlającego poziom głośności muzyki.
    /// </summary>
    [SerializeField] private TMP_Text musicVolumeTextValue = null;
    /// <summary>
    /// Poler zawierające referencje do pola tekstowego wyświetlającego poziom głośności efektów dźwiękowych.
    /// </summary>
    [SerializeField] private TMP_Text SFXVolumeTextValue = null;
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować ogólny poziom głośności.
    /// </summary>
    [SerializeField] Slider masterVolumeSlider = null;
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować poziom głośności muzyki.
    /// </summary>
    [SerializeField] Slider musicVolumeSlider = null;
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować poziom głośności efektów specjalnych.
    /// </summary>
    [SerializeField] Slider SFXVolumeSlider = null;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego poziom czułości myszy.
    /// </summary>
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować poziom czułości myszy.
    /// </summary>
    [SerializeField] private Slider controllerSenSlider = null;
    /// <summary>
    /// Pole zawierające referencje do przełącznika, którym można ustawiać odwróconą oś Y sterowania kamery gracza.
    /// </summary>
    [SerializeField] private Toggle invertYToggle = null;
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować poziom jasności w grze.
    /// </summary>
    [SerializeField] private Slider brightnessSlider = null;
     /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego poziom jasności w grze.
    /// </summary>
    [SerializeField] private TMP_Text brightnessTextValue = null;
    /// <summary>
    /// Pole zawierające referencje do rozwijanego pola umożliwiającego wybór jakości ustawień graficznych.
    /// </summary>
    [SerializeField] private TMP_Dropdown qualityDropdown;
    /// <summary>
    /// Pole zawierające referencje do przełącznika, którym można ustawić pełny ekran, lub go wyłączyć.
    /// </summary>
    [SerializeField] private Toggle fullScreenToggle;
    /// <summary>
    /// Pole zawierające referencje do przełącznika, którym można ustawić synchronizację pionową, lub ją wyłączyć.
    /// </summary>
    [SerializeField] private Toggle vSyncToggle;
    /// <summary>
    /// Pole zawierające referencje do obiektu Audio Mixera, za pomocą którego można modyfikować poziomy głośności dźwięków w grze.
    /// </summary>
    [SerializeField] AudioMixer audioMixer = null;
    /// <summary>
    /// Pola zawierające wartości głośności: ogólnej, muzyki, oraz efektów dźwiękowych.
    /// </summary>
    float master, music, sfx;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Jest jedyną metodą w tej klasie i jej zadaniem jest sprawdzenie, 
    /// czy użytkownik ma zapisane jakieś ustawienia, a jeżeli tak, to następuje ich załadowanie do logiki gry, a także 
    /// do poszczególnych elementów menu opcji.
    /// </summary>
    private void Start()
    {

        if (PlayerPrefs.HasKey("masterVolume"))
        {
            master = PlayerPrefs.GetFloat("masterVolume");
            audioMixer.SetFloat("Master", master);
            masterVolumeSlider.value = master;
            masterVolumeTextValue.text = Mathf.Round(master + 80).ToString();
            music = PlayerPrefs.GetFloat("musicVolume");
            audioMixer.SetFloat("Music", music);
            musicVolumeSlider.value = music;
            musicVolumeTextValue.text = Mathf.Round(music + 80).ToString();
            sfx = PlayerPrefs.GetFloat("sfxVolume");
            audioMixer.SetFloat("SFX", sfx);
            SFXVolumeSlider.value = sfx;
            SFXVolumeTextValue.text = Mathf.Round(sfx + 80).ToString();

        }
        else
        {
            menuController.ResetButton("Audio");
        }
        if (PlayerPrefs.HasKey("masterQuality"))
        {
            int localQuality = PlayerPrefs.GetInt("masterQuality");
            qualityDropdown.value = localQuality;
            QualitySettings.SetQualityLevel(localQuality);
        }
        if (PlayerPrefs.HasKey("masterFullscreen"))
        {
            int tmp = PlayerPrefs.GetInt("masterFullscreen");
            if (tmp == 0)
            {
                Screen.fullScreen = false;
                fullScreenToggle.isOn = false;
            }
            else
            {
                Screen.fullScreen = true;
                fullScreenToggle.isOn = true;
            }
        }
        if (PlayerPrefs.HasKey("masterBrightness"))
        {
            float tmp = PlayerPrefs.GetFloat("masterBrightness");
            brightnessTextValue.text = tmp.ToString();
            brightnessSlider.value = tmp;
        }
        if (PlayerPrefs.HasKey("vsync"))
        {
            if (PlayerPrefs.GetInt("vsync") == 1)
            {
                QualitySettings.vSyncCount = 1;
                vSyncToggle.isOn = true;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                vSyncToggle.isOn = false;
            }
        }
        if (PlayerPrefs.HasKey("masterInvertY"))
        {
            int tmp = PlayerPrefs.GetInt("masterInvertY");
            if (tmp == 1)
                invertYToggle.isOn = true;
            else
                invertYToggle.isOn = false;
        }
        if (PlayerPrefs.HasKey("Sens"))
        {
            float tmp = PlayerPrefs.GetFloat("Sens");
            controllerSenSlider.value = tmp;
            controllerSenTextValue.text = tmp.ToString();
            menuController.mainControllerSen = tmp;
        }
    }

}
