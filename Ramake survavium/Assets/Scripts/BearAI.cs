using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BearAI : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private float maxHealth = 100;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] audioRumble_Idle;
    [SerializeField] private AudioClip[] audioRumble_Damage;
    private float timer = 0f;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private Rigidbody rb;
    //patrolling
    private Vector3 walkPoint;
    private bool walkPointSet = false;
    private bool isAttack = false;
    private List<Transform> points;
    
    //attacking
    private bool alreadyAttacked;

    //states
    private bool playerInSightRange, playerInAttackRange;
    private bool isAlive = true;
    private float currentHealth;

    private void Start()
    {
        EventBus.OnPlayerDied += BearDeactivate;
        EventBus.OnPlayerRespawn += BearActivate;
        EventBus.OnEnd += CutScene;
        player = GameObject.Find("Man").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    private void OnDestroy()
    {
        EventBus.OnPlayerDied -= BearDeactivate;
        EventBus.OnPlayerRespawn -= BearActivate;
        EventBus.OnEnd -= CutScene;
    }
    private void BearDeactivate()
    {
        isAlive = false;
    }

    private void BearActivate()
    {
        isAlive = true;
    }

    private void CutScene()
    {
        StartCoroutine(CutScene());
        IEnumerator CutScene()
        {
            isAlive = false;
            yield return new WaitForSeconds(10.5f);
            isAlive = true;
            isAttack = true;
            source.volume = 0.5f;
            EventBus.OnCutSceneEnd?.Invoke();
        }
    }
    public void TakeDamage(float damage, Transform trans)
    {
        source.PlayOneShot(audioRumble_Damage[Random.Range(0, audioRumble_Damage.Length)]);
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            rb.AddForce((transform.position - trans.position) * 5, ForceMode.Impulse);
        }


        void Die()
        {
            gameObject.layer = 0;
            isAlive = false;
            EventBus.OnBearDied?.Invoke();            
            rb.isKinematic = true;
            agent.enabled = false;
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isChasing", false);

            animator.SetBool("isDie", true);
            Destroy(gameObject, 3f);
        }
    }

    private void Attack()
    {
        Collider[] playerHit = Physics.OverlapSphere(transform.position + Vector3.up / 2, attackRange, whatIsPlayer);        

        foreach (Collider hit in playerHit)
        {
            hit.GetComponent<PlayerBehaviour>().TakeDamage(Random.Range(0.1f, 0.2f), transform);
        }
    }
    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        void SearchWalkPoint()
        {
            float randomZ = Random.Range(-20, 20);
            float randomX = Random.Range(-20, 20);
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        }

        if (walkPointSet)
        {
            animator.SetBool("isPatrolling", true);
            animator.SetBool("isChasing", false);
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void ChasePlayer()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            source.PlayOneShot(audioRumble_Idle[Random.Range(0, audioRumble_Idle.Length)]);
            timer = 0f;
        }
        agent?.SetDestination(player.position);
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", true);
    }

    private void AttackPlayer()
    {
        animator.SetBool("isPatrolling", false);
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //attack
            animator.SetTrigger("isAttacking");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position + Vector3.up / 2, attackRange, whatIsPlayer);
        if (isAlive)
        {
            if (isAttack)
            {
                ChasePlayer();
                if (playerInSightRange && playerInAttackRange) AttackPlayer();
                return;
            }
            else
            {
                if (!playerInSightRange && !playerInAttackRange) Patrolling();
                if (playerInSightRange && !playerInAttackRange) ChasePlayer();
                if (playerInSightRange && playerInAttackRange) AttackPlayer();
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up / 2, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
