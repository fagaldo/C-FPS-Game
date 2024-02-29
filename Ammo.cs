using System;
using UnityEngine;
/// <summary>
/// Klasa obsługująca mechanikę amunicji.
/// </summary>
public class Ammo : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Podklasa definiująca używany typ amunicji, oraz jej ilość. Można ją określić jako "paczkę" na amunicję.
    /// </summary>
    [System.Serializable]
    private class AmmoSlot
    {
        /// <summary>
        /// Pole określające typ używanej amunicji.
        /// </summary>
        public AmmoType ammoType;
        /// <summary>
        /// Pole przechowujące informacje o pozostałej ilości używanej amunicji.
        /// </summary>
        public int ammoAmount;
    }
    /// <summary>
    /// Pole przechowujące tablice wszystkich posiadanych "paczek" różnych amunicji.
    /// </summary>
    [SerializeField] AmmoSlot[] ammoSlots;
    /// <summary>
    /// Metoda zwracająca ilość aktualnie używanej amunicji
    /// </summary>
    /// <param name="ammoType"> Określa typ aktualnie używanej amunicji.</param>
    /// <returns > Ilość aktualnie używanej amunicji.</returns>
    public int GetCurrentAmount(AmmoType ammoType)
    {
        return GetAmmoSlot(ammoType).ammoAmount;
    }
    /// <summary>
    /// Metoda odpowiedzialna za dekrementację ilości posiadanej amunicji w momencie strzelania.
    /// </summary>
    /// <param name="ammoType"> Określa typ aktualnie używanej amunicji.</param>
    public void DecreaseAmount(AmmoType ammoType)
    {
        GetAmmoSlot(ammoType).ammoAmount--;
    }
    /// <summary>
    /// Metoda odpowiedzialna za zwiększanie ilości posiadanej amunicji.
    /// </summary>
    /// <param name="ammoType"> Określa typ aktualnie używanej amunicji</param>
    /// <param name="amount"> Określa ilość o jaką zwiększana jest pula posiadanej amunicji</param>
    public void IncreaseAmount(AmmoType ammoType, int amount)
    {
        GetAmmoSlot(ammoType).ammoAmount += amount;
    }
    /// <summary>
    /// Metoda zwracająca posiadaną "paczkę" amunicji na podstawie danego typu amunicji.
    /// </summary>
    /// <param name="ammoType"> Typ amunicji dla którego chcemy uzyskać posiadaną "paczkę"</param>
    /// <returns> Posiadaną "paczkę" amunicji</returns>
    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.ammoType == ammoType)
            {
                return slot;
            }
        }
        return null;
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pola z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            ammoSlots = this.ammoSlots
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
        ammoSlots = saveData.ammoSlots;
    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public AmmoSlot[] ammoSlots;
    }


}
