using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Klasa pełniąca funkcję kontrolera broni bliskiego zasięgi, tj. pięści, oraz siekiery.
/// </summary>
public class MeleeWeapon : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do kamery gracza.
    /// </summary>
    [SerializeField] Camera FPSCamera;
    /// <summary>
    /// Pole określające odległość na jaką można zaatakować.
    /// </summary>
    [SerializeField] float range = 10f;
    /// <summary>
    /// Pole określające obrażenia zadawane przez broń.
    /// </summary>
    [SerializeField] float damage = 10f;
    /// <summary>
    /// Pole przechowujące tablicę obiektów będącymi efektami cząsteczkowymi uderzeń w różne powierzchnie.
    /// </summary>
    [SerializeField] GameObject[] hitEffects;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego ilość amunicji danej broni.
    /// </summary>
    [SerializeField] TextMeshProUGUI ammoText;
    /// <summary>
    /// Lista dźwięków wydawanych przez bronie.
    /// </summary>
    /// <typeparam name="AudioClip"> Konkretny dźwięk wydawany przez daną broń.</typeparam>
    [SerializeField] private List<AudioClip> sounds = new List<AudioClip>();
    /// <summary>
    /// Pole określające, czy wyekwipowaną przez gracza bronią krótkiego zasięgu są pięści.
    /// </summary>
    [SerializeField] bool isFist = true;
    /// <summary>
    /// Pole zawierające referencje do kontrolera animacji prawej dłoni.
    /// </summary>
    HandAnimatorManagerRight animatorController;
    /// <summary>
    /// Pole zawierajace referencje do obiektu będącego źródłem dźwięku wydawanego przy ataku.
    /// </summary>
    private AudioSource audioo;
    /// <summary>
    /// Pole określające szybkość ataku daną bronią.
    /// </summary>
    public float fireRate = 10f;
    /// <summary>
    /// Pole służące do obliczenia czasu w którym powinien zostać wykonany kolejny atak.
    /// </summary>
    private float nextTimeToFire = 0f;
    /// <summary>
    /// Pole określające która pięść powinna atakować w danym momencie w przypadku wyekwipowanych pięści.
    /// </summary>
    private bool isLeft = false;
    /// <summary>
    /// Metoda wywoływana w momencie aktywacji skryptu.
    /// </summary>
    private void OnEnable()
    {
        ammoText.text = "- | -";
    }
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Następuje w niej inicjalizacjia wszystkich wymagających tego pól skryptu.
    /// </summary>
    void Start()
    {
        audioo = GetComponent<AudioSource>();
        sounds.Add(audioo.clip);
        animatorController = FindObjectOfType<HandAnimatorManagerRight>();
    }
    /// <summary>
    /// Metoda wywoływana co klatkę w grze. Obsługuję ona wywoływanie funkcji ataku w przypadku przyciśnięcia przez gracza lewego przycisku myszy,
    /// a także oblicza czas do następnego możliwego ataku.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Attack();
            StartCoroutine(WaitForSound(2));
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za przeprowadzenie ataku. W przypadku pięści wywołuje ona również korutynę zmieniającą pięść atakującą.
    /// </summary>
    private void Attack()
    {
        ProcessRayCast();
        if (isFist)
            StartCoroutine(ChangeFist());
    }
    /// <summary>
    /// Metoda za pomocą której obsługiwana jest mechanika strzału, a w tym przypadku ataku bronią krótkodystansową.
    /// </summary>
    private void ProcessRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out hit, range))
        {

            float damageTmp = damage;
            if (hit.collider is BoxCollider)
            {
                damage = 100f;
            }
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target)
            {
                StartCoroutine(WaitForSound(0));
                target.TakeDamage(damage);
                CreateHitImpact(hit, hitEffects[0]);
            }
            else
            {
                StartCoroutine(WaitForSound(1));
                CreateHitImpact(hit, hitEffects[1]);
            }

            damage = damageTmp;
        }
        else return;
    }
    /// <summary>
    /// Metoda, która w przypadku wyekwipowanych pięści zamienia aktualnie używaną do ataku pięść na drugą.
    /// </summary>
    /// <returns> Czeka 2 sekundy aż animacja ataku się wykona.</returns>
    IEnumerator ChangeFist()
    {
        yield return new WaitForSeconds(2.0f);
        if (!isLeft)
        {
            animatorController.isLeft = true;
            isLeft = true;
        }
        else
        {
            animatorController.isLeft = false;
            isLeft = false;
        }
    }
    /// <summary>
    /// Metoda zwracająca informacje, czy efekt dźwiękowy związany z atakiem może zostać odtworzony.
    /// </summary>
    /// <returns> Informację logiczną, czy efekt dźwiękowy związany z atakiem może zostać odtworzony.</returns>
    private bool CanPlay()
    {
        if (!audioo.isPlaying)
            return true;
        else
            return false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za odtworzenie dźwięku związanego z atakiem.
    /// </summary>
    /// <param name="whichEffect"> Określa jaki efekt dźwiękowy powinien być odtworzony.</param>
    /// <returns></returns>
    IEnumerator WaitForSound(int whichEffect)
    {
        yield return CanPlay();
        audioo.PlayOneShot(sounds[whichEffect]);
    }
    /// <summary>
    /// Metoda odpowiedzialna za odtworzenie efektu cząsteczkowego związanego z uderzeniem bronią w konkretną powierzchnię.
    /// </summary>
    /// <param name="hit"> Obiekt zawierający informacje o trafionym obiekcie.</param>
    /// <param name="hitEffect"> Obiekt zawierający efekt cząsteczkowy, który powinien zostać odtworzony.</param>
    private void CreateHitImpact(RaycastHit hit, GameObject hitEffect)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, .1f);
    }
}
