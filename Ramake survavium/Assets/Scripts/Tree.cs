using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Tree : MonoBehaviour
{
    [SerializeField] private AudioSource audioDeath;
    [SerializeField] private GameObject Wood;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem fxHit;
    [SerializeField] private GameObject fxDeath;
    [SerializeField] private Transform cellForFxDeath;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float Force;
    [SerializeField] private GameObject down;
    private int currentHealth;
    public Transform[] woodCell;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        animator.enabled = false;
        rb.mass = 1000;
        rb.isKinematic = true;
    }


    public void TakeDamage(int damage, Transform trans)
    {
        animator.enabled = true;
        fxHit.Play();
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("isHit");
        }


        void Die()
        {
            down.layer = 9;
            rb.mass = 20;
            rb.isKinematic = false;
            animator.enabled = false;
            rb.AddForce((transform.position - trans.position) * Force, ForceMode.Impulse);

            StartCoroutine(Death());

            gameObject.GetComponent<Tree>().enabled = false;

            IEnumerator Death()
            {
                yield return new WaitForSeconds(2.5f);
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(Wood, woodCell[i].position, Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 0f)));
                }
                GameObject Death = Instantiate(fxDeath, cellForFxDeath.position, Quaternion.identity);
                audioDeath.Play();
                Destroy(gameObject);
            }
        }
    }
}
