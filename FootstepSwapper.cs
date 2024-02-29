using UnityEngine;
using System;
using MovementController;
/// <summary>
/// Klasa odpowiedzialna za zamianę dźwięku kroków wydawanych przez gracza w zależności od podłoża.
/// </summary>
public class FootstepSwapper : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole zawierające referencje do obiektu klasy kontrolera postaci gracza.
    /// </summary>
    [SerializeField] PlayerController controller;
    /// <summary>
    /// Pole przechowujące kolekcje dźwięków kroków dla różnych podłoży.
    /// </summary>
    [SerializeField] FootstepCollection[] terrainFootstepCollection;
    /// <summary>
    /// Pole przechowujące informacje na jakiej warstwie aktualnie gracz się znajduje.
    /// </summary>
    LayerMask currLayer;
    /// <summary>
    /// Pole przechowujące informacje o obiekcie w którego uderzyły promienie emitowane przez gracza.
    /// </summary>
    RaycastHit info;
    /// <summary>
    /// Pole przechowujące informacje o tym, czy gracz przekroczył granice pomiędzy dwoma podłożami, dla których wydawane są różne dźwięki kroków.
    /// </summary>
    public bool isIn = false;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Sprawdza ona na jakiej warstwie podłoża znajduje się gracz.
    /// </summary>
    private void Start()
    {
        if (Physics.Raycast(controller.transform.position, -Vector3.up, out info))
        {
            currLayer = info.transform.gameObject.layer;
        }
    }
    /// <summary>
    /// Metoda wykonująca się co klatkę, sprawdza ona po jakim podłożu stąpa gracz 
    /// i jeżeli zaszła w tej kwestii zmiana, to wywołuje metode podmieniającą kolekcję dźwięków kroków.
    /// </summary>
    private void Update()
    {
        if (controller.isActiveAndEnabled)
        {
            if (Physics.Raycast(controller.transform.position, -Vector3.up, out info) && controller.characterController.isGrounded)
            {
                if (info.transform.gameObject.layer != currLayer)
                {
                    currLayer = info.transform.gameObject.layer;
                    SwapCollection();
                }

            }
        }
    }
    /// <summary>
    /// Metoda podmieniająca dźwięki kroków w zależności od podłoża na jakim się znajduje
    /// </summary>
    /// <param name="loading"> Parametr informujący, czy metoda została wywołana po wczytaniu gry, czy podczas samej rozgrywki</param>
    public void SwapCollection(bool loading = false)
    {
        if (!loading)
            isIn = !isIn;
        foreach (FootstepCollection collection in terrainFootstepCollection)
        {
            if (isIn && collection.name == "Floor")
                controller.SwapFootsteps(collection);
            else if (!isIn && collection.name == "Forest")
                controller.SwapFootsteps(collection);
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pole z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            isIn = this.isIn
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
        this.isIn = saveData.isIn;
        SwapCollection(true);
    }
    /// <summary>
    /// Struktura określająca pole, które powinno zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public bool isIn;
    }

}
