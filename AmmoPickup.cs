using UnityEngine;
using System.Collections;
using TMPro;
using System;
/// <summary>
/// Klasa umożliwiająca interakcję, w postaci podnoszenia, z obiektami amunicji w grze.
/// </summary>
public class AmmoPickup : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole określające ilość podnoszonej amunicji.
    /// </summary>
    [SerializeField] int ammoAmount = 10;
    /// <summary>
    /// Pole określające typ podnoszonej amunicji.
    /// </summary>
    [SerializeField] AmmoType ammoType;
    /// <summary>
    /// Pole zawierające referencje do canvasu, który wyświetla dialog do podniesienia obiektów amunicji.
    /// </summary>
    [SerializeField] Canvas displayTextCanvas;
    /// <summary>
    /// Pole zawierające referencje do obiektu emitującego dźwięki podnoszenia amunicji.
    /// </summary>
    AudioSource pickupSound;
    /// <summary>
    /// Pole zawierające referencje do animatora obiektów umożliwiających podniesienie amunicji.
    /// </summary>
    Animator anim;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wypisującego dialog do podniesienia obiektów amunicji.
    /// </summary>
    TextMeshProUGUI ammoPickupText;
    /// <summary>
    /// Pole logiczne określające, czy dana instancja klasy, tj. obiekt z amunicją został już podniesiony.
    /// </summary>
    private bool pickedUp = false;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce podpięcie referencji pól tego wymagających.
    /// </summary>
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        pickupSound = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu z amunicją.
    /// W momencie podniesienia amuncji, dodawana jest odpowiednia jej ilość do ekwipunku, a także usuwany jest obiekt amunicji z którym zaszła interakacja.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            displayTextCanvas.enabled = true;
            ammoPickupText = displayTextCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            ammoPickupText.text = "Press E to pickup " + ammoType.ToString() + " ammo";
            if (Input.GetKeyDown(KeyCode.E))
            {
                anim.SetTrigger("Open");
                FindObjectOfType<Ammo>().IncreaseAmount(ammoType, ammoAmount);
                pickedUp = true;
                displayTextCanvas.enabled = false;
                StartCoroutine(destroyObj());
            }

        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu z amunicją.
    /// W momencie podniesienia amuncji, dodawana jest odpowiednia jej ilość do ekwipunku, a także usuwany jest obiekt amunicji z którym zaszła interakacja.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                anim.SetTrigger("Open");
                Debug.Log("pickin up ammo");
                FindObjectOfType<Ammo>().IncreaseAmount(ammoType, ammoAmount);
                pickedUp = true;
                displayTextCanvas.enabled = false;
                StartCoroutine(destroyObj());
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu z amunicją.
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
    /// Metoda umożliwiająca deaktywację na scenie obiektu z amunicją po określonym czasie, tj. po odtworzeniu dźwięku i animacji.
    /// </summary>
    /// <returns></returns>
    IEnumerator destroyObj()
    {
        pickupSound.Play();
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

