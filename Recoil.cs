
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Klasa odpowiedzialna za mechanikę rozrzutu pocisków broni.
/// </summary>
public class Recoil : MonoBehaviour
{
    /// <summary>
    /// Pole zawierające współrzędne aktualnej rotacji kamery gracza.
    /// </summary>
    private Vector3 currentRotation;
    /// <summary>
    /// Pole zawierające współrzędne docelowej rotacji kamery przy rozrzucie w wyniku wystrzału.
    /// </summary>
    private Vector3 targetRotation;
    /// <summary>
    /// Pole określające szybkość z jaką kamera porusza się w kierunku docelowym rozrzutu.
    /// </summary>
    [SerializeField] private float snappiness;
    /// <summary>
    /// Pole określające szybkość z jaką kamera wraca do punktu początkowego przed rozrzutem.
    /// </summary>
    [SerializeField] private float returnSpeed;
    /// <summary>
    /// Metoda wywoływana co klatkę. Rotuje ona odpowiednio kamerę, a w przypadku zmiany wartości pola targetRotation
    /// powoduje ona efekt rozrzutu kul.
    /// </summary>
    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    /// <summary>
    /// Metoda odpowiedzialna za efekt rozrzuty, ustawia ona odpowiednią wartość pola targetRotation,
    /// tak aby ten efekt nastąpił w wyniku działania metody Update.
    /// </summary>
    /// <param name="recoilX"> Wartość rozrzutu w osi X.</param>
    /// <param name="recoilY"> Wartość rozrzutu w osi Y.</param>
    /// <param name="recoilZ"> Wartość rozrzutu w osi Z.</param>
    public void RecoilFire(float recoilX, float recoilY, float recoilZ)
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
