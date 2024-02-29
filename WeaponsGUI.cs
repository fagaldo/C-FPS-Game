using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
/// <summary>
/// Klasa odpowiedzialna za wyświetlanie na poziomie interfejsu użytkownika ikonek posiadanych broni.
/// Dodatkowo, ikonka aktualnie wyekwipowanej broni posiada wokół siebie czerwoną obwódkę.
/// </summary>
public class WeaponsGUI : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy będącej kontrolerem broni posiadanych przez gracza.
    /// </summary>
    [SerializeField] private WeaponSwitcher weaponController;
    /// <summary>
    /// Pole zawierajace referencje do obrazka symbolizującego pistolet.
    /// </summary>
    private Image pistolImg;
    /// <summary>
    /// Pole zawierające referencje do obwódki pojawiającej się wokół obrazka pistoletu w momencie jego wyekwipowania.
    /// </summary>
    private Outline pistolOutline;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego indeks pistoletu,
    /// tj. taką cyfrę po której wciśnięciu na klawiaturze postać go wyekwipuje.
    /// </summary>
    private TextMeshProUGUI pistolIndex;
    /// <summary>
    /// Pole zawierajace referencje do obrazka symbolizującego strzelbę.
    /// </summary>
    private Image shotgunImg;
    /// <summary>
    /// Pole zawierające referencje do obwódki pojawiającej się wokół obrazka strzelby w momencie jej wyekwipowania.
    /// </summary>
    private Outline shotgunOutline;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego indeks strzelby,
    /// tj. taką cyfrę po której wciśnięciu na klawiaturze postać ją wyekwipuje.
    /// </summary>
    private TextMeshProUGUI shotgunIndex;
    /// <summary>
    /// Pole zawierajace referencje do obrazka symbolizującego karabin.
    /// </summary>
    private Image akmImg;
    /// <summary>
    /// Pole zawierające referencje do obwódki pojawiającej się wokół obrazka karabinu w momencie jego wyekwipowania.
    /// </summary>
    private Outline akmOutline;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego indeks karabinu,
    /// tj. taką cyfrę po której wciśnięciu na klawiaturze postać go wyekwipuje.
    /// </summary>
    private TextMeshProUGUI akmIndex;
    /// <summary>
    /// Pole zawierajace referencje do obrazka symbolizującego siekierę.
    /// </summary>
    private Image axeImg;
    /// <summary>
    /// Pole zawierające referencje do obwódki pojawiającej się wokół obrazka siekiery w momencie jej wyekwipowania.
    /// </summary>
    private Outline axeOutline;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego indeks siekiery,
    /// tj. taką cyfrę po której wciśnięciu na klawiaturze postać ją wyekwipuje.
    /// </summary>
    private TextMeshProUGUI axeIndex;
    /// <summary>
    /// Pole zawierajace referencje do obrazka symbolizującego pięści.
    /// </summary>
    private Image fistsImg;
    /// <summary>
    /// Pole zawierające referencje do obwódki pojawiającej się wokół obrazka pięści w momencie ich wyekwipowania.
    /// </summary>
    private Outline fistsOutline;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego indeks pięści,
    /// tj. taką cyfrę po której wciśnięciu na klawiaturze postać je wyekwipuje.
    /// </summary>
    private TextMeshProUGUI fistsIndex;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce inicjalizacja wszystkich wymagających tego pól.
    /// </summary>
    void Start()
    {
        pistolImg = GameObject.Find("pistol image").GetComponent<Image>();
        pistolOutline = pistolImg.GetComponent<Outline>();
        pistolIndex = GameObject.Find("1").GetComponent<TextMeshProUGUI>();
        pistolImg.enabled = false;
        pistolOutline.enabled = false;
        pistolIndex.enabled = false;
        shotgunImg = GameObject.Find("shotgun image").GetComponent<Image>();
        shotgunOutline = shotgunImg.GetComponent<Outline>();
        shotgunIndex = GameObject.Find("2").GetComponent<TextMeshProUGUI>();
        shotgunImg.enabled = false;
        shotgunOutline.enabled = false;
        shotgunIndex.enabled = false;
        akmImg = GameObject.Find("akm image").GetComponent<Image>();
        akmOutline = akmImg.GetComponent<Outline>();
        akmIndex = GameObject.Find("3").GetComponent<TextMeshProUGUI>();
        akmImg.enabled = false;
        akmOutline.enabled = false;
        akmIndex.enabled = false;
        axeImg = GameObject.Find("axe image").GetComponent<Image>();
        axeOutline = axeImg.GetComponent<Outline>();
        axeIndex = GameObject.Find("4").GetComponent<TextMeshProUGUI>();
        axeImg.enabled = false;
        axeOutline.enabled = false;
        axeIndex.enabled = false;
        fistsImg = GameObject.Find("fists image").GetComponent<Image>();
        fistsOutline = fistsImg.GetComponent<Outline>();
        fistsIndex = GameObject.Find("5").GetComponent<TextMeshProUGUI>();
        fistsImg.enabled = false;
        fistsOutline.enabled = false;
        fistsIndex.enabled = false;
    }
    /// <summary>
    /// Metoda wywoływana co klatkę. Ma w niej miejsce wywołanie dwóch metod,
    /// tj. metody pokazującej obrazki i indeksy posiadanych broni i metody pokazującej obwódkę wokół aktywnej broni.
    /// </summary>
    void Update()
    {
        ShowObtainedWeapons();
        OutlineActiveWeapon();
    }
    /// <summary>
    /// Metoda odpowiedzialna za aktywowanie obrazków i indeksów posiadanych broni.
    /// </summary>
    private void ShowObtainedWeapons()
    {
        foreach (var weapon in weaponController.weaponsObtained)
        {
            if (weapon.Key == 0 && weapon.Value)
                {
                    pistolImg.enabled = true;
                    pistolIndex.enabled = true;
                }
            else if (weapon.Key == 1 && weapon.Value)
                {  
                    shotgunImg.enabled = true;
                    shotgunIndex.enabled = true;
                }
            else if (weapon.Key == 2 && weapon.Value)
                {
                    akmImg.enabled = true;
                    akmIndex.enabled = true;
                }
            else if (weapon.Key == 3 && weapon.Value)
                {
                    axeImg.enabled = true;
                    axeIndex.enabled = true;
                }
            else if (weapon.Key == 4 && weapon.Value)
                {
                    fistsImg.enabled = true;
                    fistsIndex.enabled = true;
                }
        }
    }
    /// <summary>
    /// Metoda odpwiedzialna za aktywowanie obwódki wokół aktualnie wyekwipowanej broni.
    /// </summary>
    private void OutlineActiveWeapon()
    {
        if (weaponController.currentWeapon == 0)
        {
            pistolOutline.enabled = true;
            shotgunOutline.enabled = false;
            akmOutline.enabled = false;
            axeOutline.enabled = false;
            fistsOutline.enabled = false;
        }
        else if (weaponController.currentWeapon == 1)
        {
            pistolOutline.enabled = false;
            shotgunOutline.enabled = true;
            akmOutline.enabled = false;
            axeOutline.enabled = false;
            fistsOutline.enabled = false;
        }
        else if (weaponController.currentWeapon == 2)
        {
            pistolOutline.enabled = false;
            shotgunOutline.enabled = false;
            akmOutline.enabled = true;
            axeOutline.enabled = false;
            fistsOutline.enabled = false;
        }
        else if (weaponController.currentWeapon == 3)
        {
            pistolOutline.enabled = false;
            shotgunOutline.enabled = false;
            akmOutline.enabled = false;
            axeOutline.enabled = true;
            fistsOutline.enabled = false;
        }
        else if (weaponController.currentWeapon == 4)
        {
            pistolOutline.enabled = false;
            shotgunOutline.enabled = false;
            akmOutline.enabled = false;
            axeOutline.enabled = false;
            fistsOutline.enabled = true;
        }
    }
}
