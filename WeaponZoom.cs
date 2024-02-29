using UnityEngine;
using MovementController;
/// <summary>
/// Klasa odpowiedzialna za mechanikę celowania z bronii, czyli przybliżenia pola widzenia kamery podczas celowania.
/// </summary>
public class WeaponZoom : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy będącej kontrolerem ruchu gracza.
    /// </summary>
    [SerializeField] PlayerController controller;
    /// <summary>
    /// Pole zawierające wartość pola widzenia kamery przy wyłączonym trybie celowania.
    /// </summary>
    [SerializeField] float zoomedOutFOV = 60f;
    /// <summary>
    /// Pole zawierające wartość pola widzenia kamery przy włączonym trybie celowania.
    /// </summary>
    [SerializeField] float zoomedInFOV = 25f;
    /// <summary>
    /// Pole zawierajace wartość czułości myszy przy wyłączonym trybie celowania.
    /// </summary>
    private float zoomedLookSpeed = 1f;
    /// <summary>
    /// Pole zawierajace wartość czułości myszy przy włączonym trybie celowania.
    /// </summary>
    private float zoomedOutLookSpeed = 2f;
    /// <summary>
    /// Pole określające, czy tryb celowania jest włączony, czy nie.
    /// </summary>
    public bool isZoomed = false;
    /// <summary>
    /// Metoda wywoływana tylko przy wyłączeniu skryptu.
    /// </summary>
    private void OnDisable()
    {
        ZoomOut();
    }
    /// <summary>
    /// Metoda wywoływana tylko w pierwszej klatce gry. Przypisuje ona wartości pól czułości myszy, jeżeli te są zapisane przez gracza lokalnie.
    /// </summary>
    private void Start()
    {
        if (PlayerPrefs.HasKey("Sens"))
        {
            zoomedOutLookSpeed = PlayerPrefs.GetFloat("Sens");
            zoomedLookSpeed = PlayerPrefs.GetFloat("Sens") - 1;
        }
    }
    /// <summary>
    /// Metoda wywoływana co klatkę. Ma w niej miejsce obsługa wejścia, a także wywoływanie metod włączających i wyłączających tryb celowania.
    /// </summary>
    private void Update()
    {
        zoomedOutLookSpeed = PlayerPrefs.GetFloat("Sens");
        zoomedLookSpeed = PlayerPrefs.GetFloat("Sens") - 1;
        if (Input.GetMouseButtonDown(1))
        {
            if (!isZoomed)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za wyjście z trybu celowania.
    /// </summary>
    private void ZoomOut()
    {
        isZoomed = false;
        controller.InstallFOV = zoomedOutFOV;
        controller.lookSpeed = zoomedOutLookSpeed;
        controller.RunningFOV = 65f;
    }
    /// <summary>
    /// Metoda odpowiedzialna za wejście do trybu celowania.
    /// </summary>
    private void ZoomIn()
    {
        isZoomed = true;
        controller.InstallFOV = zoomedInFOV;
        controller.lookSpeed = zoomedLookSpeed;
        controller.RunningFOV = 45f;
    }
}
