using System;
using UnityEngine;
/// <summary>
/// Klasa odpowiedzialna za obsługę mechaniki punktów zdrowia i życia przeciwników.
/// </summary>
public class EnemyHealth : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole przechowujące wartość punktów zdrowia przeciwnika.
    /// </summary>
    [SerializeField] float health = 100f;
    /// <summary>
    /// Pole przechowujące referencje do obiektu głównej klasy sztucznej inteligencji, tj. kontrolera zachowania przeciwników.
    /// </summary>
    [SerializeField] EnemyAI enemyController;
    /// <summary>
    /// Pole przechowujące informacje, czy przeciwnik jest martwy.
    /// </summary>
    bool isDead = false;
    /// <summary>
    /// Metoda zwracająca informacje, czy przeciwnik jest martwy.
    /// </summary>
    /// <returns> Informacje, czy przeciwnik jest martwy. </returns>
    public bool IsDead()
    {
        return isDead;
    }
    /// <summary>
    /// Metoda odpowiedzialna za prawidłowe otrzymywanie obrażeń przez przeciwnika, a także modyfikacji aktualnej liczby punktów zdrowia.
    /// </summary>
    /// <param name="hitPoints"> Punkty obrażeń otrzymywanych przez przeciwnika. </param>
    public void TakeDamage(float hitPoints)
    {
        if (health > 0)
        {
            health -= hitPoints;
            BroadcastMessage("OnDamageTaken", SendMessageOptions.DontRequireReceiver);
        }
        if (health <= 0)
        {
            Die();
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za śmierć przeciwnika i aktywację odpowiednich animacji z tym związanych.
    /// </summary>
    private void Die()
    {
        if (!isDead)
        {
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponentInChildren<BoxCollider>().enabled = false;
        }
        isDead = true;
    }
    /// <summary>
    /// Metoda odpowiedzialna za prawidłowe "wskrzeszenie" obiektu przeciwnika w momencie, gdy zostałby wczytany stan gry w którym dany przeciwnik jeszcze żył, z punktu w grze w którym został zabity.
    /// </summary>
    private void Ressurect()
    {
        GetComponent<Animator>().ResetTrigger("move");
        GetComponent<Animator>().ResetTrigger("die");
        GetComponent<Animator>().SetTrigger("idle");
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponentInChildren<BoxCollider>().enabled = true;
        enemyController.enabled = true;
        enemyController.navMeshAgent.enabled = true;
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pola z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pola które zostają zapisane w pliku. </returns>
    public object SaveState()
    {
        return new SaveData()
        {
            isDead = this.isDead,
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z
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
        this.isDead = saveData.isDead;
        Vector3 loadedPosition = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        Physics.IgnoreLayerCollision(0, 0, true);
        Physics.IgnoreLayerCollision(6, 0, true);
        enemyController.navMeshAgent.Warp(loadedPosition);
        Physics.IgnoreLayerCollision(0, 0, false);
        Physics.IgnoreLayerCollision(6, 0, false);
        if (isDead)
        {
            isDead = false;
            Die();
        }
        else
        {
            Ressurect();
        }

    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public bool isDead;
        public float positionX;
        public float positionY;
        public float positionZ;

    }
}
