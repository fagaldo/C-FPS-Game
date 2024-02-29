using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/// <summary>
/// Klasa odpowiedzlana za mechanikę przylotu helikoptera w finalnej scenie gry, a także za wyswietlenie ekranu końcowego.
/// </summary>
public class ChopperArrival : MonoBehaviour
{
    /// <summary>
    /// Pole przechowujące canvas wyświetlający dialog umożliwiający wsiądnięcie do helikoptera.
    /// </summary>
    [SerializeField] Canvas getInCanvas;
    /// <summary>
    /// Pole przechowujące canvas wyświetlający ekran końcowy gry.
    /// </summary>
    [SerializeField] Canvas theEndCanvas;
    /// <summary>
    /// Pole przechowujące tablicę canvasów wyłączanych w momencie wsiądnięcia do helikoptera i wyświetlenia ekranu końcowego.
    /// </summary>
    [SerializeField] Canvas[] toDis;
    /// <summary>
    /// Pole przechowujące referencje do obiektu kontrolera cząsteczek, używane do sprawdzenia, czy rozpalone jest ognisko,
    /// w skutek czego rozpoczynana jest animacja lotu helikoptera.
    /// </summary>
    ParticleController checker;
    /// <summary>
    /// Pole przechowujące referencje do źródła dźwięku helikoptera.
    /// </summary>
    AudioSource chopperAudio;
    /// <summary>
    /// Pole przechowujące referencje do pola tekstowego wyświetlającego tekst dialogu przy wsiadaniu do helikoptera.
    /// </summary>
    TextMeshProUGUI getInText;
    /// <summary>
    /// Pole przechowujące referencje do obiektu Animation odpowiedzialnego za animowanie przylotu helikoptera.
    /// </summary>
    private Animation anim;
    /// <summary>
    /// Pole przechowujące informacje, czy helikopter jest w stanie lotu.
    /// </summary>
    private bool isFlying = false;
    /// <summary>
    /// Pole przechowujące informacje, czy input w grze został wyczyszczony, aby prawidłowo wykryć wciskane przez gracza klawisze.
    /// </summary>
    private bool hasCleared = false;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Następuje w niej inicjalizacjia wszystkich wymagających tego pól skryptu.
    /// </summary>
    void Start()
    {
        getInText = getInCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        chopperAudio = GetComponent<AudioSource>();
        anim = GetComponent<Animation>();
        checker = FindObjectOfType<ParticleController>();
        theEndCanvas.enabled = false;
    }
    /// <summary>
    /// Metoda wykonywana co klatkę w grze. Sprawdzany w niej jest stan rozpalenia ogniska i aktywowany jest lot helikoptera.
    /// </summary>
    private void Update()
    {
        if (checker.IsLighten() && !isFlying)
        {
            StartCoroutine(ActivateFlight());
            isFlying = true;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider helikoptera.
    /// W przypadku interakcji gracz może "wsiąść" do helikoptera, tym samym kończąc grę.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            getInCanvas.enabled = true;
            getInText.text = "Press E to get into the choppa";
            if (other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                theEndCanvas.enabled = true;
                foreach (var dis in toDis)
                    dis.enabled = false;
                StartCoroutine(LoadMenuAsync());
            }
        }

    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu helikoptera.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodziła kolizja.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            getInCanvas.enabled = false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu helikoptera.
    /// W przypadku interakcji gracz może "wsiąść" do helikoptera, tym samym kończąc grę.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            theEndCanvas.enabled = true;
            foreach (var dis in toDis)
                dis.enabled = false;
            StartCoroutine(LoadMenuAsync());
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za aktywację animacji lotu helikoptera.
    /// </summary>
    /// <returns> Czeka 10sekund. </returns>
    IEnumerator ActivateFlight()
    {
        yield return new WaitForSeconds(10f);
        anim.Play();
        anim.Blend("Fly");
        chopperAudio.Play();
    }
    /// <summary>
    /// Metoda odpowiedzialna za asynchroniczne załadowanie menu głównego gry po jej ukończeniu.
    /// Oo załadowaniu sceny czyści ona wejście i czeka na wciśnięcie dowolnego przycisku przez gracza w celu kontynuacji,.
    /// </summary>
    /// <returns> Nic.</returns>
    IEnumerator LoadMenuAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        Time.timeScale = 0;
        operation.allowSceneActivation = false;
        Input.ResetInputAxes();
        while (!operation.isDone)
        {
            if (!hasCleared)
            {
                Input.ResetInputAxes();
                hasCleared = true;
            }
            if (Input.anyKey)
                operation.allowSceneActivation = true;
            yield return null;
        }
    }

}
