using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System;
using System.IO;
/// <summary>
/// Klasa odpowiedzialna za kontrolowanie wszystkich operacji i mechanik związanych z 
/// menu głównym gry i przede wszystkim menu ustawień gry.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Volume Settings")]
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
    /// Pole zawierające wartość domyślną głośności gry.
    /// </summary>
    [SerializeField] private float defVolume = 0f;
    /// <summary>
    /// Pole zawierające referencje do obiektu będącego oknem dialogowym potwierdzającym zapisanie wybranych ustawień.
    /// </summary>
    [SerializeField] private GameObject confirmationPrompt = null;
    /// <summary>
    /// Pole zawierające referencje do obiektu Audio Mixera, za pomocą którego można modyfikować poziomy głośności dźwięków w grze.
    /// </summary>
    [SerializeField] AudioMixer audioMixer = null;
    [Header("Gameplay Settings")]
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego poziom czułości myszy.
    /// </summary>
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować poziom czułości myszy.
    /// </summary>
    [SerializeField] private Slider controllerSenSlider = null;
    /// <summary>
    /// Pole zawierające wartość domyślną poziomu czułości myszy w grze.
    /// </summary>
    [SerializeField] private float defaultSens = 4;
    /// <summary>
    /// Pole zawierające wartość czułości myszy wybraną przez użytkownika w ustawieniach.
    /// </summary>
    public float mainControllerSen = 4;
    /// <summary>
    /// Pole zawierające referencje do przełącznika, którym można ustawiać odwróconą oś Y sterowania kamerą gracza.
    /// </summary>
    [SerializeField] private Toggle invertYToggle = null;
    [Header("Graphics Settings")]
    /// <summary>
    /// Pole zawierające referencje do suwaka, którym można modyfikować poziom jasności w grze.
    /// </summary>
    [SerializeField] private Slider brightnessSlider = null;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego poziom jasności w grze.
    /// </summary>
    [SerializeField] private TMP_Text brightnessTextValue = null;
    /// <summary>
    /// Pole zawierające wartość domyślną poziomu jasności w grze.
    /// </summary>
    [SerializeField] private float defaultBrightness = 0.05f;
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
    /// Pole zawierające informacje o wybranym przez użytkownika poziomie jakości ustawień graficznych.
    /// </summary>
    private int qualityLevel;
    /// <summary>
    /// Pole zawierające informacje logiczną o tym, czy gra wyświetlana jest w pełnym ekranie.
    /// </summary>
    private bool _isFullScreen;
    /// <summary>
    /// Pole zawierające informacje o tym, czy ustawiona jest synchronizacja pionowa w grze.
    /// </summary>
    private bool isVsync;
    /// <summary>
    /// Pole zawierające wartość jasności w grze.
    /// </summary>
    private float brightnessLevel;
    [Header("Resolution Dropdowns")]
    /// <summary>
    /// Pole zawierające referencje do rozwijanego pola umożliwiającego wybór wyświetlanej rozdzielczości gry.
    /// </summary>
    public TMP_Dropdown resolutionDropdown;
    /// <summary>
    /// Tablica zawierająca rozdzielczości wyświetlane w rozwijanym polu służącym do jej wyboru.
    /// </summary>
    private Resolution[] resolutions;

    [Header("Levels to load")]
    /// <summary>
    /// Pole zawierające informacje o tym jaki lvl powinien być wczytany po rozpoczęciu gry przez użytkownika.
    /// </summary>
    private string levelToLoad;
    /// <summary>
    /// Pola zawierające wartości głośności: ogólnej, muzyki, oraz efektów dźwiękowych.
    /// </summary>
    float master, music, sfx;
    /// <summary>
    /// Pole zawierające referencje do obiektu będącego dialogiem informującym o braku pliku zapisu
    /// w przypadku gdy użytkownik chce wczytać grę, a tego zapisu brakuje
    /// </summary>
    /// <value></value>
    [SerializeField] private GameObject noSavedGameDialog = null;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy odpowiedzialnej za wyświetlanie odpowiednich ekranów ładowania.
    /// </summary>
    [SerializeField] LoadingScreen loadingScreen = null;
    /// <summary>
    /// Pole zawierające ścieżkę pod którą zapisywane są pliki zapisu gry.
    /// </summary>
    /// <value> Ścieżka zapisu stanu gry.</value>
    private string savePath => $"{Application.persistentDataPath}/save.txt";
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce skanowanie dostępnych dla danego sprzętu gracza rozdzielczości,
    /// a także odpowiednie załadowanie ich do rozwijanego pola wyboru rozdzielczości.
    /// </summary>
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + "@" + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    /// <summary>
    /// Metoda odpowiedzialna za rozpoczęcia nowej rozgrywki i usunięcie starego pliku zapisu.
    /// </summary>
    public void NewGameDialogYes()
    {
        PlayerPrefs.DeleteKey("SavedLevel");
        PlayerPrefs.SetInt("Loaded", 0);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        loadingScreen.LoadScene("Asylum");
    }
    /// <summary>
    /// Metoda odpowiedzialna za załadowanie rozgrywki, lub wyświetlenie dialogu informującego o braku pliku zapisu.
    /// </summary>
    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel") && File.Exists(savePath))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            loadingScreen.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }
    /// <summary>
    /// Metoda obsługująca przycisk wyjścia z gry.
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie zadanego ogólnego poziomu głośności gry.
    /// </summary>
    /// <param name="volume"> Wartość ustawianego poziomu głośności.</param>
    public void SetMasterVolume(float volume)
    {
        volume = masterVolumeSlider.value;
        audioMixer.SetFloat("Master", volume);
        volume = Mathf.Round(masterVolumeSlider.value + 80);
        masterVolumeTextValue.text = volume.ToString();
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie zadanego poziomu głośności muzyki gry.
    /// </summary>
    /// <param name="volume"> Wartość ustawianego poziomu głośności.</param>
    public void SetMusicVolume(float volume)
    {
        volume = musicVolumeSlider.value;
        audioMixer.SetFloat("Music", volume);
        volume = Mathf.Round(musicVolumeSlider.value + 80);
        musicVolumeTextValue.text = volume.ToString();
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie zadanego poziomu głośności efektów dźwiękowych gry.
    /// </summary>
    /// <param name="volume"> Wartość ustawianego poziomu głośności.</param>
    public void SetSFXVolume(float volume)
    {
        volume = SFXVolumeSlider.value;
        audioMixer.SetFloat("SFX", volume);
        volume = Mathf.Round(SFXVolumeSlider.value + 80);
        SFXVolumeTextValue.text = volume.ToString();
    }
    /// <summary>
    /// Metoda umożliwiająca lokalne zapisanie wybranych przez użytkownika ustawień dźwiękowych.
    /// </summary>
    public void VolumeApply()
    {
        audioMixer.GetFloat("Master", out master);
        audioMixer.GetFloat("Music", out music);
        audioMixer.GetFloat("SFX", out sfx);
        PlayerPrefs.SetFloat("masterVolume", master);
        PlayerPrefs.SetFloat("musicVolume", music);
        PlayerPrefs.SetFloat("sfxVolume", sfx);
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie zadanego poziomu czułości myszy w grze.
    /// </summary>
    /// <param name="sensitivity"> Wartość ustawianego poziomu czułości myszy.</param>
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = (float)Math.Round(controllerSenSlider.value, 2);
        controllerSenTextValue.text = mainControllerSen.ToString();
    }
    /// <summary>
    /// Metoda umożliwiająca lokalne zapisanie wybranych przed użytkownika ustawień rozgrywki.
    /// </summary>
    public void GameplayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }
        PlayerPrefs.SetFloat("Sens", mainControllerSen);

    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie zadanego poziomu jasności w grze.
    /// </summary>
    public void SetBrightness()
    {
        brightnessLevel = brightnessSlider.value;
        brightnessTextValue.text = Math.Round(brightnessLevel, 2).ToString();
    }
    /// <summary>
    /// Metoda umożliwaiająca ustawienie trybu pełnego ekranu.
    /// </summary>
    public void SetFullScreen()
    {
        _isFullScreen = fullScreenToggle.isOn;
        Screen.fullScreen = _isFullScreen;
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie trybu synchronizacji pionowej.
    /// </summary>
    public void SetVsync()
    {
        isVsync = vSyncToggle.isOn;
        QualitySettings.vSyncCount = isVsync ? 1 : 0;
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie poziomu jakości grafiki w grze.
    /// </summary>
    public void SetQuality()
    {
        qualityLevel = qualityDropdown.value;
    }
    /// <summary>
    /// Metoda umożliwiająca ustawienie wybranej rozdzielczości gry.
    /// </summary>
    public void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    /// <summary>
    /// Metoda umożliwaiająca lokalne zapisanie wybranych przez użytkownika ustawień graficznych.
    /// </summary>
    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", brightnessLevel);
        PlayerPrefs.SetInt("masterQuality", qualityLevel);
        QualitySettings.SetQualityLevel(qualityLevel);
        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        PlayerPrefs.SetInt("vsync", (isVsync ? 1 : 0));
        Screen.fullScreen = _isFullScreen;
        QualitySettings.vSyncCount = isVsync ? 1 : 0;

    }
    /// <summary>
    /// Metoda umożliwiająca zresetowanie ustawień konkretnej kategorii opcji gry do wartości domyślnych.
    /// </summary>
    /// <param name="MenuType"> Kategoria resetowanych opcji.</param>
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            audioMixer.SetFloat("Master", defVolume);
            masterVolumeSlider.value = defVolume;
            masterVolumeTextValue.text = Mathf.Round(defVolume + 80).ToString();
            audioMixer.SetFloat("Music", defVolume);
            musicVolumeSlider.value = defVolume;
            musicVolumeTextValue.text = Mathf.Round(defVolume + 80).ToString();
            audioMixer.SetFloat("SFX", defVolume);
            SFXVolumeSlider.value = defVolume;
            SFXVolumeTextValue.text = Mathf.Round(defVolume + 80).ToString();
            VolumeApply();
        }
        else if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSens.ToString();
            controllerSenSlider.value = defaultSens;
            mainControllerSen = defaultSens;
            invertYToggle.isOn = false;
            GameplayApply();
        }
        else if (MenuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString();
            qualityDropdown.value = 2;
            QualitySettings.SetQualityLevel(2);
            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;
            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

    }
    /// <summary>
    /// Metoda odpowiedzialna za wyświetlenie dialogu potwierdzajacego zapis ustawień konkretnej kategorii opcji.
    /// </summary>
    /// <param name="MenuType"> Kategoria zapisywanych opcji.</param>
    public void ConfirmationBox(string MenuType)
    {
        confirmationPrompt.SetActive(true);
        if (MenuType == "Audio")
        {
            VolumeApply();
        }
        else if (MenuType == "Gameplay")
        {
            GameplayApply();
        }
        else if (MenuType == "Graphics")
        {
            GraphicsApply();
        }
    }

}
