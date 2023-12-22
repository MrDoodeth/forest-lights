using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Audio;
using UnityEngine.Localization;
using Platforms;
using YG;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] public Platform platform;

    [Header("Audio")]
    [SerializeField] private AudioMixerSnapshot inPause;
    [SerializeField] public AudioMixerSnapshot outPause;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip audioDamage;
    [SerializeField] private AudioClip audioPop;
    [SerializeField] private AudioClip audioBushPop;
    [SerializeField] private AudioClip[] audioEat;
    [SerializeField] private AudioClip[] audioChop;
    [SerializeField] private AudioClip audioKnock;
    [SerializeField] private AudioClip audioStep;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.37f;
    [SerializeField] private float RotationSpeed = 3f;
    [SerializeField] private float rotSpeed = 1f;
    [SerializeField] private Transform target;
    [SerializeField] private Transform Bug;
    [SerializeField] private FixedJoystick joystickMovement;
    [SerializeField] private FixedJoystick joystickAttack;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button pickUpButton;
    [SerializeField] private Button HealButton;

    [Header("Hit")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private Transform attackZone;
    [SerializeField] private ParticleSystem fxChop;

    [Header("GUI")]
    [SerializeField] private LocalizedString scoreLoc;
    [SerializeField] private float sprintCost = -0.7f;
    [SerializeField] private Image progressBar_Stamina;
    [SerializeField] private Image progressBar_Altars;
    [SerializeField] private Image progressBar_Health;
    [SerializeField] private TextMeshProUGUI woodAmountCounter;
    [SerializeField] private TextMeshProUGUI berryAmountCounter;
    [SerializeField] private TextMeshProUGUI killAmountCounter;
    [SerializeField, Range(0.01f, 1f)] private float maxHealth = 1;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;

    [Header("Layers")]
    [SerializeField] private LayerMask berryLayer;
    [SerializeField] private LayerMask treeLayer;
    [SerializeField] private LayerMask stumpLayer;
    [SerializeField] private LayerMask altarLayer;
    [SerializeField] private LayerMask bearLayer;

    [SerializeField] private PlayableDirector director;

    private float vInput;
    private float hInput;
    private float isMove;
    private float Stamina;
    private float altarCounter;
    private int killCounter;
    private float currentHealth;
    private bool isPause = false;
    private bool isAlive = true;
    private float selectionRate = 0.8f, hitRate = 1.06f;
    private float nextSelectionTime = 0f, nextHitTime = 0f;

    private int woodAmount;
    private int berryAmount;

    private float Speed;
    private Rigidbody _rb;
    private Vector3 Move = new Vector3();

    
    private void UpdateText(string text)
    {
        killAmountCounter.text = text;
    }

    public void PauseButton()
    {
        isPause = !isPause;
        if (isPause)
        {
            PauseOn();
        }
        else
        {
            PauseOff();
        }
    }
    private void PauseOn()
    {
        isAlive = false;
        inPause.TransitionTo(0.2f);
        Time.timeScale = 0;
        menuPanel.SetActive(true);
    }
    private void PauseOff()
    {
        isAlive = true;
        outPause.TransitionTo(0.2f);
        Time.timeScale = 1;
        menuPanel.SetActive(false);
    }

    public void TakeDamage(float damage, Transform trans)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {           
            EventBus.OnPlayerDied?.Invoke();
        }
        else
        {
            source.PlayOneShot(audioDamage);
            _rb.AddForce((transform.position - trans.position) * 15, ForceMode.Impulse);
        }
    }
    private void Die()
    {
        source.PlayOneShot(loseSound);
        _rb = GetComponent<Rigidbody>();
        EventBus.OnPlayerDied -= Die;
        inPause.TransitionTo(0f);
        _rb.isKinematic = true;
        animator.SetBool("isDie", true);
        isAlive = false;
    }
    public void Respawn()
    {
        EventBus.OnPlayerDied += Die;
        outPause.TransitionTo(0f);
        _rb.isKinematic = false;
        animator.SetBool("isDie", false);
        transform.localRotation = Quaternion.identity;
        transform.position = new Vector3(34f, 0.0777f, 32f);
        currentHealth = maxHealth;
        isAlive = true;
    }
    private void ShowLoseMenu()
    {
        losePanel.SetActive(true);
    }
    private void Step()
    {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(audioStep);
    }
    private void GUI_Update()
    {
        killCounter = Mathf.Clamp(killCounter, 0, 1000);
        scoreLoc.Arguments[0] = killCounter;
        scoreLoc.RefreshString();
        woodAmount = Mathf.Clamp(woodAmount, 0, 10);
        woodAmountCounter.text = woodAmount.ToString();
        berryAmount = Mathf.Clamp(berryAmount, 0, 20);
        berryAmountCounter.text = berryAmount.ToString();
        currentHealth = Mathf.Clamp(currentHealth, 0, 1);
        progressBar_Stamina.fillAmount = Stamina;
        progressBar_Altars.fillAmount = altarCounter;
        progressBar_Health.fillAmount = currentHealth;

    }
    private void BearDie()
    {
        killCounter++;
    }
    private void Chop()
    {
        Collider[] chopHit = Physics.OverlapSphere(attackZone.position, attackRange, treeLayer);
        Collider[] stumpHit = Physics.OverlapSphere(attackZone.position, attackRange, stumpLayer);
        Collider[] bearHit = Physics.OverlapSphere(attackZone.position, attackRange, bearLayer);

        foreach (Collider tree in chopHit) 
        {
            tree.GetComponent<Tree>().TakeDamage(Random.Range(25, 35), transform);
            source.PlayOneShot(audioChop[Random.Range(0, audioChop.Length)]);
            fxChop.Play();
        }
        
        foreach (Collider stump in stumpHit)
        {           
            stump.GetComponent<Stump>().TakeDamage(Random.Range(25, 35));
            source.PlayOneShot(audioChop[Random.Range(0, audioChop.Length)]);
            fxChop.Play();          
        }

        foreach (Collider bear in bearHit)
        {
            bear.GetComponent<BearAI>().TakeDamage(Random.Range(25, 40), transform);
            source.PlayOneShot(audioKnock);
        }
    }
    public void Hit()
    {
        if (Stamina > 0.125 && Time.time >= nextHitTime)
        {
            animator.SetTrigger("isHit");
            Stamina -= 0.11f;
            nextHitTime = Time.time + 1 / hitRate;
        }
    }
    public void SelectionOfBerries()
    {
        if (berryAmount < 20 && Time.time >= nextSelectionTime)
        {
            Collider[] selectionBerry = Physics.OverlapSphere(attackZone.position, attackRange, berryLayer);
            foreach (Collider Bush in selectionBerry)
            {
                _rb.isKinematic = true;
                animator.SetTrigger("isSelection");
                source.PlayOneShot(audioBushPop);
                Bush.GetComponent<Berry_bush>().Selection();
                StartCoroutine(Death());

                IEnumerator Death()
                {
                    yield return new WaitForSeconds(1.380f);
                    berryAmount++;
                    StartCoroutine(berryRespawn());
                    yield return new WaitForSeconds(0.6f);
                    _rb.isKinematic = false;

                    IEnumerator berryRespawn()
                    {
                        yield return new WaitForSeconds(300f);
                        Bush.gameObject.SetActive(true);
                    }
                }
            }
            nextSelectionTime = Time.time + 1 / selectionRate;
        }          
    }
    public void Heal()
    {
        if (currentHealth < 1 && berryAmount > 0 && Time.time >= nextHitTime)
        {
            source.PlayOneShot(audioEat[Random.Range(0, audioEat.Length)]);
            berryAmount -= 1;
            currentHealth += 0.3f;
            nextHitTime = Time.time + 1 / hitRate;
        }
    }
    public void LightAltar()
    {
        if (woodAmount > 9 && Time.time >= nextSelectionTime)
        {
            Collider[] Altar = Physics.OverlapSphere(attackZone.position, attackRange, altarLayer);
            foreach (Collider altar in Altar)
            {
                _rb.isKinematic = true;
                animator.SetTrigger("isSelection");
                altar.GetComponent<Altar>().Fire();
                StartCoroutine(LightUp());

                IEnumerator LightUp()
                {
                    yield return new WaitForSeconds(1f);
                    Bug.localScale = new Vector3(100, 100, 100);
                    woodAmount -= 10;
                    yield return new WaitForSeconds(0.6f);
                    altarCounter += 0.125f;
                    _rb.isKinematic = false;
                }
            }
            nextSelectionTime = Time.time + 1 / selectionRate;
        }
    }
    private void CheckExit()
    {
        if (altarCounter >= 1)
        {
            director.Play();
            EventBus.OnEnd?.Invoke();
            StartCoroutine(CutScene());

            IEnumerator CutScene()
            {               
                _rb.isKinematic = true;
                yield return new WaitForSeconds(10.5f);
                _rb.isKinematic = false;
            }
        }
    }

    private void Win()
    {
        source.pitch = 1;
        source.PlayOneShot(winSound);
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }
    private Vector3 VectorFix(float hIn, float vIn)
    {
        Vector3 VInput = new Vector3(vIn, 0, vIn);
        Vector3 HInput = new Vector3(hIn, 0, -hIn);

        Vector3 Direction = VInput + HInput;

        Direction.x = Mathf.Clamp(Direction.x, -1, 1);
        Direction.z = Mathf.Clamp(Direction.z, -1, 1);
        return Direction;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wood")
        {
            if (woodAmount < 10)
            {
                source.PlayOneShot(audioPop);
                Destroy(collision.gameObject);
                Bug.localScale += new Vector3(3, 3, 3);
                woodAmount++;
            }
        }

        if (collision.gameObject.tag == "Finish")
        {
            Win();
        }
    }
    void OnDrawGizmos()
    {
        if (attackZone == null)
            return;
        Gizmos.DrawWireSphere(attackZone.position, attackRange);
    }


    private void OnEnable()
    {
        scoreLoc.Arguments = new object[] { killCounter };
        scoreLoc.StringChanged += UpdateText;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        YandexGame.FullscreenShow();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        EventBus.OnExitActivated += CheckExit;
        EventBus.OnBearDied += BearDie;
        EventBus.OnPlayerDied += Die;
        EventBus.OnPlayerRespawn += Respawn;
        currentHealth = maxHealth;
        woodAmount = 0;
        berryAmount = 0;
        Stamina = 1f;
        altarCounter = 0.125f * 0f;

        if (platform == Platform.PC)
        {
            joystickMovement.gameObject.SetActive(false);
            joystickAttack.gameObject.SetActive(false);
            attackButton.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(false);
            pickUpButton.gameObject.SetActive(false);
            HealButton.gameObject.SetActive(false);            
        }
    }
  
    private void OnDestroy()
    {
        EventBus.OnExitActivated -= CheckExit;
        EventBus.OnBearDied -= BearDie;
        EventBus.OnPlayerDied -= Die;
        EventBus.OnPlayerRespawn -= Respawn;
        scoreLoc.StringChanged -= UpdateText;
    }
    void Update()
    {

        GUI_Update();

        //// #Console command
        //if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.N))
        //{
        //    CheckExit();
        //}

        void Rotate()
        {
            Vector3 direction = new Vector3(target.position.x - transform.position.x, 0f, target.position.z - transform.position.z);
            Quaternion _rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, rotSpeed * Time.deltaTime);
        }


        if (platform == Platform.PC)
        {
            vInput = Input.GetAxis("Vertical");
            hInput = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseButton();
            }
        }

        if (platform == Platform.Phone)
        {
            vInput = joystickMovement.Vertical;
            hInput = joystickMovement.Horizontal;
        }

        Speed = moveSpeed;
        animator.SetFloat("isMove", isMove);
        Stamina = Mathf.Clamp(Stamina, 0, 1);
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("2"))
        {
            Speed *= 0.6f;
        }

        if (isAlive)
        {
            if (Move != Vector3.zero)
            {
                isMove = 1f;

                if (platform == Platform.PC)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Hit();
                    }

                    if (Input.GetMouseButton(1))
                    {
                        Speed *= 0.6f;
                        Rotate();
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.LookRotation(Move);
                        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
                    }
                }

                if (platform == Platform.Phone)
                {
                    if (joystickAttack.Vertical != 0 || joystickAttack.Horizontal != 0)
                    {
                        Speed *= 0.6f;
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(VectorFix(joystickAttack.Horizontal, joystickAttack.Vertical)), rotSpeed * Time.deltaTime);

                        Hit();
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.LookRotation(Move);
                        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
                    }
                }               
            }
            else
            {
                isMove = 0f;
                if (platform == Platform.PC)
                {
                    if (Input.GetMouseButton(1))
                    {
                        Rotate();
                    }
                }

                if (platform == Platform.PC)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Hit();
                    }
                }

                if (platform == Platform.Phone) 
                {
                    if (joystickAttack.Vertical != 0 || joystickAttack.Horizontal != 0)
                    {
                        Speed *= 0.6f;
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(VectorFix(joystickAttack.Horizontal, joystickAttack.Vertical)), rotSpeed * Time.deltaTime);

                        Hit();
                    }
                }

                if (platform == Platform.PC)
                {
                    //selectBerries
                    {

                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            SelectionOfBerries();
                        }
                    }
                    //light up
                    {
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            LightAltar();
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        Heal();
                    }
                }
                
            }
        }      
    }

    public void Run(bool isRun)
    {

        if (isRun)
        {
            Stamina -= sprintCost;
            if (Stamina > 0.05)
            {
                Speed *= 1.6f;
                animator.speed = 1.6f;
            }
            else
            {
                animator.speed = 1f;
            }
        }
        else
        {
            Stamina += sprintCost * 0.2f;
            animator.speed = 1f;
        }
    }
    void FixedUpdate()
    {
        if (isAlive)
        //sprint
        {
            if (Move != Vector3.zero)
            {
                Run(Input.GetKey(KeyCode.LeftShift));
            }
            else
            {
                Stamina += sprintCost * 0.55f;
            }
        }

        //movement
        {
            
            if ((vInput == 1f || vInput == -1f) || (hInput == 1f || hInput == -1f))
            {
                Move = Vector3.Normalize(VectorFix(hInput, vInput));
            }
            else
            {
                Move = VectorFix(hInput, vInput);
            }

            if(isAlive)
            {
                _rb.MovePosition(transform.position + Move * Time.fixedDeltaTime * Speed);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(90, 0, 0), 5f * Time.deltaTime);
            }
        }
    }
}
