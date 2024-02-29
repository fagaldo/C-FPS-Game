using UnityEngine;
using TMPro;
using System;
/// <summary>
/// Klasa umożliwiająca interakcję, w postaci podnoszenia, z obiektami baterii do latarki w grze.
/// </summary>
public class BatteryPickup : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole określające wartość kąta ustawianego dla światła latarki w momencie podniesienia baterii.
    /// </summary>
    [SerializeField] float angleAmount = 80;
    /// <summary>
    /// Pole określające wartość intensywności przywracanej światłu latarki w momencie podniesienia baterii.
    /// </summary>
    [SerializeField] float intensityAmount = 4;
    /// <summary>
    /// Pole zawierające referencje do canvasu, który wyświetla dialog do podniesienia obiektu baterii.
    /// </summary>
    [SerializeField] Canvas displayTextCanvas;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wypisującego dialog do podniesienia obiekty baterii.
    /// </summary>
    TextMeshProUGUI batteryPickupText;
    /// <summary>
    /// Pole zawierajace referencje do obiektu klasy odpowiadającej za funkcjonowanie systemu latarkii.
    /// </summary>
    FlashLightSystem flashLight;
    /// <summary>
    /// Pole logiczne określające, czy dana instancja klasy, tj. bateria do latarki została już podniesiona.
    /// </summary>
    private bool pickedUp = false;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry.
    /// </summary>
    private void Start()
    {
        displayTextCanvas.enabled = false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu baterii.
    /// W momencie podniesienia baterii zwiększane są parametry latarkii (kąt i intensywność światła), a obiekt baterii jest usuwany.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            displayTextCanvas.enabled = true;
            batteryPickupText = displayTextCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            batteryPickupText.text = "Press E to pickup BATTERY";
            flashLight = other.GetComponentInChildren(typeof(FlashLightSystem)) as FlashLightSystem;
            if (flashLight && Input.GetKeyDown(KeyCode.E))
            {
                flashLight.RestoreLightAngle(angleAmount);
                flashLight.AddLightIntensity(intensityAmount);
                gameObject.SetActive(false);
                displayTextCanvas.enabled = false;
                pickedUp = true;
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu baterii.
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
    /// W tym przypadku jednym z nich jest collider obiektu baterii.
    /// W momencie podniesienia baterii zwiększane są parametry latarkii (kąt i intensywność światła), a obiekt baterii jest usuwany.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (flashLight && Input.GetKeyDown(KeyCode.E))
            {
                flashLight.RestoreLightAngle(angleAmount);
                flashLight.AddLightIntensity(intensityAmount);
                gameObject.SetActive(false);
                displayTextCanvas.enabled = false;
                pickedUp = true;
            }
        }
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