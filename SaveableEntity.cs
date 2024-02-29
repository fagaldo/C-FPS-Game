using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Klasa obsługująca system zapisywania i wczytywania stanów rozgrywki.
/// </summary>
public class SaveableEntity : MonoBehaviour
{
    /// <summary>
    /// Pole przechowujące ID obiektu w grze zapisującego swój stan.
    /// </summary>
    [SerializeField] private string id;
    /// <summary>
    /// Pole przechowujące ID obiektu w grze zapisującego swój stan.
    /// </summary>
    public string Id => id;
    /// <summary>
    /// Metoda generująca ID dla obiektu w grze z poziomu silnika.
    /// </summary>
    [ContextMenu("Generate ID")]
    private void GenerateId()
    {
        id = Guid.NewGuid().ToString();
    }
    /// <summary>
    /// Metoda iterująca po wszystkich obiektach klas rozszerzających interfejs ISaveable,
    /// czyli takich, które zapisują swoje pola do obiektu zapisywanego w pliku. Dla każdego obiektu pobiera ona informacje,
    ///  które mają zostać zapisane i umieszcza je w odpowiednim obiekcie.
    /// </summary>
    /// <returns> Obiekt przechowujący zapisane w grze informacje.</returns>
    public object SaveState()
    {
        var state = new Dictionary<string, object>();
        foreach (var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.SaveState();
        }
        return state;
    }
    /// <summary>
    /// Metoda iterująca po wszystkich obiektach klas rozszerzających interfejs ISaveable,
    /// czyli takich, które zapisują swoje pola do pliku. Dla każdego obiektu wywołuje ona funkcje wczytającą 
    /// zapisane wcześniej stany pól danych klas.
    /// </summary>
    /// <param name="state"> Obiekt przechowujące zapisane w grze informacje.</param>
    public void LoadState(object state)
    {
        var stateDictionary = (Dictionary<string, object>)state;
        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();
            if (stateDictionary.TryGetValue(typeName, out object savedState))
            {
                saveable.LoadState(savedState);
            }
        }
    }
}
