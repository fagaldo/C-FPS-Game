using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Klasa odpowiedzialna za działanie menu pauzy w grze.
/// </summary>
public class PauseMenuHandler : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do canvasu wyświetlającego menu pauzy w grze.
    /// </summary>
    [SerializeField] Canvas pauseMenuCanvas;
    /// <summary>
    /// Pole zawierające referencje do canvasu wyświetlającego celownik w grze.
    /// </summary>
    [SerializeField] Canvas recticleCanvas;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy odpowiedzialnego za mechanikę punktów życia postaci.
    /// </summary>
    [SerializeField] PlayerHealth playerHealth;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry.
    /// </summary>
    void Start()
    {
        pauseMenuCanvas.enabled = false;
    }
    /// <summary>
    /// Metoda wywoływana co klatkę.
    /// Ma w niej miejscę skanowanie wejść w celu wykrycia chęci gracza do włączenia menu pauzy, lub wyłączenia.
    /// W przypadku wykrycia takiego zamiaru wywoływane są metody to obsługujące.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenuCanvas.isActiveAndEnabled && playerHealth.GetHealth() > 0)
            OpenMenu();
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenuCanvas.isActiveAndEnabled)
            ResumeGame();

    }
    /// <summary>
    /// Metoda odpowiedzialna za włączenie menu pauzy w grze. Wyłącza ona wszystkie przeszkadzające elementy interfejsu,
    /// a także aktywuje kursor myszy.
    /// </summary>
    private void OpenMenu()
    {
        recticleCanvas.enabled = false;
        pauseMenuCanvas.enabled = true;
        Time.timeScale = 0;
        if (FindObjectOfType<WeaponSwitcher>())
            FindObjectOfType<WeaponSwitcher>().enabled = false;
        foreach (var gameObj in FindObjectsOfType(typeof(Weapon)) as Weapon[])
        {
            gameObj.canShoot = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    /// <summary>
    /// Metoda odpowiedzialna za wyjście z menu pauzy w grze. Włącza ona wszystkie wcześniej wyłączone elementy interfejsu,
    /// a także chowa kursor myszy.
    /// </summary>
    public void ResumeGame()
    {
        recticleCanvas.enabled = true;
        pauseMenuCanvas.enabled = false;
        Time.timeScale = 1;
        if (FindObjectOfType<WeaponSwitcher>())
            FindObjectOfType<WeaponSwitcher>().enabled = true;
        foreach (var gameObj in FindObjectsOfType(typeof(Weapon)) as Weapon[])
        {
            gameObj.canShoot = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    /// <summary>
    /// Metoda odpowiedzialna za powrót z poziomu gry do menu głównego.
    /// </summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
