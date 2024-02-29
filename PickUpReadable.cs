using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Klasa umożliwiająca interakcję, w postaci podnoszenia, z obiektami które można podnieść i przeczytać w grze, tj. kartkami papiery.
/// </summary>
public class PickUpReadable : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do canvasu wyświetlającego stronę kartki podniesionej przez gracza.
    /// </summary>
    [SerializeField] Canvas displayPageCanvas;
    /// <summary>
    /// Pole zawierające referencje do canvasu wyświetlającego dialog do podniesienia kartki.
    /// </summary>
    [SerializeField] Canvas displayTextCanvas;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlającego tekst na podniesionej przez gracza kartce.
    /// </summary>
    TextMeshProUGUI pageContent;
    /// <summary>
    /// Pole przechowujące tekst przepisywany później do pola tekstowego wyświetlanego na podniesionej kartce.
    /// </summary>
    [SerializeField] string text;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wypisującego dialog do podniesienia kartki.
    /// </summary>
    TextMeshProUGUI pickupText;
    /// <summary>
    /// Pole logiczne określające, czy dana instancja klasy, tj. kartka papieru została już podniesiona.
    /// </summary>
    private bool pickedUp = false;
    /// <summary>
    /// Metoda wywoływania tylko w pierwszej klatce gry.
    /// </summary>
    private void Start()
    {
        displayPageCanvas.enabled = false;
    }
    /// <summary>
    /// Metoda wywoływana co klatkę. Skanuje ona input, a gdy ten się pojawi i gracz podniósł już kartkę, 
    /// to wyłącza ona canvas z zawartością kartki.
    /// </summary>
    private void Update()
    {
        if (pickedUp && Input.GetKeyDown(KeyCode.Tab))
        {
            displayPageCanvas.enabled = false;
            pickedUp = false;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu zapisanej kartki papiery.
    /// W momencie interakcji wyświetlany jest canvas zawierający kartkę wraz z jej treścią.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            displayTextCanvas.enabled = true;
            pickupText = displayTextCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            pickupText.text = "Press E to pickup journal page";
            pageContent = displayPageCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            pageContent.text = text;
            if (Input.GetKeyDown(KeyCode.E))
            {
                displayPageCanvas.enabled = true;
                pickedUp = true;
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu zapisanej kartki papiery.
    /// W momencie interakcji wyświetlany jest canvas zawierający kartkę wraz z jej treścią.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                displayPageCanvas.enabled = true;
                pickedUp = true;
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
}
