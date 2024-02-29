using System.Collections;
using UnityEngine;
using TMPro;
using System;
/// <summary>
/// Klasa odpowiedzialna za kontrolę efektów cząsteczkowych związanych z rozpaleniem ogniska w finalnej scenie gry.
/// </summary>
public class ParticleController : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole zawierające referencje do efektu cząsteczkowego ognia.
    /// </summary>
    [SerializeField] ParticleSystem fire;
    /// <summary>
    /// Pole zawierające referencje do obiektu emitującego światło z ogniska.
    /// </summary>
    [SerializeField] Light fireLight;
    /// <summary>
    /// Pole będące obiektem klasy odpowiedzialen za mechanikę podnoszenia zapałek potrzebnych do rozpalenia ogniska.
    /// </summary>
    MatchesPickup matchesPickup;
    /// <summary>
    /// Pole zawierające referencje do canvasu dialogowego wyświetlanego w momencie interakcji z ogniskiem.
    /// </summary>
    [SerializeField] Canvas textCanvas;
    /// <summary>
    /// Pole zawierające referencje do pola tekstowego wyświetlanego dialogu w momencie interakcji z ogniskiem.
    /// </summary>
    TextMeshProUGUI fireplaceText;
    /// <summary>
    /// Pole będące tablicą zawierającą referencję do obiektów przeciwników, którzy pojawiają się wokół ogniska w momencie jego rozpalenia.
    /// </summary>
    [SerializeField] GameObject[] Enemies;
    /// <summary>
    /// Pole zawierające informację logiczną, czy ognisko zostało już rozpalone.
    /// </summary>
    private bool isLighten = false;
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Ustawiane, oraz przypisywane są w niej wszystkie wymagające tego pola klasy.
    /// </summary>
    private void Start()
    {
        fire.enableEmission = false;
        fireLight.enabled = false;
        matchesPickup = FindObjectOfType<MatchesPickup>();
        fireplaceText = textCanvas.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider ogniska. Jeżeli gracz podniósł wcześniej zapałki, to może rozpalić ognisko w momencie interakcji.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zaszła kolizja.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isLighten)
        {
            if (matchesPickup.PickedUp())
            {
                textCanvas.enabled = true;
                fireplaceText.text = "Press E to light up the fireplace";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    textCanvas.enabled = false;
                    StartCoroutine(LightUp());
                }
            }
            else
            {
                textCanvas.enabled = true;
                fireplaceText.text = "You need something to light up the fire with";
            }
        }

    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia zakończenia kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider ogniska.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodziła kolizja.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textCanvas.enabled = false;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obsługę mechaniki interakcji w momencie wykrycia ciągłej kolizji między colliderami obiektów.
    /// W tym przypadku jednym z nich jest collider ogniska. Jeżeli gracz podniósł wcześniej zapałki, to może rozpalić ognisko w momencie interakcji.
    /// </summary>
    /// <param name="other"> Collider obiektu z którym zachodzi kolizja.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && matchesPickup.PickedUp() && Input.GetKeyDown(KeyCode.E) && !isLighten)
            StartCoroutine(LightUp());
    }
    /// <summary>
    /// Metoda zwracająca stan zmiennej mówiącej o rozpaleniu ogniska.
    /// </summary>
    /// <returns> Stan zmiennej mówiącej o rozpaleniu ogniska.</returns>
    public bool IsLighten()
    {
        return isLighten;
    }
    /// <summary>
    /// Metoda odpowiedzialna za rozpalenie ogniska z opóźnieniem po interakcji z graczem.
    /// </summary>
    /// <returns> Czeka 2.5 sekundy do rozpalenia.</returns>
    IEnumerator LightUp()
    {
        isLighten = true;
        yield return new WaitForSeconds(2.5f);
        fire.enableEmission = true;
        fireLight.enabled = true;
        SpawnEnemies();
    }
    /// <summary>
    /// Metoda odpowiedzialna za natychmiastowe rozpalenie ogniska w momencie wczytania stanu gry w którym było już ono rozpalone.
    /// </summary>
    private void LightUpInstantly()
    {
        fire.enableEmission = true;
        fireLight.enabled = true;
        SpawnEnemies();
    }
    /// <summary>
    /// Metoda odpowiedzialna za aktywację przeciwników wokół ogniska.
    /// </summary>
    private void SpawnEnemies()
    {
        foreach (var enemy in Enemies)
        {
            enemy.SetActive(true);
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pole z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierajacy pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            isLighten = this.isLighten
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
        isLighten = saveData.isLighten;
        if (isLighten)
            LightUpInstantly();
    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public bool isLighten;
    }
}
