using UnityEngine;
using TMPro;
using System.Collections;
using System;
/// <summary>
/// Klasa umożliwiająca interakcję, w postaci podnoszenia, z obiektami apteczek w grze.
/// </summary>
public class HealthPickup : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy odpowiedzialnej za mechanikę punktów życia gracza.
    /// </summary>
    [SerializeField] PlayerHealth playerHP;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego w interfejsie użytkownika punkty życia gracza. 
    /// </summary>
    TextMeshProUGUI hpDisplay;
    /// <summary>
    /// Pole zawierające referencje do canvasu, który wyświetla dialog do podniesienia obiektu apteczki.
    /// </summary>
    [SerializeField] Canvas pickupCanvas;
    /// <summary>
    /// Pole zawierające referencje od pliku audio odtwarzane w momencie podniesienia apteczki.
    /// </summary>
    AudioSource sighSound;
    /// <summary>
    /// Pole logiczne określające, czy dana instancja klasy, tj. obiekt apteczki został już podniesiony.
    /// </summary>
    private bool pickedUp = false;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce podpięcie referencji pól tego wymagających.
    /// </summary>
    private void Start()
    {
        sighSound = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu apteczki.
    /// W momencie podniesienia apteczki przywracana jest odpowiednia ilość pkt życia graczowi (max 100), a także usuwany jest użyty obiekt apteczki.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            pickupCanvas.enabled = true;
            hpDisplay = pickupCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            hpDisplay.text = "Press E to pickup MEDKIT";
            if (Input.GetKeyDown(KeyCode.E) && playerHP.GetHealth() < 100)
            {
                pickedUp = true;
                playerHP.RestoreHealth(50);
                GetComponent<Animator>().SetTrigger("Open");
                pickupCanvas.enabled = false;
                StartCoroutine(destroyObj());
            }
            else if (playerHP.GetHealth() == 100)
            {
                hpDisplay.text = "You cant pickup health with full HP";
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu apteczki.
    /// W momencie podniesienia apteczki przywracana jest odpowiednia ilość pkt życia graczowi (max 100), a także usuwany jest użyty obiekt apteczki.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && !pickedUp && playerHP.GetHealth() < 100 && other.gameObject.tag == "Player")
        {
            pickedUp = true;
            playerHP.RestoreHealth(50);
            GetComponent<Animator>().SetTrigger("Open");
            pickupCanvas.enabled = false;
            StartCoroutine(destroyObj());
        }
        else if (playerHP.GetHealth() == 100)
        {
            hpDisplay.text = "You cant pickup health with full HP";
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu apteczki.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodziła kolizja.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pickupCanvas.enabled = false;
        }
    }
    /// <summary>
    /// Metoda umożliwiająca deaktywację na scenie obiektu apteczki po określonym czasie, tj. po odtworzeniu dźwięku i animacji.
    /// </summary>
    /// <returns></returns>
    IEnumerator destroyObj()
    {
        sighSound.Play();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
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
