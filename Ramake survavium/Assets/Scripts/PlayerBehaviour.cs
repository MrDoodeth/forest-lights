using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Audio;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Platform platform;

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
    [SerializeField] private FixedJoystick joystick;

    [Header("Hit")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private Transform attackZone;
    [SerializeField] private ParticleSystem fxChop;

    [Header("GUI")]
    [SerializeField] private float sprintCost = -0.002f;
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
    private enum Platform { PC, Phone };
    private Vector3 Move = new Vector3();

    void OnDrawGizmos()
    {
        if (attackZone == null)
            return;
        Gizmos.DrawWireSphere(attackZone.position, attackRange);
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

    private void OnDestroy()
    {
        EventBus.OnExitActivated -= CheckExit;
        EventBus.OnBearDied -= BearDie;
        EventBus.OnPlayerDied -= Die;
    }
    public void TakeDamage(float damage, Transform trans)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            source.PlayOneShot(loseSound);
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
        inPause.TransitionTo(0f);
        _rb.isKinematic = true;
        animator.SetBool("isDie", true);
        transform.position += Vector3.up * 0.16f;
        isAlive = false;
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
        killAmountCounter.text = killCounter.ToString();
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
    private void Hit()
    {
        if (Stamina > 0.125)
        {
            animator.SetTrigger("isHit");
            Stamina -= 0.125f;
        }
    }
    private void SelectionOfBerries()
    {
        if (berryAmount < 20)
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
        }          
    }

    private void Heal()
    {
        if (currentHealth < 1 && berryAmount > 0)
        {
            source.PlayOneShot(audioEat[Random.Range(0, audioEat.Length)]);
            berryAmount -= 1;
            currentHealth += 0.3f;
        }
    }
    private void LightAltar()
    {
        if (woodAmount > 9)
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
                    altarCounter += 0.0625f;
                    _rb.isKinematic = false;
                }
            }
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

    private void Win()
    {
        source.pitch = 1;
        source.PlayOneShot(winSound);
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        EventBus.OnExitActivated += CheckExit;
        EventBus.OnBearDied += BearDie;
        EventBus.OnPlayerDied += Die;
        currentHealth = maxHealth;
        woodAmount = 0;
        berryAmount = 0;
        Stamina = 1f;
        altarCounter = 0.0625f * 0f;
    }


    void Update()
    {
        void Rotate()
        {
            Vector3 direction = new Vector3(target.position.x - transform.position.x, 0f, target.position.z - transform.position.z);
            Quaternion _rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, rotSpeed * Time.deltaTime);
        }

        GUI_Update();

        if (platform == Platform.PC)
        {
            joystick.gameObject.SetActive(false);
            vInput = Input.GetAxis("Vertical");
            hInput = Input.GetAxis("Horizontal");
        }

        if (platform == Platform.Phone)
        {
            joystick.gameObject.SetActive(true);
            vInput = joystick.Vertical;
            hInput = joystick.Horizontal;
        }

        Speed = moveSpeed;
        animator.SetFloat("isMove", isMove);
        Stamina = Mathf.Clamp(Stamina, 0, 1);
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("2"))
        {
            Speed *= 0.6f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }
        if (isAlive)
        {
            if (Move != Vector3.zero)
            {
                isMove = 1f;

                if (Time.time >= nextHitTime)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Hit();
                        nextHitTime = Time.time + 1 / hitRate;
                    }
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
            else
            {
                isMove = 0f;

                if (Input.GetMouseButton(1))
                {
                    Rotate();
                }

                if (Time.time >= nextHitTime)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Hit();
                        nextHitTime = Time.time + 1 / hitRate;
                    }
                }

                if (Time.time >= nextSelectionTime)
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        Heal();
                        nextSelectionTime = Time.time + 1 / selectionRate;
                    }
                }

                //selectBerries
                {
                    if (Time.time >= nextSelectionTime)
                    {
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            SelectionOfBerries();
                            nextSelectionTime = Time.time + 1 / selectionRate;
                        }
                    }
                }
                //light up
                {
                    if (Time.time >= nextHitTime)
                    {
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            LightAltar();
                            nextHitTime = Time.time + 1 / hitRate;
                        }
                    }
                }
            }
        }      
    }

    void FixedUpdate()
    {
        if (isAlive)
        //sprint
        {           
            if (Move != Vector3.zero)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Stamina += sprintCost;
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
                    Stamina -= sprintCost * 0.1f;
                    animator.speed = 1f;
                }
            }
            else
            {
                Stamina -= sprintCost * 0.2f;
            }
        }

        //movement
        {
            Vector3 VInput = new Vector3(vInput, 0, vInput);
            Vector3 HInput = new Vector3(hInput, 0, -hInput);

            Vector3 Direction = VInput + HInput;

            Direction.x = Mathf.Clamp(Direction.x, -1, 1);
            Direction.z = Mathf.Clamp(Direction.z, -1, 1);
            if ((vInput == 1f || vInput == -1f) || (hInput == 1f || hInput == -1f))
            {
                Move = Vector3.Normalize(Direction);
            }
            else
            {
                Move = Direction;
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
