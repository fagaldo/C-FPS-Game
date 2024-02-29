using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
/// <summary>
/// Klasa obsługująca mechanikę punktów zdrowia, oraz życia gracza.
/// </summary>
public class PlayerHealth : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole przechowujące informację o ilości punktów życia gracza.
    /// </summary>
    [SerializeField] float playerHealth = 100f;
    /// <summary>
    /// Pole zawierające referencje do canvasu wyświetlającego punkty życia gracza.
    /// </summary>
    [SerializeField] Canvas HPCanvas;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego punkty życia gracza.
    /// </summary>
    TextMeshProUGUI hpDisplay;
    /// <summary>
    /// Metoda zwracająca ilość punktów życia gracza.
    /// </summary>
    /// <returns> Ilość punktów życia gracza.</returns>
    public float GetHealth()
    {
        return playerHealth;
    }
    /// <summary>
    /// Metoda za pomocą której zadawane są obrażenia postaci gracza.
    /// </summary>
    /// <param name="dmg"> Ilość punktów życia jaką należy odebrać graczowi.</param>
    public void TakeDamage(float dmg)
    {
        if (playerHealth > 0) playerHealth -= dmg;
        if (playerHealth <= 0)
        {
            FindObjectOfType<DeathHandler>().HandleDeath();
        }
    }
    /// <summary>
    /// Metoda za pomocą której przywracane są punkty życia postaci gracza.
    /// </summary>
    /// <param name="amount"> Ilość przywracanych punktów życia.</param>
    public void RestoreHealth(float amount)
    {
        amount = 100 - playerHealth;
        if (amount > 50) amount = 50;
        playerHealth += amount;
    }
    /// <summary>
    /// Metoda wykonywana co klatkę, ustawiana jest w niej wyświetlana w interfejsie użytkwonika prawdiwłowa liczba punktów życia gracza.
    /// </summary>
    private void Update()
    {
        hpDisplay = HPCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        hpDisplay.text = Math.Round(playerHealth).ToString();
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pole z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierajacy pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            playerHealth = this.playerHealth
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
        playerHealth = saveData.playerHealth;
    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public float playerHealth;
    }
}
