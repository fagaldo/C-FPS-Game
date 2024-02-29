using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MovementController;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
/// <summary>
/// Metoda odpowiedzialna za najważniejsze funkcjonalności systemu zapisu i wczytywania.
/// </summary>
public class SaveLoadSystem : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy kontrolera menu opcji.
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
    /// Pole zawierające referencje do przełącznika, którym można ustawiać odwróconą oś Y sterowania kamerą gracza.
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
    /// Pole zawierającego referencje do obrazka wyświetlanego w momencie udanego zapisu stanu gry.
    /// </summary>
    [SerializeField] Image savedImg = null;
    /// <summary>
    /// Pola zawierające wartości głośności: ogólnej, muzyki, oraz efektów dźwiękowych.
    /// </summary>
    float master, music, sfx;
    /// <summary>
    /// Pole zawierające referencje do kontrolera ruchu gracza.
    /// </summary>
    PlayerController playerController = null;
    /// <summary>
    /// Pole zawierające referencje do obiektu emitującego światło na scenie.
    /// </summary>
    [SerializeField] Light light = null;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy będącego kontrolerem przerywników filmowych.
    /// </summary>
    ManageCutscene sceneController = null;
    /// <summary>
    /// Pole określające ścieżkę pod którą zapisywane są pliki zapisu.
    /// </summary>
    /// <value> Ścieżka do zapisu.</value>
    public string savePath => $"{Application.persistentDataPath}/save.txt";
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Ma w niej miejsce inicjalizacja pola,
    /// a także wywłanie metody inicjalizującej po czasie.
    /// </summary>
    private void Start()
    {
        savedImg.enabled = false;
        StartCoroutine(LoadLater());
    }
    /// <summary>
    /// Metoda inicjalizująca wszystkie wymagające tego pola poprzez podpięcie referencji do innych obiektów po zakoczeniu przerywnika filmowego.
    /// </summary>
    /// <returns> Czeka aż przerywnik filmowy zostanie zakończony. </returns>
    IEnumerator LoadLater()
    {
        sceneController = FindObjectOfType<ManageCutscene>();
        yield return new WaitUntil(sceneController.HasFinished);
        playerController = FindObjectOfType<PlayerController>();
        LoadEverything();
    }
    /// <summary>
    /// Metoda odpowiedzialna za załadowanie do gry zapisanych wcześniej, preferowanych przez użytkownika ustawień. Ustawienia ładowane są do logiki gry,
    /// a także do poszczególnych elementów menu opcji gry.
    /// </summary>
    public void LoadSettings()
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
            if (PlayerPrefs.GetString("SavedLEvel") == "Graveyard")
                light.intensity = tmp + 0.1f;
            else
                light.intensity = tmp;
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
            {
                if (playerController)
                {
                    invertYToggle.isOn = true;
                    playerController.flipY = true;
                }
            }
            else
            {
                if (playerController)
                {
                    invertYToggle.isOn = false;
                    playerController.flipY = false;
                }
            }
        }
        if (PlayerPrefs.HasKey("Sens"))
        {
            float tmp = PlayerPrefs.GetFloat("Sens");
            controllerSenSlider.value = tmp;
            controllerSenTextValue.text = tmp.ToString();
            menuController.mainControllerSen = tmp;
            if (playerController)
                playerController.lookSpeed = tmp;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wywołanie metody ładującej ustawienia, a także w przypadku zapisanego stanu gry, samej gry.
    /// </summary>
    private void LoadEverything()
    {
        LoadSettings();
        if (PlayerPrefs.HasKey("SavedLevel"))
            Load();
    }
    /// <summary>
    /// Metoda wysokopoziomowa odpowiedzialna za funkcjonalność zapisu stanu gry, wywołuje kolejne metody odpowiedzialna za tę funkcjonalność.
    /// </summary>
    /// <param name="scena"> Określa na jakim poziomie znajduje się gracz zapisujący grę.</param>
    [ContextMenu("Save")]
    public void Save(string scena = "abc")
    {
        sceneController = FindObjectOfType<ManageCutscene>();

        if (sceneController.HasFinished() && scena == "abc")
        {
            PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt("Loaded", 1);
            var state = LoadFile();
            SaveState(state);
            SaveFile(state);
        }
        else if (sceneController.HasFinished() && scena != "abc")
        {
            PlayerPrefs.SetString("SavedLevel", scena);
            PlayerPrefs.SetInt("Loaded", 0);
            var state = LoadFile();
            SaveState(state);
            SaveFile(state);
        }
        savedImg.enabled = true;
        StartCoroutine(DisableSavedImage());
    }
    /// <summary>
    /// Metoda odpowiedzialna za wygaszenie obrazka symbolizującego zapis stanu rozgrywki.
    /// </summary>
    /// <returns> Czeka 2.5sekundy.</returns>
    IEnumerator DisableSavedImage()
    {
        yield return new WaitForSeconds(2.5f);
        savedImg.enabled = false;
    }
    /// <summary>
    /// Metoda wysokopoziomowa odpowiedzialna za wczytanie stanu zapisu rozgrywki, wywołuje kolejne metody odpowiedzialne za tę funkcjonalność.
    /// </summary>
    [ContextMenu("Load")]
    public void Load()
    {
        if (PlayerPrefs.HasKey("SavedLevel") && sceneController.HasFinished())
        {
            var state = LoadFile();
            LoadState(state);
        }
    }
    /// <summary>
    /// Metoda niskopoziomowa odpowiedzialna za prawidłowe zapisanie pliku zawierającego zapisany stan gry za pomocą binarnego formatowania.
    /// </summary>
    /// <param name="state"> Obiekt zawierający zapisany stan gry.</param>
    void SaveFile(object state)
    {
        using (var stream = File.Open(savePath, FileMode.Create))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, state);

        }
    }
    /// <summary>
    /// Metoda niskopoziomowa odpowiedzialna za prawidłowe odczytanie pliku z zapisanym stanem rozgrywki za pomocą deserializacji.
    /// </summary>
    /// <returns> Mapę zawierającą zapisane dane dla odpowiednich obiektów.</returns>
    Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(savePath))
        {
            return new Dictionary<string, object>();
        }
        using (var stream = File.Open(savePath, FileMode.Open))
        {
            var formatter = new BinaryFormatter();
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }

    }
    /// <summary>
    /// Metoda odpowiedzialna za zapisanie stanu rozgrywki. Wpisuje ona do mapy odpowiednie wartości uzyskiwane z kolejnych obiektów zapisujących swoje dane.
    /// </summary>
    /// <param name="state"> Obiekt mapy do którego zapisywane są dane dla odpowiednich obiektów.</param>
    void SaveState(Dictionary<string, object> state)
    {
        foreach (var saveable in Resources.FindObjectsOfTypeAll<SaveableEntity>())
        {
            state[saveable.Id] = saveable.SaveState();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wczytanie stanu rozgrywki. Odczytuje ona z mapy odpowiednie wartości pól zapisanych przez dane obiekty.
    /// </summary>
    /// <param name="state"> Mapa zawierająca zapisane dane dla odpowiednich obiektów.</param>
    void LoadState(Dictionary<string, object> state)
    {
        foreach (var saveable in Resources.FindObjectsOfTypeAll<SaveableEntity>())
        {
            if (state.TryGetValue(saveable.Id, out object savedState))
            {
                saveable.LoadState(savedState);
            }
        }
    }

}
