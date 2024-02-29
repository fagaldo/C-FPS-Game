using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Klasa odpowiedzialna za obsługę mechaniki latarkii.
/// </summary>
public class FlashLightSystem : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole określające wartość zanikania intensywności światła latarki.
    /// </summary>
    [SerializeField] float lightDecay = .1f;
    /// <summary>
    /// Pole określające wartość zanikania kątu światła latarki.
    /// </summary>
    [SerializeField] float angleDecay = 1f;
    /// <summary>
    /// Pole określające maksymalny kąt światła latarki.
    /// </summary>
    [SerializeField] float minimumAngle = 30f;
    /// <summary>
    /// Pole określające czas do następnego zmniejszenia stanu baterii latarki.
    /// </summary>
    [SerializeField] float nextTimeToDecay = 0f;
    /// <summary>
    /// Pole określające intensywność z jaką zmniejszany jest stan baterii latarki.
    /// </summary>
    [SerializeField] float decayRate = .5f;
    /// <summary>
    /// Pole przechowujące referencje do obrazka z interfejsu użytkownika ukazującego poziom naładowania latarki.
    /// </summary>
    [SerializeField] Image flashlightStatus = null;
    /// <summary>
    /// Pole przechowujące informacje, czy latarka jest załączona w danym momencie.
    /// </summary>
    private bool isOn = true;
    /// <summary>
    /// Pole przechowujące referencje do obiektu źródła dźwięku latarki.
    /// </summary>
    private AudioSource audioo;
    /// <summary>
    /// Pole przechowujące referencje do obiektu emitującego światło w grze.
    /// </summary>
    Light myLight;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Następuje w niej inicjalizacjia wszystkich wymagających tego pól skryptu.
    /// </summary>
    private void Start()
    {
        myLight = GetComponent<Light>();
        audioo = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Metoda wykonywana co klatkę w grze. Wywoływane są w niej metody zmniejszające intensywność światła, a także ma tu miejsce
    /// obsługa wyłączania i włączania latarki.
    /// </summary>
    private void Update()
    {
        Normalize();
        DecreaseLightAngle();
        DecreaseLightIntensity();
        flashlightStatus.fillAmount = myLight.intensity / 5f;
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
                if (!isOn)
                    myLight.enabled = false;
                else
                    myLight.enabled = true;
            audioo.Play();
        }
        
    }
    /// <summary>
    /// Metoda która upewnia się, że parametry światła latarki nie wyjdą poza ich maksimum.
    /// </summary>
    private void Normalize()
    {
        if (myLight.spotAngle > 70)
            myLight.spotAngle = 70;
        if (myLight.intensity > 5)
            myLight.intensity = 5;
    }
    /// <summary>
    /// Metoda odpowiedzialna za przywracanie wartości kąta światła latarki w momencie podniesienia baterii.
    /// </summary>
    /// <param name="restoreAngle"> Wartość kąta przywracanego światła.</param>
    public void RestoreLightAngle(float restoreAngle)
    {
        myLight.spotAngle += restoreAngle;

    }
    /// <summary>
    /// Metoda odpowiedzialna za przywracanie intensywności światła latarki w momencie podniesienia baterii.
    /// </summary>
    /// <param name="intensityAmount"> Wartość intensywności o jaką inkrementowane jest światło.</param>
    public void AddLightIntensity(float intensityAmount)
    {
        myLight.intensity += intensityAmount;

    }
    /// <summary>
    /// Metoda odpowiedzialna za zmniejszanie kąta światła latarki w czasie.
    /// </summary>
    private void DecreaseLightAngle()
    {
        if (myLight.spotAngle > minimumAngle && Time.time >= nextTimeToDecay && isOn)
        {
            myLight.spotAngle -= angleDecay;
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za zmniejszanie intensywności światła latarki w czasie.
    /// </summary>
    private void DecreaseLightIntensity()
    {
        if (Time.time >= nextTimeToDecay && isOn)
        {
            myLight.intensity -= lightDecay;
            CalculateNextDecayTime();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wyliczanie czasu w którym powinna nastąpić ponowna dekrementacja parametrów światła latarki.
    /// </summary>
    private void CalculateNextDecayTime()
    {
        nextTimeToDecay = Time.time + 1f / decayRate;
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pola z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pola które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            spotAngle = myLight.spotAngle,
            lightIntensity = myLight.intensity
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
        myLight.intensity = saveData.lightIntensity;
        myLight.spotAngle = saveData.spotAngle;
    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public float lightIntensity;
        public float spotAngle;
    }
}
