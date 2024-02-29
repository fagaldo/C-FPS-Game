using System;
using UnityEngine;
using TMPro;
/// <summary>
/// Klasa umożliwiająca interakcję, w postaci podnoszenia, z obiektem zapałek w grze.
/// </summary>
public class MatchesPickup : MonoBehaviour, ISaveable
{   /// <summary>
    /// Pole zawierające referencje do canvasu, który wyświetla dialog do podniesienia obiektu zapałek.
    /// </summary>
    [SerializeField] Canvas displayTextCanvas;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wypisującego dialog do podniesienia obiektu zapałek.
    /// </summary>
    TextMeshProUGUI matchesPickupText;
    /// <summary>
    /// Pole logiczne określające, czy dana instancja klasy, tj. obiekt zapałek został już podniesiony.
    /// </summary>
    private bool pickedUp = false;
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu zapałek.
    /// W momencie podniesienia zapałek przełączana jest odpowiednia flaga, a także deaktywowany jest obiekt zapałek.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            displayTextCanvas.enabled = true;
            matchesPickupText = displayTextCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            matchesPickupText.text = "Press E to pickup MATCHES";
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.SetActive(false);
                displayTextCanvas.enabled = false;
                pickedUp = true;
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu zapałek.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodziła kolizja.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            displayTextCanvas.enabled = false;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu zapałek.
    /// W momencie podniesienia zapałek przełączana jest odpowiednia flaga, a także deaktywowany jest obiekt zapałek.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.SetActive(false);
                displayTextCanvas.enabled = false;
                pickedUp = true;
            }
        }
    }
    /// <summary>
    /// Metoda zwracająca informację o tym, czy obiekt zapałek został już podniesiony.
    /// </summary>
    /// <returns> Informację logiczną, czy obiekt zapałek został podniesiony.</returns>
    public bool PickedUp()
    {
        if (pickedUp)
            return true;
        else
            return false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pole z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            pickedUp = this.pickedUp
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
        this.pickedUp = saveData.pickedUp;
        if (pickedUp)
        {
            gameObject.SetActive(false);
        }

    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public bool pickedUp;
    }
}
