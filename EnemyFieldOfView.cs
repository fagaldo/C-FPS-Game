using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Klasa odpowiedzialna za obsługę mechaniki pola widzenia przeciwników.
/// </summary>
public class EnemyFieldOfView : MonoBehaviour
{
    /// <summary>
    /// Pole przechowujące wartość promienia pola widzenia przeciwnika.
    /// </summary>
    public float radius;
    /// <summary>
    /// Pole przechowujące wartość kątu pola widzenia przeciwnika.
    /// </summary>
    [Range(0, 360)]
    public float angle;
    /// <summary>
    /// Pole przechowujące referencje do obiektu gracza.
    /// </summary>
    public GameObject playerRef;
    /// <summary>
    /// Pole przechowujące wartwę docelową określającą gracza, czyli tą na którą sztuczna inteligencja po wykryciu zareaguje.
    /// </summary>
    public LayerMask targetMask;
    /// <summary>
    /// Pole przechowujące warstwy określające przeszkody jakie mogą być między przeciwnikiem a graczem.
    /// </summary>
    public LayerMask obstrucionMask;
    /// <summary>
    /// Pole przechowujące informacje, czy gracz jest widziany przez przeciwnika.
    /// </summary>
    public bool canSeePlayer;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Ma w niej miejsce podpięcie referencji do obiektu postaci gracza,
    /// a także uruchomienie korutyny odpowiedzialnej za skanowanie pola widzenia przeciwnika.
    /// </summary>
    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }
    /// <summary>
    /// Metoda odmierzająca czas po którym cyklicznie sprawdzane jest pole widzenia przeciwnika.
    /// </summary>
    /// <returns> Czeka określony czas.</returns>
    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za sprawdzanie pola widzenia przeciwnika i wyszukiwanie w nim graza.
    /// W przypadku spełnienia wszystkich warunków wykrycia gracza przestawia ona odpowiednią flagę, tj. zmienną boolowską.
    /// </summary>
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstrucionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
