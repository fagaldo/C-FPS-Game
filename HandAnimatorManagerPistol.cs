using UnityEngine;

/// <summary>
/// Klasa obsługująca animacje dłoni gracza trzymającego pistolet wykorzystująca maszynę stanów.
/// </summary>
public class HandAnimatorManagerPistol : MonoBehaviour
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
    /// Pole przechowujące informacje o obecnym stanie animacji ręki.
    /// </summary>
    public int currentState = 5;
    /// <summary>
	/// Pole przechowujące referencje do obiektu klasy kontrolującej mechanikę bronii.
	/// </summary>
    WeaponSwitcher weaponController;

    /// <summary>
	/// Metoda wykonywana tylko w pierwszej klatce gry.
	/// </summary>
    void Start()
    {
        handAnimator = GetComponent<Animator>();
        weaponController = FindObjectOfType<WeaponSwitcher>();
    }
    /// <summary>
	/// Metoda wykonywana przy aktywacji obiektu obsługiwanego przez skrypt.
	/// </summary>
    private void OnEnable()
    {
        currentState = 5;
    }
    /// <summary>
    /// Metoda wykonywana co klatkę, kontroluje ona zmiany stanów animatora, a także aktywację w nim odpowiednich zmiennych.
    /// </summary>
    void Update()
    {
        if (currentState == 5)
        {
            handAnimator.SetInteger("State", currentState);
            TurnOnState(currentState);
        }
        handAnimator.SetBool("Action", Input.GetMouseButton(0));
        handAnimator.SetBool("Hold", Input.GetMouseButton(1));

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
