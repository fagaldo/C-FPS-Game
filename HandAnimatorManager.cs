using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// Klasa obsługująca animacje lewej dłoni gracza wykorzystająca maszynę stanów.
/// </summary>
public class HandAnimatorManager : MonoBehaviour
{
	/// <summary>
	/// Pole przechowujące modele stanów.
	/// </summary>
	public StateModel[] stateModels;
	/// <summary>
	/// Pole przechowujące referencje do obiektu animatora dłoni.
	/// </summary>
	public Animator handAnimator;
	/// <summary>
    /// Pole przechowujące informacje o ostatnim stanie animacji ręki.
    /// </summary>
	public int lastState = -1;
	/// <summary>
    /// Pole przechowujące informacje o obecnym stanie animacji ręki.
    /// </summary>
	public int currentState = 100;
	/// <summary>
	/// Pole przechowujące informacje, czy w danym momencie, gdy w grze trzymany jest lewy przycisk myszy
	/// atak wykonywany jest lewą ręką.
	/// </summary>
	[SerializeField] public bool isLeft = false;
	/// <summary>
	/// Pole przechowujące referencje do obiektu klasy kontrolującej mechanikę bronii.
	/// </summary>
	WeaponSwitcher weaponController;

	/// <summary>
	/// Metoda wykonywana tylko w pierwszej klatce gry.
	/// </summary>
	void Start ()
	{
		handAnimator = GetComponent<Animator> ();
		weaponController = FindObjectOfType<WeaponSwitcher>();
	}
	/// <summary>
	/// Metoda wykonywana przy aktywacji obiektu obsługiwanego przez skrypt.
	/// </summary>
	private void OnEnable() {
		currentState = 100;
	}
	
	/// <summary>
	/// Metoda wykonywana co klatkę, kontroluje ona zmiany stanów animatora,
	/// a także aktywację w nim odpowiednich zmiennych. Sprawdza ona również, czy w przypadku
	/// gdy w grze gracz jako broń używa pięści, to atak powinien być wykonany naprzemiennie z prawej, czy z lewej pięści.
	/// </summary>
	void Update ()
	{
		if(Input.GetMouseButton (0) && isLeft && weaponController.currentWeapon == 4)
		{
			currentState = 3;
			StartCoroutine(TriggerAction());
		}
		if(Input.GetKeyDown (KeyCode.E))
			handAnimator.SetTrigger ("Pickup");
		if (lastState != currentState || currentState == 100) 
		{
			lastState = currentState;
			handAnimator.SetInteger ("State", currentState);
			TurnOnState (currentState);
		}
		if(isLeft)
		handAnimator.SetBool ("Action", Input.GetMouseButton (0));
		else
		{
			currentState = 100;
		}
			
		handAnimator.SetBool ("Hold", Input.GetMouseButton (1));

	}
	/// <summary>
	/// Metoda, która po upływie pewnego czasu aktywuje ustawia odpowiednią zmienną w animatorze.
	/// </summary>
	/// <returns> Czeka 0.25sekundy.</returns>
	IEnumerator TriggerAction()
	{
		yield return new WaitForSeconds(0.25f);
		handAnimator.SetBool ("Action", true);
	}
	/// <summary>
	/// Metoda odpowiedzialna za przełączanie stanów modeli.
	/// </summary>
	/// <param name="stateNumber"> Numer stanu animacji aktualnie ustawionego.</param>
	void TurnOnState (int stateNumber)
	{
		foreach (var item in stateModels) {
			if (item.stateNumber == stateNumber && !item.go.activeSelf)
				item.go.SetActive (true);
			else if (item.go.activeSelf)
				item.go.SetActive (false);
		}
	}


}
/// <summary>
/// Klasa reprezentująca model stanów animowanych obietków.
/// </summary>
[Serializable]
public class StateModel
{
	/// <summary>
	/// Pole przechowujące numer stanu danego obiektu.
	/// </summary>
	public int stateNumber;
	/// <summary>
	/// Pole przechowujące referencje do animowanego obiektu.
	/// </summary>
	public GameObject go;
}
