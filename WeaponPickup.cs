using TMPro;
using UnityEngine;

/// <summary>
/// Klasa umożliwiająca interakcję, w postaci podnoszenia, z obiektami broni w grze.
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do canvasu, który wyświetla dialog do podniesienia obiektów broni.
    /// </summary>
    [SerializeField] Canvas displayTextCanvas;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wypisującego dialog do podniesienia obiektów broni.
    /// </summary>
    TextMeshProUGUI pickupText;
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy będącej kontrolerem broni posiadanych przez gracza.
    /// </summary>
    [SerializeField] WeaponSwitcher weaponController;
    /// <summary>
    /// Pole zawierające wartość określającą do której broni przypisany jest skrypt.
    /// </summary>
    [SerializeField] int whichWeapon;
    /// <summary>
    /// Metoda wywoływana co klatkę, sprawdza ona czy broń nie jest już posiadana przez gracza, a jeżeli tak, to jej
    /// instancja jest deaktywowana.
    /// </summary>
    private void Update()
    {
        foreach (var tmp in weaponController.weaponsObtained)
        {
            if (tmp.Key == whichWeapon && tmp.Value)
            {
                gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu broni.
    /// W zależności od tego z jaką bronią gracz wszedł w interakcje, taka jest podnoszona.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            displayTextCanvas.enabled = true;
            pickupText = displayTextCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            if (gameObject.name == "AKM")
                pickupText.text = "Press E to pickup AKM";
            else if (gameObject.name == "Shotgun")
                pickupText.text = "Press E to pickup SHOTGUN";
            else if (gameObject.name == "Pistol")
                pickupText.text = "Press E to pickup PISTOL";
            else if (gameObject.name == "Axe")
                pickupText.text = "Press E to pickup AXE";
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (gameObject.name == "AKM")
                {
                    weaponController.weaponsObtained[2] = true;
                    weaponController.currentWeapon = 2;
                }
                else if (gameObject.name == "Shotgun")
                {
                    weaponController.weaponsObtained[1] = true;
                    weaponController.currentWeapon = 1;
                }
                else if (gameObject.name == "Pistol")
                {
                    weaponController.weaponsObtained[0] = true;
                    weaponController.currentWeapon = 0;
                }
                else if (gameObject.name == "Axe")
                {
                    weaponController.weaponsObtained[3] = true;
                    weaponController.currentWeapon = 3;
                }
                gameObject.SetActive(false);
                displayTextCanvas.enabled = false;
                weaponController.pickedUp = true;
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu broni.
    /// W zależności od tego z jaką bronią gracz wszedł w interakcje, taka jest podnoszona.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (gameObject.name == "AKM")
                {
                    weaponController.weaponsObtained[2] = true;
                    weaponController.currentWeapon = 2;
                }
                else if (gameObject.name == "Shotgun")
                {
                    weaponController.weaponsObtained[1] = true;
                    weaponController.currentWeapon = 1;
                }
                else if (gameObject.name == "Pistol")
                {
                    weaponController.weaponsObtained[0] = true;
                    weaponController.currentWeapon = 0;
                }
                else if (gameObject.name == "Axe")
                {
                    weaponController.weaponsObtained[3] = true;
                    weaponController.currentWeapon = 3;
                }
                gameObject.SetActive(false);
                displayTextCanvas.enabled = false;
                weaponController.pickedUp = true;
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider obiektu broni.
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
