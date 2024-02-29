using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
/// <summary>
/// Klasa odpowiedzialna za mechanikę strzelania z broni.
/// </summary>
public class Weapon : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole zawierające referencje do kamery gracza.
    /// </summary>
    [SerializeField] Camera FPSCamera;
    /// <summary>
    /// Pole zawierające wartość maksymalnej odległości na jaką broń może strzelać.
    /// </summary>
    [SerializeField] float range = 100f;
    /// <summary>
    /// Pole zawierające wartość obrażeń zadawanych przez bron.
    /// </summary>
    [SerializeField] float damage = 10f;
    /// <summary>
    /// Pole zawierające referencje do obiektu efektów cząsteczkowych towarzyszących wystrzałowi z broni.
    /// </summary>
    [SerializeField] ParticleSystem muzzleFlash;
    /// <summary>
    /// Tablica zawierająca obiekty efektów cząsteczkowych pojawiających się po kontakcie pocisku z powierzchnią.
    /// </summary>
    [SerializeField] GameObject[] hitEffects;
    /// <summary>
    /// Pole zawierajace referencje do obiektu będącego graficznym odwzrowaniem dziury po kuli pojawiającej się po kontakcie z konkretnymi powierzchniami.
    /// </summary>
    [SerializeField] GameObject bulletHole;
    /// <summary>
    /// Pole zawierajace referencje do obiektu klasy odpowiedzialnej za system amunicji.
    /// </summary>
    [SerializeField] Ammo ammo;
    /// <summary>
    /// Pole będące reprezentacją typu wyliczeniowego określającego typ amunicji.
    /// </summary>
    [SerializeField] AmmoType ammoType;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego obecny na dany moment stan amunicji broni.
    /// </summary>
    [SerializeField] TextMeshProUGUI ammoText;
    /// <summary>
    /// Pole zawierajace informacje o rozmiarze magazynka broni.
    /// </summary>
    [SerializeField] int magazineSize = 5;
    /// <summary>
    /// Lista dźwięków wydawanych przez bronie.
    /// </summary>
    /// <typeparam name="AudioClip"> Konkretny dźwięk wydawany przez daną broń.</typeparam>
    [SerializeField] private List<AudioClip> sounds = new List<AudioClip>();
    /// <summary>
    /// Pole określające rozrzut broni w osi X przy strzelaniu z biodra, bez trybu celowania.
    /// </summary>
    [SerializeField] private float recoilX;
    /// <summary>
    /// Pole określające rozrzut broni w osi Y przy strzelaniu z biodra, bez trybu celowania.
    /// </summary>
    [SerializeField] private float recoilY;
    /// <summary>
    /// Pole określające rozrzut broni w osi Z przy strzelaniu z biodra, bez trybu celowania.
    /// </summary>
    [SerializeField] private float recoilZ;
    /// <summary>
    /// Pole określające rozrzut broni w osi X przy strzelaniu w trybie celowania.
    /// </summary>
    [SerializeField] private float aimRecoilX;
    /// <summary>
    /// Pole określające rozrzut broni w osi Y przy strzelaniu w trybie celowania.
    /// </summary>
    [SerializeField] private float aimRecoilY;
    /// <summary>
    /// Pole określające rozrzut broni w osi Z przy strzelaniu w trybie celowania.
    /// </summary>
    [SerializeField] private float aimRecoilZ;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy obsługującego mechanikę celowania.
    /// </summary>
    private WeaponZoom weaponZoom;
    /// <summary>
    /// Pole zawierające informacje o pozostałej liczbie naboi w magazynku.
    /// </summary>
    private int magazineAmount;
    /// <summary>
    /// Pole zawierające referencje do obiektu emitującego dźwięki broni.
    /// </summary>
    private AudioSource audioo;
    /// <summary>
    /// Pole zawierające referencje do obiektu będącego efektem dźwiękowym wystrzału.
    /// </summary>
    private AudioClip fire;
    /// <summary>
    /// Pole zawierające wartość określającą z jaką częstotliwością broń może strzelać.
    /// </summary>
    public float fireRate = 10f;
    /// <summary>
    /// Pole zaierające wartość używaną do obliczenia czasu w którym można oddać kolejny strzał.
    /// </summary>
    private float nextTimeToFire = 0f;
    /// <summary>
    /// Pole określające czy w danym momencie można strzelać z broni.
    /// </summary>
    public bool canShoot = true;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy odpowiedzialnej za mechanikę rozrzutu pocisków.
    /// </summary>
    Recoil recoilScript;
    /// <summary>
    /// Tablica zawierająca obiekty wrogów, które mogą zostać sprowokowane po usłyszeniu dźwięku wystrzału.
    /// </summary>
    EnemyAI[] enemies;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce inicjalizacja wszystkich wymagających tego pól, a także napełnienie magazynka amunicją.
    /// </summary>
    private void Start()
    {
        enemies = FindObjectsOfType<EnemyAI>();
        audioo = GetComponent<AudioSource>();
        weaponZoom = GetComponent<WeaponZoom>();
        if (magazineAmount == 0 && ammo.GetCurrentAmount(ammoType) > 0)
        {
            for (int i = 0; i < magazineSize; i++)
            {
                ammo.DecreaseAmount(ammoType);
                magazineAmount++;
                if (ammo.GetCurrentAmount(ammoType) == 0)
                    break;
            }
        }
        recoilScript = GetComponentInParent<Recoil>();
        fire = audioo.clip;
    }
    /// <summary>
    /// Metoda wywoływana w momencie wyłączenia obiektu.
    /// </summary>
    private void OnDisable()
    {
        audioo.clip = fire;
        if (!canShoot)
            canShoot = true;
    }
    /// <summary>
    /// Metoda wywoływana co klatkę, w odpowiedzi na działania gracza ma w niej miejsce wywoływanie funkcji odpowiedzialnych za strzał, przeładowanie (w tym animacji z tym związanych)
    /// i wydawanie dźwięków broni.
    /// </summary>
    void Update()
    {
        DisplayAmmo();
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire && canShoot && magazineAmount > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            audioo.Play();
        }
        if (Input.GetKeyDown(KeyCode.R) && magazineAmount < magazineSize && ammo.GetCurrentAmount(ammoType) > 0 && canShoot)
        {
            canShoot = false;
            audioo.PlayOneShot(sounds[1]);
            GetComponentInParent<Animator>().SetTrigger("Reload");
            if (ammoType.ToString() == "Light")
                FindObjectOfType<HandAnimatorManager>().handAnimator.SetTrigger("Reload");
            GetComponentInParent<Animator>().ResetTrigger("Idle");
            StartCoroutine(EndReload());
        }
        if (magazineAmount == 0 && Input.GetMouseButton(0) && Time.time >= nextTimeToFire && canShoot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            audioo.clip = sounds[0];
            audioo.Play();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wykonanie przeładowania broni w określonym czasie z uwagi na czas animacji przeładowania.
    /// W momencie przeładowania zmniejsza ona ilość posiadanej amunicji.
    /// </summary>
    /// <returns> Czeka 4sekundy aż będzie mogła zakończyć proces przeładowania.</returns>
    IEnumerator EndReload()
    {
        yield return new WaitForSeconds(4f);
        GetComponentInParent<Animator>().ResetTrigger("Reload");
        GetComponentInParent<Animator>().SetTrigger("Idle");
        int tmp = magazineSize - magazineAmount;
        if (ammo.GetCurrentAmount(ammoType) >= tmp)
        {
            magazineAmount += tmp;
            for (int i = 0; i < tmp; i++)
                ammo.DecreaseAmount(ammoType);
        }
        else
        {
            int tmp2 = ammo.GetCurrentAmount(ammoType);
            for (int i = 0; i < tmp2; i++)
            {
                magazineAmount++;
                ammo.DecreaseAmount(ammoType);
            }
        }
        canShoot = true;
        audioo.clip = fire;
    }
    /// <summary>
    /// Metoda odpowiedzialna za wyświetlanie obecnego stanu amunicji broni.
    /// </summary>
    private void DisplayAmmo()
    {
        ammoText.text = magazineAmount + " | " + ammo.GetCurrentAmount(ammoType).ToString();
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki wystrzału pocisków z bronii (oraz animacji z tym związanych) i rozrzutu pocisków, poprzez wywołanie odpowiednich metod.
    /// </summary>
    private void Shoot()
    {
        PlayMuzzleFlash();
        if (ammoType.ToString() == "Shells")
            ProcessShotgunRayCast();
        else
            ProcessRayCast();
        GetComponentInParent<Animator>().SetTrigger("Shot");
        TauntEnemies();
        if (weaponZoom.isZoomed)
            recoilScript.RecoilFire(aimRecoilX, aimRecoilY, aimRecoilZ);
        else
            recoilScript.RecoilFire(recoilX, recoilY, recoilZ);
        magazineAmount--;
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki wystrzału pocisków z karabinu, oraz pistoletu.
    /// </summary>
    private void ProcessRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out hit, range))
        {
            float damageTmp = damage;
            if (hit.collider is BoxCollider) // trafienie w głowę przeciwnika
            {
                damage = 100f;
            }
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target) // trafienie w ciało przeciwnika
            {
                target.TakeDamage(damage);
                CreateHitImpact(hit, hitEffects[0]);
            }
            else
            {
                CreateHitImpact(hit, hitEffects[1]);
            }
            damage = damageTmp;

        }
        else return;
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki wystrzału pocisków ze strzelby uwzględniająca specyfikę strzelania z tego rodzaju bronii.
    /// </summary>
    private void ProcessShotgunRayCast()
    {
        var pellets = 8;
        var rays = new Ray[pellets];
        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = FPSCamera.transform.forward; // początkowy cel
            Vector3 spread = Vector3.zero;
            spread += FPSCamera.transform.up * UnityEngine.Random.Range(-1f, 1f); // dodanie losowego rozrzutu w ponie (może być ujemny)
            spread += FPSCamera.transform.right * UnityEngine.Random.Range(-1f, 1f); // dodanie losowego rozrzutu w poziomie
            direction += spread.normalized * UnityEngine.Random.Range(0f, 0.2f);
            rays[i] = new Ray(FPSCamera.transform.position, direction);
        }
        RaycastHit hit;
        foreach (var ray in rays) // dla każdego pocisku osobna analiza promienia
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, range))
            {
                float damageTmp = damage;
                if (hit.collider is BoxCollider)
                {
                    damage = 100f;
                }
                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target)
                {
                    target.TakeDamage(damage);
                    CreateHitImpact(hit, hitEffects[0]);
                }
                else
                {
                    CreateHitImpact(hit, hitEffects[1]);
                }
                damage = damageTmp;
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wywołanie efektu prowokowania wrogów w wyniku efektu dźwiękowego wystrzału.
    /// </summary>
    private void TauntEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.OnShotsFired();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za odtworzenie efektu cząsteczkowego wystrzału z broni.
    /// </summary>
    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }
    /// <summary>
    /// Metoda odpowiedzialna za stworzenie efektu graficznego po kontakcie kuli z daną powierzchnią.
    /// </summary>
    /// <param name="hit"> Obiekt zawierający informacje o trafionym punkcie.</param>
    /// <param name="hitEffect"> Obiekt zawierający referencję do efektu cząsteczkowego towarzyszącemu kontaktowi kuli z powierzchnią.</param>
    private void CreateHitImpact(RaycastHit hit, GameObject hitEffect)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        if (hit.transform.gameObject.layer == 6 || hit.transform.gameObject.layer == 7)
        {
            GameObject hole = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
            hole.transform.position += hole.transform.forward / 1000;
            Destroy(hole, 5f);
        }
        Destroy(impact, .1f);

    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pole z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            magazineAmount = this.magazineAmount
        };
    }
    /// <summary>
    /// Metoda odpowiedzialna za wczytanie wcześniej zapisanego stanu obiektu obsługiwanego przez skrypt i wykonanie
    /// operacji mających na celu przywrócenie obiektu do stanu zapisanego.
    /// </summary>
    /// <param name="state"> Obiekt przechowujący zapisany stan pól ze skryptów gry.</param>
    public void LoadState(object state)
    {
        var saveData = (SaveData)state;
        this.magazineAmount = saveData.magazineAmount;
    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public int magazineAmount;
    }
}
