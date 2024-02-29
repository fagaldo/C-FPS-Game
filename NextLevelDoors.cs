using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using MovementController;
/// <summary>
/// Klasa odpowiedzialna za mechanikę interakcji z drzwiami po przejściu przez które następuje przejście z pierszego do drugiego poziomu.
/// </summary>
public class NextLevelDoors : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencję do canvasu wyświetlającego dialog przejścia przez drzwi i ukończenia pierwszego poziomu.
    /// </summary>
    [SerializeField] Canvas displayTextCanvas;
    /// <summary>
    /// Pole zawierające referencje do obiektu będącego ekranem ładowania drugiego poziomu.
    /// </summary>
    [SerializeField] GameObject loadingScreen;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego dialog przejścia przez drzwi i ukończenia pierwszego poziomu.
    /// </summary>
    TextMeshProUGUI nextLevelDialog;
    /// <summary>
    /// Pole zawierajace referencje do obiektu klasy odpowiedzialnej za system zapisu.
    /// </summary>
    [SerializeField] SaveLoadSystem saveIt;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlanego na ekranie ładowania trzeciego poziomu.
    /// </summary>
    [SerializeField] TMP_Text continueDialog;
    /// <summary>
    /// Tablica zawierające elementy, które są deaktywowane w momencie podjęcia interakcji z drzwiami na końcu pierwszego poziomu, czyli przejścia do drugiego.
    /// </summary>
    [SerializeField] Canvas[] toDisable;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy będącej kontrolerem mechaniki ruchu postaci gracza.
    /// </summary>
    private PlayerController player;
    /// <summary>
    /// Pole przechowujące informacje, czy input w grze został wyczyszczony, aby prawidłowo wykryć wciskane przez gracza klawisze.
    /// </summary>
    private bool hasCleared = false;
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider drzwi na końcu pierwszego poziomu.
    /// W momencie interakcji gracza z drzwiami wywoływana jest korutyna ładująca kolejny poziom.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            displayTextCanvas.enabled = true;
            nextLevelDialog = displayTextCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            nextLevelDialog.text = "Press E to exit this level";
            Input.ResetInputAxes();
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(LoadSceneAsync("Tunnel"));
                loadingScreen.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider drzwi na końcu pierwszego poziomu.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodziła kolizja.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            displayTextCanvas.enabled = false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider drzwi na końcu pierwszego poziomu.
    /// W momencie interakcji gracza z drzwiami wywoływana jest korutyna ładująca kolejny poziom.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(LoadSceneAsync("Tunnel"));
                foreach (var dis in toDisable)
                {
                    dis.enabled = false;
                }
                loadingScreen.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za asynchroniczne załadowanie sceny. W czasie ładowania wyświetla ona obecny progres, a po
    /// załadowaniu sceny czyści ona input i czeka na wciśnięcie dowolnego przycisku przez gracza w celu kontynuacji, 
    /// wyświetlając przy tym stosowny komunikat na ekranie.
    /// </summary>
    /// <param name="sceneName"> Odpowiada za nazwę sceny, którą gra powinna załadować.</param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 0;
        operation.allowSceneActivation = false;
        player = FindObjectOfType<PlayerController>();
        player.transform.position = new Vector3(-16f, 1f, -25f);
        saveIt.Save(sceneName);
        Input.ResetInputAxes();
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            continueDialog.text = "Loading progress: " + progressValue * 100 + "%";
            if (operation.progress >= 0.9f)
            {
                if (!hasCleared)
                {
                    Input.ResetInputAxes();
                    hasCleared = true;
                }
                continueDialog.text = "Press any key to continue";
                if (Input.anyKey)
                {
                    Time.timeScale = 1;
                    operation.allowSceneActivation = true;
                    foreach (var dis in toDisable)
                    {
                        dis.enabled = true;
                    }
                }
            }
            yield return null;
        }
    }
}
