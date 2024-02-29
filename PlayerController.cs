using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
/// <summary>
/// Przestrzeń nazw funkcjonalności związanych z kontrolerem poruszania postaci gracza.
/// </summary>
namespace MovementController
{
    [RequireComponent(typeof(CharacterController))]
    /// <summary>
    /// Największa klasa w aplikacji. Odpowiada w pełni za mechanikę poruszania się, a także wydawania dźwięków z tym związanych.
    /// </summary>
    public class PlayerController : MonoBehaviour, ISaveable
    {
        /// <summary>
        /// Pole przechowujące referencje do obrazka wyświetlającego w interfejsie użytkownika status zmęczenia postaci po zbyt długim sprincie.
        /// </summary>
        [SerializeField] Image noRunningImg;
        /// <summary>
        /// Pole przechowujące informacje o kolorze startowym obrazka zmęczenia sprintem.
        /// </summary>
        public Color startColor = Color.white;
        /// <summary>
        /// Pole przechowujące informacje o kolorze końcowym obrazka zmęczenia sprintem.
        /// </summary>
        public Color endColor = Color.black;
        /// <summary>
        /// Pole przechowujące informacje używaną do obliczania częstotliwości migania obrazka zmęcznia sprintem.
        /// </summary>
        [Range(0, 10)]
        public float speed = 1;
        /// <summary>
        /// Pole zawierające referencje do obiektu klasy odpowiedzialnej za mechanikę punktów życia gracza.
        /// Potrzebna jest w tej klasie do odpowiedniej implementacji obrażen w wyniku upadku z wysokości.
        /// </summary>
        private PlayerHealth playerHealth = null;
        [Header("PlayerController")]
        /// <summary>
        /// Pole zawierające referencje do obiektu kamery postaci gracza.
        /// </summary>
        [SerializeField] public Transform Camera;
        /// <summary>
        /// Pole zawierające wartość prędkości ruchu postaci gracza.
        /// </summary>
        [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
        /// <summary>
        /// Pole zawierające wartość prędkości ruchu postaci gracza w momencie kucania.
        /// </summary>
        [Range(0.1f, 5)] public float CrouchSpeed = 1.0f;
        /// <summary>
        /// Pole zawierające wartość prędkości ruchu postaci gracza w momencie sprintu.
        /// </summary>
        [SerializeField, Range(2, 20)] float RuningSpeed = 4.0f;
        /// <summary>
        /// Pole zawierające wartość prędkości ruchu postaci gracza w momencie skoku.
        /// </summary>
        [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
        /// <summary>
        /// Pole zawierające wartość prędkości ruchu kamery postaci gracza.
        /// </summary>
        [SerializeField, Range(0.5f, 10)] public float lookSpeed = 2.0f;
        /// <summary>
        /// Pole zawierające wartość maksymalną na jaką można obrócić kamerę w osi X
        /// </summary>
        [SerializeField, Range(10, 120)] float lookXLimit = 80.0f;
        [Space(20)]
        [Header("Advance")]
        /// <summary>
        /// Pole zawierające wartość pola widzenia kamery gracza w momencie sprintu.
        /// </summary>
        [SerializeField] public float RunningFOV = 65.0f;
        /// <summary>
        /// Pole zawierające wartość prędkości zmiany pola widzenia kamery gracza w momencie rozpoczęcia sprintu.
        /// </summary>
        [SerializeField] float SpeedToFOV = 4.0f;
        /// <summary>
        /// Pole określające wartość wysokości kamery postaci gracza w momencie kucania.
        /// </summary>
        [SerializeField] float CrouchHeight = 1.0f;
        /// <summary>
        /// Pole zawierające wartość grawitacji wpływającej na postać gracza.
        /// </summary>
        [SerializeField] float gravity = 20.0f;
        /// <summary>
        /// Pole zawierające wartość czasu potrzebną dla postaci gracza do przejścia w sprint.
        /// </summary>
        [SerializeField] float timeToRunning = 2.0f;
        /// <summary>
        /// Pole zawierające informację logiczną, czy postać gracza może się w danym momencie poruszać.
        /// </summary>
        [HideInInspector] public bool canMove = true;
        /// <summary>
        /// Pole zawierające informację logiczną, czy postać gracza może w danym momencie sprintować.
        /// </summary>
        [HideInInspector] public bool CanRunning = true;
        [Header("SFX")]
        /// <summary>
        /// Lista zawierająca różne dźwięku kroków odtwarzane w czasie chodzenia postaci gracza.
        /// </summary>
        /// <typeparam name="AudioClip"> Pojedyńczy odgłos kroku.</typeparam>
        [SerializeField] private List<AudioClip> m_FootstepSounds = new List<AudioClip>();
        /// <summary>
        /// Pole zawierajace referencje do obiektu odgłosu skoku postaci gracza.
        /// </summary>
        [SerializeField] private AudioClip m_JumpSound;
        /// <summary>
        /// Pole zawierajace referencje do obiektu odgłosu lądowania po skosku postaci gracza.
        /// </summary>
        [SerializeField] private AudioClip m_LandSound;
        /// <summary>
        /// Pole zawierajace referencje do obiektu odgłosu postaci w momencie zmęczenia sprintem.
        /// </summary>
        [SerializeField] private AudioClip sighingSound;
        /// <summary>
        /// Pole zawierajace referencje do obiektu odgłosu postaci w momencie upadku z dużej wysokości powodującego obrażenia.
        /// </summary>
        [SerializeField] private AudioClip fallDmgSound;
        /// <summary>
        /// Pole zawierające referencje do obiektu odtwarzającego dźwięki postaci gracza.
        /// </summary>
        private AudioSource m_AudioSource;
        /// <summary>
        /// Pole zawierające informacje logiczną mówiącą o tym, czy postać gracza wykonała kolejny krok i powinien być odtworzony kolejny dźwięk.
        /// </summary>
        private bool isNextStep = true;
        [Space(20)]
        [Header("Input")]
        /// <summary>
        /// Pole zawierające informacje o klawiszu służącym do kucania.
        /// </summary>
        [SerializeField] KeyCode CroughKey = KeyCode.LeftControl;
        /// <summary>
        /// Pole zawierające referencje do obiektu kontrolera postaci kluczowego dla poprawnego działania mechaniki poruszania zaimplementowej w tej klasie.
        /// </summary>
        [HideInInspector] public CharacterController characterController;
        /// <summary>
        /// Pole zawierające współrzędne świadczące o kierunku poruszania się postaci gracza.
        /// </summary>
        [HideInInspector] public Vector3 moveDirection = Vector3.zero;
        /// <summary>
        /// Pole zawierające informacje logiczną mówiącą o tym, czy postać znajduje się w stanie kucania.
        /// </summary>
        bool isCrouch = false;
        /// <summary>
        /// Pole zawierające wartość domyślnej wysokości kamery postaci gracza w czasie kucania.
        /// </summary>
        float InstallCroughHeight;
        /// <summary>
        /// Pole zawierające wartość kluczową do poprawnej rotacji kamery gracza.
        /// </summary>
        float rotationX = 0;
        /// <summary>
        /// Pole zawierające informację logiczną mówiącą o tym, czy postać gracza sprintuje.
        /// </summary>
        [HideInInspector] public bool isRunning = false;
        /// <summary>
        /// Pole zawierające wartość domyślnego pola widzenia kamery gracza.
        /// </summary>
        public float InstallFOV;
        /// <summary>
        /// Pole zawierające referencje do obiektu kamer postaci gracza, której wartości są modyfikowane w skrypcie.
        /// </summary>
        Camera cam;
        /// <summary>
        /// Pole zawierające informację logiczną o tym, czy postać gracza znajduje się w ruchu.
        /// </summary>
        [HideInInspector] public bool Moving;
        /// <summary>
        /// Pole zawierające wartość używaną do obliczenia prędkości ruchu postaci gracza w pionie.
        /// </summary>
        [HideInInspector] public float vertical;
        /// <summary>
        /// Pole przechowujące wartość używaną do obliczenia prędkości ruchu postaci gracza w poziomie.
        /// </summary>
        [HideInInspector] public float horizontal;
        /// <summary>
        /// Pole zawierające wartość używaną do obliczenia prędkości rotacji kamery postagi gracza w pionie.
        /// </summary>
        [HideInInspector] public float Lookvertical;
        /// <summary>
        /// Pole zawierające wartość używaną do obliczenia prędkości rotacji kamery postagi gracza w poziomie.
        /// </summary>
        [HideInInspector] public float Lookhorizontal;
        /// <summary>
        /// Pole zawierające wartość prędkości sprintu gracza, wartość ta może być modyfikowana w skrypcie.
        /// </summary>
        float RunningValue;
        /// <summary>
        /// Pole zawierające informacje logiczną świadczącą o odwróceniu osi Y rotacji kamery postaci gracza.
        /// </summary>
        public bool flipY = false;
        /// <summary>
        /// Pole zawierające wartość prędkości chodu postaci gracza. Wartość ta może być modyfikowana w skrypcie. 
        /// </summary>
        [HideInInspector] public float WalkingValue;
        /// <summary>
        /// Pole zawierające współrzędne wskazujące na pozycje postaci gracza odczytaną z pliku zapisu.
        /// </summary>
        private Vector3 loadedPosition = Vector3.zero;
        /// <summary>
        /// Pole zawierające informacje o aktualnym czasie sprintu postaci gracza.
        /// </summary>
        float sprintTime = 0;
        /// <summary>
        /// Pole zawierające informacje logiczną o tym, czy postać gracza jest zmęczona sprintem i nie może dalej biec.
        /// </summary>
        private bool isExhausted = false;
        /// <summary>
        /// Pole zawierające informacje logiczną mówiącą o tym, czy postać gracza dotyka ziemi.
        /// </summary>
        private bool wasGrounded = true;
        /// <summary>
        /// Pole zawierające informacje logiczczną o tym, czy postać gracza w poprzedniej klatce była w stanie spadku swobodnego.
        /// </summary>
        private bool wasFalling = false;
        /// <summary>
        /// Pole zawierające wartość dystansu spadku swobodnego przebytego przez postać gracza.
        /// </summary>
        private float fallDistance = 0;
        /// <summary>
        /// Pole zawierające wartość wysokości na jakiej postać gracza rozpoczęła spadek swobodny.
        /// </summary>
        private float fallingStart = 0;
        /// <summary>
        /// Metoda wywoływana tylko w pierwszej klatce gry. Ma w niej miejsce inicjalizacja wszystkich wymagających tego pól klasy.
        /// </summary>
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            cam = GetComponentInChildren<Camera>();
            m_AudioSource = GetComponent<AudioSource>();
            playerHealth = GetComponent<PlayerHealth>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InstallCroughHeight = characterController.height;
            InstallFOV = cam.fieldOfView;
            RunningValue = RuningSpeed;
            WalkingValue = walkingSpeed;
            noRunningImg.enabled = false;
        }
        /// <summary>
        /// Najważniejsza metoda klasy, wywoływana co klatkę. Ma w niej miejsce przede wszystkim zdeterminowanie kierunku ruchu postaci gracza w oparciu o dane wejściowe,
        /// oraz obliczenie jego prędkości. Obliczany jest także czas sprintu, gdzie po przekroczeniu pewnej granicy postać przechodzi w stan zmęczenia i nie może więcej sprintować.
        /// Determinowana i modyfikowana jest również pozycja kamery postaci gracza, oraz jej pole widzenia.
        /// Dodatkowo wywoływane są pomocnicze metody odtwarzające odpowiednie dźwięki ruchu, tudzież wyświetlające obrazek zmęczenia postaci.
        /// </summary>
        void Update()
        {

            RaycastHit CroughCheck;
            if (isExhausted)
                ShowExhaustedImage();
            else
                noRunningImg.enabled = false;
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            isRunning = !isCrouch ? CanRunning ? Input.GetKey(KeyCode.LeftShift) : false : false;
            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Vertical") : 0;
            horizontal = canMove ? (isRunning ? (RunningValue) : (WalkingValue)) * Input.GetAxis("Horizontal") : 0;
            if (isRunning) RunningValue = Mathf.Lerp(RunningValue, RuningSpeed, timeToRunning * Time.deltaTime);
            else RunningValue = WalkingValue;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * vertical) + (right * horizontal);
            if (horizontal != 0)
                moveDirection = moveDirection * 0.75f;
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
                PlayJumpSound();
                StartCoroutine(PlayLandingSound());
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (isRunning)
            {
                sprintTime += Time.deltaTime;
            }
            else
            {
                if (sprintTime > 0)
                    sprintTime -= Time.deltaTime;
                if (sprintTime <= 5f)
                    isExhausted = false;
            }
            if (sprintTime > 10)
            {
                m_AudioSource.PlayOneShot(sighingSound);
                CanRunning = false;
                isExhausted = true;
            }
            else if (!isExhausted)
            {
                CanRunning = true;
            }
            characterController.Move(moveDirection * Time.deltaTime);
            Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;
            if (Moving && characterController.isGrounded)
                PlayFootStepAudio();
            if (Cursor.lockState == CursorLockMode.Locked && canMove)
            {
                Lookvertical = -Input.GetAxis("Mouse Y") * (flipY ? -1 : 1);
                Lookhorizontal = Input.GetAxis("Mouse X");

                rotationX += Lookvertical * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Lookhorizontal * lookSpeed, 0);


                if (isRunning && Moving) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
                else cam.fieldOfView = Mathf.Lerp(InstallFOV, cam.fieldOfView, SpeedToFOV * Time.deltaTime);
            }

            if (Input.GetKey(CroughKey))
            {
                isCrouch = true;
                float Height = Mathf.Lerp(characterController.height, CrouchHeight, 5 * Time.deltaTime);
                characterController.height = Height;
                WalkingValue = Mathf.Lerp(WalkingValue, CrouchSpeed, 6 * Time.deltaTime);

            }
            else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
            {
                if (characterController.height != InstallCroughHeight)
                {
                    isCrouch = false;
                    float Height = Mathf.Lerp(characterController.height, InstallCroughHeight, 6 * Time.deltaTime);
                    characterController.height = Height;
                    WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
                }
            }

        }
        /// <summary>
        /// Metoda odpowiedzialna za wyświetlenie obrazka świadczącego o zmęczeniu sprintem postaci gracza.
        /// </summary>
        private void ShowExhaustedImage()
        {
            noRunningImg.enabled = true;
            noRunningImg.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
        }
        /// <summary>
        /// Metoda używana do obliczania dystansu spadku swobodnego postaci gracza.
        /// Ponad to wywołuje ona metodę innej klasy odbierającej punkty życia postaci w przypadku wystarczająco długiego upadku.
        /// </summary>
        private void FixedUpdate()
        {
            if (!characterController.isGrounded)
            {
                if (!wasFalling)
                {
                    wasFalling = true;
                    fallingStart = Math.Abs(transform.position.y);
                }
                float tmp = transform.position.y;
                if (tmp > fallingStart)
                    fallingStart = tmp;
            }
            if (!wasGrounded && characterController.isGrounded)
            {
                fallDistance = Math.Abs(transform.position.y - fallingStart);
                if (fallDistance > 5)
                {
                    playerHealth.TakeDamage(2 * fallDistance);
                    m_AudioSource.PlayOneShot(fallDmgSound);
                }
                fallDistance = 0;
                fallingStart = 0;
                wasFalling = false;
            }
            wasGrounded = characterController.isGrounded;
        }
        /// <summary>
        /// Metoda odpowiedzialna za podmienianie odgłosów kroków postaci w przypadku zmiany podłoża.
        /// </summary>
        /// <param name="collection"> Kolekcja zawierająca dźwięki kroków postaci na określonej powierzchni.</param>
        public void SwapFootsteps(FootstepCollection collection)
        {
            m_FootstepSounds.Clear();
            for (int i = 0; i < collection.footstepSounds.Count; i++)
            {
                m_FootstepSounds.Add(collection.footstepSounds[i]);
            }
        }
        /// <summary>
        /// Metoda odpowiedzialna za odtwarzanie różnych (losowych z danej kolekcji) odgłosów kroków postaci.
        /// Wywołuje ona również metodę obliczającą czas do odtworzenia następnego dźwięku kroku/
        /// </summary>
        private void PlayFootStepAudio()
        {
            if (!isNextStep) return;
            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Count);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            isNextStep = false;

            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
            StartCoroutine(CountToNextStep());
        }
        /// <summary>
        /// Metoda odpowiedzialna za odtwarzanie dźwięku skoku postaci gracza.
        /// </summary>
        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }
        /// <summary>
        /// Metoda odpowiedzialna za odczekanie określonej długości czasu pomiędzy poszczególnymi odgłosami kroków postaci.
        /// Czas ten różni się w zależności od sposobu poruszania się postaci, tj. kucanie, sprint, lub chód.
        /// </summary>
        /// <returns> Czeka określoną długość czasu.</returns>
        IEnumerator CountToNextStep()
        {
            if (isRunning)
                yield return new WaitForSeconds(0.25f);
            else if (isCrouch)
                yield return new WaitForSeconds(1f);
            else if (!isRunning && !isCrouch)
                yield return new WaitForSeconds(0.5f);
            isNextStep = true;
        }
        /// <summary>
        /// Metoda odpowiedzialna za odtwarzanie odgłosu lądowania postaci gracza po skoku
        /// </summary>
        /// <returns> Czeka niespełna pół sekundy od skoku.</returns>
        IEnumerator PlayLandingSound()
        {
            yield return new WaitForSeconds(0.45f);
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();

        }
        /// <summary>
        /// Metoda odpowiedzialna za określenie, które pola z tego skryptu powinno być zapisane.
        /// </summary>
        /// <returns> Obiekt zawierający pola które zostają zapisane w pliku. </returns>
        public object SaveState()
        {
            return new SaveData()
            {
                positionX = transform.position.x,
                positionY = transform.position.y,
                positionZ = transform.position.z
            };
        }
        /// <summary>
        /// Metoda odpowiedzialna za wczytanie wcześniej zapisanego stanu obiektu obsługiwanego przez skrypt i wykonanie
        /// operacji mających na celu przywrócenie obiektu postaci gracza do stanu zapisanego.
        /// </summary>
        /// <param name="state"> Obiekt przechowujący zapisany stan pól ze skryptów gry.</param>
        public void LoadState(object state)
        {
            var saveData = (SaveData)state;
            loadedPosition = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
            Physics.IgnoreLayerCollision(0, 3, true);
            Physics.IgnoreLayerCollision(6, 3, true);
            Physics.IgnoreLayerCollision(7, 3, true);
            characterController.Move(CalculateDirection(loadedPosition));
            if (Vector3.Distance(transform.position, loadedPosition) < 1)
            {
                Physics.IgnoreLayerCollision(0, 3, false);
                Physics.IgnoreLayerCollision(6, 3, false);
                Physics.IgnoreLayerCollision(7, 3, false);
                return;
            }
            else //Tutaj robimy to samo, aby postać na pewno została prawidłowo przeteleportowana do zapisanego miejsca.
            {
                characterController.Move(CalculateDirection(loadedPosition));
                Physics.IgnoreLayerCollision(0, 3, false);
                Physics.IgnoreLayerCollision(6, 3, false);
                Physics.IgnoreLayerCollision(7, 3, false);
            }
        }
        /// <summary>
        /// Metoda używana do obliczenia wektora odległości o jaki powinna zostać przemieszczona postać gracza,
        /// aby znaleźć się w pozycji zapisanej, która została wczytana.
        /// </summary>
        /// <param name="loadedPosition"> Pozycja odczytana z pliku zapisu.</param>
        /// <returns> Współrzędne o jakie należy przesunąć postać gracza.</returns>
        private Vector3 CalculateDirection(Vector3 loadedPosition)
        {
            var tmpX = loadedPosition.x - transform.position.x;
            var tmpY = loadedPosition.y - transform.position.y;
            var tmpZ = loadedPosition.z - transform.position.z;
            return new Vector3(tmpX, tmpY, tmpZ);
        }
        /// <summary>
        /// Struktura określająca pola, które powinny zostać zapisane.
        /// </summary>
        [Serializable]
        private struct SaveData
        {
            public float positionX;
            public float positionY;
            public float positionZ;
        }

    }
}