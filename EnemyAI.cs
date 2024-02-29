using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Główna klasa odpowiedzialna za obsługę mechaniki działania sztucznej inteligencji przeciwnika.
/// </summary>
public class EnemyAI : MonoBehaviour, ISaveable
{
    /// <summary>
    /// Pole zawierające wartość zasięgu w którym przeciwnik będzie gonił gracza.
    /// </summary>
    [SerializeField] float chaseRange = 7f;
    /// <summary>
    /// Pole zawierające wartość prędkości z jaką obracają się przeciwnicy w stronę gracza.
    /// </summary>
    [SerializeField] float turnSpeed = 5f;
    /// <summary>
    /// Pole przechowujące obiekt klasy odpowiedzialnej za symulacje pola widzenia wrogów.
    /// </summary>
    EnemyFieldOfView enemyEyes;
    /// <summary>
    /// Pole przechowujące obiekt klasy sterującej ruchem przeciwników po siatce na danym poziomie.
    /// </summary>
    public NavMeshAgent navMeshAgent;
    /// <summary>
    /// Pole przechowujące obiekt klasy obsługującej system zdrowia przeciwników.
    /// </summary>
    EnemyHealth healthController;
    /// <summary>
    /// Pole przechowujące wartość odległości od przeciwnika do gracza.
    /// </summary>
    float distanceToTarget = Mathf.Infinity;
    /// <summary>
    /// Pole przechowujące informacje, czy przeciwnik został sprowokowany przez gracza.
    /// </summary>
    bool isProvoked = false;
    /// <summary>
    /// Pole przechowujące informacje, czy gracz oddalił się na tyle od przeciwnika, że ten może przejść w stan "jałowy".
    /// </summary>
    bool canChill = true;
    /// <summary>
    /// Pole przechowujące początkową pozycje przeciwnika.
    /// </summary>
    Vector3 startingPos;
    /// <summary>
    /// Pole przechowujące początkową rotacje przeciwnika.
    /// </summary>
    Vector3 startingRot;
    /// <summary>
    /// Pole przechowujące referencje do obiektu gracza.
    /// </summary>
    Transform target;
    /// <summary>
    /// Pole przechowujące informacje, czy przeciwnik znajduje się w pozycji początkowej.
    /// </summary>
    bool hasReturned = true;
    /// <summary>
    /// Pole przechowujące obiekt klasy menadżera przerywnika filmowego.
    /// </summary>
    ManageCutscene cutsceneManager;
    /// <summary>
    /// Pole przechowujące referencje do obiektu animatora.
    /// </summary>
    Animator animator;
    /// <summary>
    /// Metoda wykonywana tylko w pierwszej klatce gry. Ma w niej miejsce wywołanie metody inicjalizacji wymagających tego pól.
    /// </summary>
    void Start()
    {
        cutsceneManager = FindObjectOfType<ManageCutscene>();
        StartCoroutine(Inicialize());
    }
    /// <summary>
    /// Metoda inicjalizująca wszystkie wymagające tego pola poprzez uzyskanie referencji do innych obiektów w grze.
    /// Robi to w momencie zakończenia przerywnika filmowego.
    /// </summary>
    /// <returns> Czeka aż przerywnik filmowy zostanie zakończony. </returns>
    IEnumerator Inicialize()
    {
        healthController = GetComponent<EnemyHealth>();
        enemyEyes = GetComponentInChildren<EnemyFieldOfView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        yield return new WaitUntil(cutsceneManager.HasFinished);
        target = FindObjectOfType<PlayerHealth>().transform;
        startingPos = transform.position;
        startingRot = transform.rotation.eulerAngles;
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Metoda wykonująca się co klatkę, sprawdza ona warunki dla zauważenia i ataku przeciwnika, a także dla odpuszczenia pogoni i powrotu do pozycji początkowej.
    /// Dodatkowo w przypadku śmierci wroga wyłącza ona skrypt dla danego obiektu.
    /// </summary>
    void Update()
    {
        if (cutsceneManager.HasFinished() && target)
        {
            if (healthController.IsDead())
            {
                enabled = false;
                navMeshAgent.enabled = false;
                return;
            }
            if (hasReturned && transform.rotation.eulerAngles != startingRot)
                FaceDefault();
            if (enemyEyes.canSeePlayer)
                SpottedPlayer();
                
            distanceToTarget = Vector3.Distance(target.position, transform.position);

            if (isProvoked) EngageTarget();

            else if (distanceToTarget <= chaseRange)
            {
                isProvoked = true;
            }

            if (distanceToTarget > 1.5 * chaseRange && canChill && !hasReturned)
            {
                ReturnToPos();
            }
        }

    }
    /// <summary>
    /// Metoda obsługująca przypadek w którym przeciwnik zauważa gracza i przechodzi w stan sprowokowania.
    /// </summary>
    private void SpottedPlayer()
    {
        isProvoked = true;
        canChill = false;
        if (distanceToTarget > 2 * chaseRange)
            StartCoroutine(CountToChill(6.5f));
    }
    /// <summary>
    /// Metoda obsługująca przypadek w którym przeciwnik jest atakowany przez gracza i przechodzi w stan sprowokowania.
    /// </summary>
    public void OnDamageTaken()
    {
        isProvoked = true;
        canChill = false;
        if (distanceToTarget > 2 * chaseRange)
            StartCoroutine(CountToChill(6.5f));
    }
    /// <summary>
    /// Metoda obsługująca przypadek w którym przeciwnik jest w niedalekiej odległości od gracza i po usłyszeniu wystrzałów przechodzi w stan sprowokowania.
    /// </summary>
    public void OnShotsFired()
    {
        if (distanceToTarget < 2.5 * chaseRange)
        {
            isProvoked = true;
            canChill = false;
        }
        if (distanceToTarget > 2 * chaseRange)
            StartCoroutine(CountToChill(6.5f));
    }
    /// <summary>
    /// Metoda odliczająca czas po którym przeciwnik może przejść w stan "jałowy" i przestać gonić gracza.
    /// </summary>
    /// <param name="duration"> Określa czas po jakim przeciwnik może przejść w stan "jałowy" i przestać gonić gracza.</param>
    /// <returns> Czas po jakim przeciwnik może przejść w stan "jałowy" i przestać gonić gracza.</returns>
    IEnumerator CountToChill(float duration)
    {
        yield return new WaitForSeconds(duration);
        canChill = true;
    }
    /// <summary>
    /// Metoda obsługująca sytuacje w której przeciwnik został sprowokowany i atakuje gracza. Jeżeli ten jest w zasięgu, atakuje go, jeżeli nie jest, goni go.
    /// </summary>
    private void EngageTarget()
    {
        FaceTarget();
        hasReturned = false;
        if (isAttackable())
        {
            AttackTarget();
        }
        else
        {
            ChaseTarget();
        }
    }
    /// <summary>
    /// Metoda określająca, czy gracz jest w zasięgu ataku przeciwnika.
    /// </summary>
    /// <returns> Zwraca logiczny stan, czy gracz jest w zasięgu ataku.</returns>
    private bool isAttackable()
    {
        if (distanceToTarget <= navMeshAgent.stoppingDistance)
            return true;
        else return false;
    }
    /// <summary>
    /// Metoda odpowiedzialna za aktywację animacji ataku przeciwnika.
    /// </summary>
    private void AttackTarget()
    {
        animator.SetBool("attack", true);
        animator.ResetTrigger("move");
    }
    /// <summary>
    /// Metoda odpowiedzialna za pogoń przeciwnika za graczem, oraz aktywacją odpowiedniej animacji.
    /// </summary>
    private void ChaseTarget()
    {
        animator.SetBool("attack", false);
        animator.ResetTrigger("idle");
        animator.SetTrigger("move");
        navMeshAgent.SetDestination(target.position);
        if (distanceToTarget > 2 * chaseRange)
            StartCoroutine(CountToChill(6.5f));
    }
    /// <summary>
    /// Metoda odpowiedzialna za powrót przeciwnika do pozycji początkowej.
    /// </summary>
    private void ReturnToPos()
    {
        if (Vector3.Distance(startingPos, transform.position) < 3)
        {
            animator.ResetTrigger("move");
            animator.SetTrigger("idle");
            FaceDefault();
            hasReturned = true;
        }
        else
        {
            animator.SetTrigger("move");
            isProvoked = false;
            navMeshAgent.SetDestination(startingPos);
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za obrót przeciwnika w stronę gracza w przypadku sprowokowania.
    /// </summary>
    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
    /// <summary>
    /// Metoda odpowiedzialna za obrót przeciwnika do rotacji początkowej.
    /// </summary>
    private void FaceDefault()
    {
        transform.rotation = Quaternion.Euler(startingRot);
    }
    /// <summary>
    /// Metoda odpowiedzialna za określenie, które pole z tego skryptu powinno być zapisane.
    /// </summary>
    /// <returns> Obiekt zawierający pole które zostaje zapisane w pliku.</returns>
    public object SaveState()
    {
        return new SaveData()
        {
            isProvoked = this.isProvoked
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
        this.isProvoked = saveData.isProvoked;
        animator = GetComponent<Animator>();
        if (isActiveAndEnabled)
        {
            if (!healthController)
                healthController = GetComponent<EnemyHealth>();
            if (!isProvoked && !healthController.IsDead())
            {
                animator.ResetTrigger("move");
                animator.SetTrigger("idle");
                navMeshAgent.ResetPath();
            }
        }

    }
    /// <summary>
    /// Struktura określająca pola, które powinny zostać zapisane.
    /// </summary>
    [Serializable]
    private struct SaveData
    {
        public bool isProvoked;
    }
}
