using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Klasa odpowiedzialna za wykonywanie odpowiednich zdarzeń powiązanych z animacjami.
/// </summary>
public class EnemyAnimEvents : MonoBehaviour
{
    /// <summary>
    /// Pole przechowujące obiekt klasy zdrowia gracza.
    /// </summary>
    PlayerHealth target;
    /// <summary>
    /// Pole przechowujące obiekt klasy wyświetlającej otrzymane obrażenia.
    /// </summary>
    DisplayDamage displayDmg;
    /// <summary>
    /// Pole przechowujące wartość obrażeń zadawaną przez przeciwnika.
    /// </summary>
    [SerializeField] float damage = 50f;
    /// <summary>
    /// Pole przechowujące wartość odległości przeciwnika do gracza.
    /// </summary>
    private float distanceToTarget = Mathf.Infinity;
    /// <summary>
    /// Pole przechowujące obiekt klasy źródła dźwięku w grze.
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// Lista przechowująca dźwięki wydawane przez przeciwników.
    /// </summary>
    /// <typeparam name="AudioClip"> Konkretny dźwięk wydawany przez przeciwnika. </typeparam>
    [SerializeField] private List<AudioClip> sounds = new List<AudioClip>();
    /// <summary>
    /// Pole przechowujące obiekt klasy odpowiedzialnej za symulacje pola widzenia wrogów.
    /// </summary>
    EnemyFieldOfView enemyFOV;
    /// <summary>
    /// Pole przechowujące obiekt menadżera przerywnika filmowego.
    /// </summary>
    ManageCutscene cutsceneManager;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry.
    /// </summary>
    void Start()
    {
        StartCoroutine(Inicialize());
    }
    /// <summary>
    /// Metoda inicjalizująca wszystkie wymagające tego pola poprzez referencje do innych obiektów w grze.
    /// </summary>
    /// <returns> Czeka aż przerywnik filmowy zostanie zakończony. </returns>
    IEnumerator Inicialize()
    {
        cutsceneManager = FindObjectOfType<ManageCutscene>();
        audioSource = GetComponent<AudioSource>();
        enemyFOV = GetComponentInChildren<EnemyFieldOfView>();
        yield return new WaitUntil(cutsceneManager.HasFinished);
        displayDmg = FindObjectOfType<DisplayDamage>();
        target = FindObjectOfType<PlayerHealth>();
    }
    /// <summary>
    /// Metoda odpowiedzialna za zadanie obrażeń przez przeciwnika w momencie ataku.
    /// </summary>
    public void AttackHitEvent()
    {
        if (!target) return;
        displayDmg.DisplayDmg();
        target.TakeDamage(damage);
    }
    /// <summary>
    /// Metoda odpowiedzialna za wydanie odgłosu ataku przez przeciwnika w momencie ataku.
    /// </summary>
    public void MakeAttackSound()
    {
        audioSource.clip = sounds[0];
        audioSource.Play();
    }
    /// <summary>
    /// Metoda odpowiedzialna za wydanie innego odgłosu ataku przez przeciwnika w momencie ataku.
    /// </summary>
    public void MakeAttackSound2()
    {
        audioSource.clip = sounds[4];
        audioSource.Play();
    }
    /// <summary>
    /// Metoda odpowiedzialna za wydanie odgłosu "jałowego" przez przeciwnika będącego w takim stanie.
    /// </summary>
    public void MakeIdleSound()
    {
        if (cutsceneManager.HasFinished())
        {
            audioSource.clip = sounds[1];
            audioSource.Play();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wydanie odgłosu umierania przez przeciwnika w momencie śmierci.
    /// </summary>
    public void MakeDieSound()
    {
        if (distanceToTarget < 15)
        {
            audioSource.clip = sounds[2];
            audioSource.Play();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wydanie odgłosu kroku przez przeciwnika w momencie biegu w stronę gracza.
    /// </summary>
    public void MakeStepSound()
    {
        distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
        if (enemyFOV.canSeePlayer || distanceToTarget <= 7.5)
        {
            audioSource.clip = sounds[3];
            audioSource.Play();
        }
    }
}
