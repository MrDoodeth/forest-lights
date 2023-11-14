using UnityEngine;

public class Stump : MonoBehaviour
{
    [SerializeField] private GameObject Wood;
    [SerializeField] private GameObject Grass;
    [SerializeField] private int maxHealth = 200;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

        void Die()
        {
            Instantiate(Wood, transform.position + Vector3.up / 2, Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 0f)));
            Instantiate(Grass, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
