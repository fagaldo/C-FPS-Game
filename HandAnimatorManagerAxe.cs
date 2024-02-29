using UnityEngine;

/// <summary>
/// Klasa obsługująca animacje dłoni gracza trzymającej siekierę wykorzystująca maszynę stanów.
/// </summary>
public class HandAnimatorManagerAxe : MonoBehaviour
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
    public int currentState = 100;
    /// <summary>
    /// Pole przechowujące informacje o ostatnim stanie animacji ręki.
    /// </summary>
    int lastState = -1;
    /// <summary>
    /// Pole przechowujące informacje, czy przez rękę wykonywana jest jakaś czynność.
    /// </summary>
    private bool hasAction = false;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry.
    /// </summary>
    void Start()
    {
        handAnimator = GetComponent<Animator>();
    }
    /// <summary>
    /// Metoda wykonywana tylko przy wyłączeniu obiektu obsługiwanego przez skrypt.
    /// </summary>
    private void OnDisable()
    {
        hasAction = false;
    }
    /// <summary>
    /// Metoda wykonywana co klatkę, kontroluje ona zmiany stanów animatora, a także aktywację odpowiednich zmiennych w animatorze.
    /// </summary>
    void Update()
    {
        currentState = 6;

        if (lastState != currentState || lastState == 6)
        {
            lastState = currentState;
            handAnimator.SetInteger("State", currentState);
            TurnOnState(currentState);
        }

        handAnimator.SetBool("Action", Input.GetMouseButton(0));
        handAnimator.SetBool("Hold", Input.GetMouseButton(1));
        if (!hasAction)
        {
            handAnimator.SetBool("Action", true);
            hasAction = true;
        }

    }
    /// <summary>
    /// Metoda odpowiedzialna za przełączanie stanów modeli.
    /// </summary>
    /// <param name="stateNumber"> Numer stanu animacji aktualnie ustawionego.</param>
    void TurnOnState(int stateNumber)
    {
        foreach (var item in stateModels)
        {
            if (item.stateNumber == stateNumber)
                item.go.SetActive(true);
            else if (item.go.activeSelf)
                item.go.SetActive(false);
        }
    }


}
