using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Klasa będąca kontrolerem ekwipunku gracza, której główną funkcją jest obsługa żądania zmiany broni przez gracza.
/// Klasa ta jest singletonem, czyli w aplikacji może istnieć tylko jedna instancja obiektu tej klasy.
/// </summary>
public class WeaponSwitcher : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Statyczne pole odpowiedzialne za zwracanie istniejącej instacji obiektu tej klasy.
    /// </summary>
    /// <value> Obiekt klasy WeaponSwitcher </value>
    public static WeaponSwitcher Instance { get; private set; }
    /// <summary>
    /// Metoda wywoływana przy utworzeniu instancji obiektu z tym skryptem. Upewnia się ona, że istnieć będzie tylko jedna instancja tej klasy
    /// co jest kluczowe dla prawidłowe działania innych mechanik związanych z posiadanymi brońmi.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    /// <summary>
    /// Pole przechowujące indeks aktualnie wyekwipowanej przez gracza broni.
    /// </summary>
    [SerializeField] public int currentWeapon = 0;
    /// <summary>
    /// Pole będące mapą zawierającą informacje o posiadanych przez gracza broniach.
    /// </summary>
    /// <typeparam name="int"> Indeks bronii.</typeparam>
    /// <typeparam name="bool"> Logiczny stan mówiący o tym, czy broń o danym indeksie jest w posiadaniu gracza, czy nie.</typeparam>
    /// <returns></returns>
    [SerializeField] public Dictionary<int, bool> weaponsObtained = new Dictionary<int, bool>();
    /// <summary>
    /// Pole często modyfikowane przez inny skrypt, mówiące nam o tym, czy broń została właśnie podniesiona.
    /// </summary>
    public bool pickedUp = false;
    /// <summary>
    /// Pole informujące o tym, czy gracz przekręcił kółko myszy do góry. Ta informacja jest konieczna przy zamianie broni w oparciu o kółko myszy.
    /// </summary>
    private bool up = false;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce inicjalizaja mapy posiadanych bronii, oraz wywołanie metody ustawiającej aktywną broń.
    /// </summary>
    void Start()
    {
        weaponsObtained.Add(0, false);
        weaponsObtained.Add(1, false);
        weaponsObtained.Add(2, false);
        weaponsObtained.Add(3, false);
        weaponsObtained.Add(4, true);

        SetActiveWeapon();
    }
    /// <summary>
    /// Metoda wywoływana co klatkę. Ma w niej miejsce przetwarzanie sygnałów wejściowych od gracza,
    /// a także wywołanie metody zmieniającej aktywną broń w przypadku sygnału od gracza, który na to wpłynął
    /// </summary>
    void Update()
    {
        int previousWeapon = currentWeapon;
        ProcessKeyInput();
        ProcessScrollWheel();
        if (previousWeapon != currentWeapon || pickedUp)
        {
            SetActiveWeapon();
        }
    }
    /// <summary>
    /// Metoda przetwarzająca sygnały wejściowe w postaci ruszania kółkiem myszy przez gracza.
    /// W jej wyniku zamieniany jest indeks aktywnej broni, a potem jest ona zamieniana w grze.
    /// </summary>
    private void ProcessScrollWheel()
    {
        int tmp = currentWeapon;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            up = true;
            if (currentWeapon >= transform.childCount - 1)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon++;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            up = false;
            if (currentWeapon <= 0)
            {
                currentWeapon = transform.childCount - 1;
            }
            else
            {
                currentWeapon--;
            }
        }
        if (!weaponsObtained[currentWeapon])
        {
            if (!up)
            {
                if (currentWeapon > 0)
                {
                    for (currentWeapon = currentWeapon; currentWeapon > -1; currentWeapon--)
                    {
                        if (weaponsObtained[currentWeapon])
                            return;
                    }
                }
                if (currentWeapon == -1 || currentWeapon == 0)
                {
                    currentWeapon = transform.childCount - 1;
                    if (weaponsObtained[currentWeapon])
                        return;
                    for (currentWeapon = currentWeapon; currentWeapon > -1; currentWeapon--)
                    {
                        if (weaponsObtained[currentWeapon])
                            return;
                    }
                }
                currentWeapon = tmp;
            }
            else if (up)
            {
                if (currentWeapon < transform.childCount - 1)
                {
                    for (currentWeapon = currentWeapon; currentWeapon < transform.childCount; currentWeapon++)
                    {
                        if (weaponsObtained[currentWeapon])
                            return;
                    }
                }
                if (currentWeapon == transform.childCount || currentWeapon == transform.childCount - 1)
                {
                    currentWeapon = 0;
                    if (weaponsObtained[currentWeapon])
                        return;
                    for (currentWeapon = currentWeapon; currentWeapon < transform.childCount; currentWeapon++)
                    {
                        if (weaponsObtained[currentWeapon])
                            return;
                    }

                }
                currentWeapon = tmp;
            }

        }
    }
    /// <summary>
    /// Metoda przetwarzająca sygnały wejściowe w postaci wciskania klawiszy numerycznych (od 1 do 5), przez gracza.
    /// W jej wyniku zamieniany jest indeks aktywnej broni, a potem jest ona zamieniana w grze.
    /// </summary>
    private void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponsObtained[0])
        {
            currentWeapon = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponsObtained[1])
        {
            currentWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weaponsObtained[2])
        {
            currentWeapon = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && weaponsObtained[3])
        {
            currentWeapon = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && weaponsObtained[4])
        {
            currentWeapon = 4;
        }

    }
    /// <summary>
    /// Metoda ustawiająca aktywną broń w grze na podstawie wcześniej ustawionego indeksu aktywnej broni.
    /// </summary>
    private void SetActiveWeapon()
    {
        int weaponIndex = 0;
        foreach (Transform weapon in transform)
        {
            if (weaponIndex == currentWeapon)
            {
                weapon.gameObject.SetActive(true);
                pickedUp = false;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            weaponIndex++;
        }
        pickedUp = false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pola z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pola które zostają zapisane w pliku. </returns>
    public object SaveState()
    {
        return new SaveData()
        {
            currentWeapon = this.currentWeapon,
            weaponsObtained = this.weaponsObtained
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
        weaponsObtained = saveData.weaponsObtained;
        currentWeapon = saveData.currentWeapon;
        SetActiveWeapon();
    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public int currentWeapon;
        public Dictionary<int, bool> weaponsObtained;
    }
}
