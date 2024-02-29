using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Klasa odpowiedzialna za przeładowywanie gry, oraz wychodzenie z niej do menu głównego.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Metoda przeładowywująca grę, wywoływana, gdy w momencie śmierci gracz wybierze opcje wczytania ostatniego zapisu.
    /// </summary>
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        FindObjectOfType<SaveLoadSystem>().Load();
        foreach (var gameObj in FindObjectsOfType(typeof(Weapon)) as Weapon[])
        {
            gameObj.canShoot = true;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za opuszczenie przez gracza gry do menu głównego.
    /// </summary>
    public void QuitGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
