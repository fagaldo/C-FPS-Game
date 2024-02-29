using UnityEngine;
using System.Collections;
/// <summary>
/// Klasa obsługująca animacje prawej dłoni gracza wykorzystająca maszynę stanów.
/// </summary>
public class HandAnimatorManagerRight : MonoBehaviour
{
    /// <summary>
	/// Pole przechowujące modele stanów.
	/// </summary>
    public StateModel[] stateModels;
    /// <summary>
	/// Pole przechowujące referencje do obiektu animatora dłoni.
	/// </summary>
    Animator handAnimator;
    /// <summary>
    /// Pole przechowujące informacje o ostatnim stanie animacji ręki.
    /// </summary>
    public int lastState = -1;
    /// <summary>
    /// Pole przechowujące informacje o obecnym stanie animacji ręki.
    /// </summary>
    public int currentState = 100;
    /// <summary>
	/// Pole przechowujące referencje do obiektu klasy kontrolującej mechanikę bronii.
	/// </summary>
    WeaponSwitcher weaponsController;
    /// <summary>
	/// Pole przechowujące informacje, czy w danym momencie, gdy w grze trzymany jest lewy przycisk myszy
	/// atak wykonywany jest lewą ręką.
	/// </summary>
    [SerializeField] public bool isLeft = false;
    /// <summary>
	/// Metoda wykonywana tylko w pierwszej klatce gry.
	/// </summary>
    void Start()
    {
        handAnimator = GetComponent<Animator>();
        weaponsController = GetComponentInParent<WeaponSwitcher>();
    }
    /// <summary>
	/// Metoda wykonywana przy aktywacji obiektu obsługiwanego przez skrypt.
	/// </summary>
    private void OnEnable()
    {
        currentState = 100;
    }
    /// <summary>
	/// Metoda wykonywana co klatkę, kontroluje ona zmiany stanów animatora,
	/// a także aktywację w nim odpowiednich zmiennych. Sprawdza ona również, czy w przypadku
	/// gdy w grze gracz jako broń używa pięści, to atak powinien być wykonany naprzemiennie z prawej, czy z lewej pięści.
    /// Przełącza ona również stan pola "isLeft" w kontrolorze animacji lewej dłoni.   
	/// </summary>
    void Update()
    {
        if (Input.GetMouseButton(0) && !isLeft)
        {
            currentState = 2;
            StartCoroutine(TriggerAction());
        }

        if (lastState != currentState || currentState == 100)
        {
            lastState = currentState;
            handAnimator.SetInteger("State", currentState);
            TurnOnState(currentState);
        }

        if (!isLeft)
            handAnimator.SetBool("Action", Input.GetMouseButton(0));
        else
        {
            currentState = 100;
        }
        handAnimator.SetBool("Hold", Input.GetMouseButton(1));

        FindObjectOfType<HandAnimatorManager>().isLeft = isLeft;
    }
    /// <summary>
	/// Metoda, która po upływie pewnego czasu aktywuje ustawia odpowiednią zmienną w animatorze.
	/// </summary>
	/// <returns> Czeka 0.25sekundy.</returns>
    IEnumerator TriggerAction()
    {
        yield return new WaitForSeconds(0.25f);
        handAnimator.SetBool("Action", true);
    }
    /// <summary>
	/// Metoda odpowiedzialna za przełączanie stanów modeli.
	/// </summary>
	/// <param name="stateNumber"> Numer stanu animacji aktualnie ustawionego.</param>
    void TurnOnState(int stateNumber)
    {
        foreach (var item in stateModels)
        {
            if (item.stateNumber == stateNumber && !item.go.activeSelf)
                item.go.SetActive(true);
            else if (item.go.activeSelf)
                item.go.SetActive(false);
        }
    }

}

