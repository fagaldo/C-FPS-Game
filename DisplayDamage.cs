using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Klasa odpowiedzialna za graficzne i dźwiękowe wyświetlanie, a także odtwarzanie efektu otrzymanych 
/// obrażeń przez postać gracza.
/// </summary>
public class DisplayDamage : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do canvasu wyświetlającego krew na ekranie.
    /// </summary>
    [SerializeField] Canvas displayDamageCanvas;
    /// <summary>
    /// Pole zawierajace referencje do pliku audio, który jest odtwarzany w momencie otrzymania obrażeń.
    /// </summary>
    [SerializeField] AudioClip dmgAudio = null;
    /// <summary>
    /// Pole zawierające referencje do obiektu odtwarzającego dźwięk w momencie otrzymania obrażeń.
    /// </summary>
    AudioSource audioSource = null;
    /// <summary>
    /// Metoda wywoływana tylko przy aktywacji skryptu.
    /// </summary>
    private void Awake()
    {
        displayDamageCanvas.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Metoda odpowiedzialna za graficzne wyświetlenie, a także dźwiękowe odtworzenie efektu otrzymanych 
    /// obrażeń przez postać gracza. Dodatkowo wywołuje korutynę, która po czasie deaktywuje efekt krwi na ekranie.
    /// </summary>
    public void DisplayDmg()
    {
        displayDamageCanvas.enabled = true;
        audioSource.PlayOneShot(dmgAudio);
        StartCoroutine(CountToDeactivate(1.5f));
    }
    /// <summary>
    /// Metoda odpowiedzialna za deaktywację po zadanym czasie efektu graficznego krwi na ekranie.
    /// </summary>
    /// <param name="duration"> Czas jaki metoda ma odczekać.</param>
    /// <returns> Czeka określony czas.</returns>
    IEnumerator CountToDeactivate(float duration)
    {
        yield return new WaitForSeconds(duration);
        displayDamageCanvas.enabled = false;
    }

}
