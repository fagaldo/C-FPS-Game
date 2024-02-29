using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
/// <summary>
/// Klasa odpowiedzialna za prawidłowe ładowanie danych poziomów
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do ekranu ładowania pierwszego poziomu.
    /// </summary>
    [SerializeField] GameObject loadingScreen;
    /// <summary>
    /// Pole zawierające referencje do ekranu ładowania drugiego poziomu.
    /// </summary>
    [SerializeField] GameObject tunnelLoadingScreen;
    /// <summary>
    /// Pole zawierające referencje do ekranu ładowania trzeciego poziomu.
    /// </summary>
    [SerializeField] GameObject graveyardLoadingScreen;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego dialog kontynuacji po załadowaniu poziomu.
    /// </summary>
    [SerializeField] TMP_Text continueDialog;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego dialog kontynuacji po załadowaniu drugiego poziomu.
    /// </summary>
    [SerializeField] TMP_Text continueTunnelDialog;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego dialog kontynuacji po załadowaniu trzeciego poziomu.
    /// </summary>
    [SerializeField] TMP_Text continueGraveyardDialog;
    /// <summary>
    /// Pole zawierające referencje do ekranu menu głównego gry.
    /// </summary>
    [SerializeField] GameObject mainScreen;
    /// <summary>
    /// Pole zawierające informacje, czy osie wejściowe w czasie ładowania zostały wyczyszczone. Robione jest to ze względu na 
    /// chęć jak najlepszego i bezbłędnego odczytywania danych wejściuowych.
    /// </summary>
    private bool hasCleared = false;
    /// <summary>
    /// Metoda odpowiedzialna za aktywację odpowiedniego ekranu ładowania, a także uruchomienie korutyny asynchronicznie ładującej dany poziom.
    /// </summary>
    /// <param name="sceneName"> Odpowiada za nazwę sceny, którą gra powinna załadować.</param>

    public void LoadScene(string sceneName)
    {
        mainScreen.SetActive(false);
        if (sceneName == "Asylum")
            loadingScreen.SetActive(true);
        else if (sceneName == "Tunnel")
            tunnelLoadingScreen.SetActive(true);
        else if (sceneName == "Graveyard")
            graveyardLoadingScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    /// <summary>
    /// Metoda odpowiedzialna za asynchroniczne załadowanie sceny. W czasie ładowania wyświetla ona obecny progres, a po
    /// załadowaniu sceny czyści ona wejście i czeka na wciśnięcie dowolnego przycisku przez gracza w celu kontynuacji, 
    /// wyświetlając przy tym stosowny komunikat na ekranie.
    /// </summary>
    /// <param name="sceneName"> Odpowiada za nazwę sceny, którą gra powinna załadować.</param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
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
                    operation.allowSceneActivation = true;

            }
            yield return null;
        }

    }

}
