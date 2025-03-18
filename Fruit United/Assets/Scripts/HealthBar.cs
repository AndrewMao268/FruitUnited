using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] float maxHealth=100;
    float currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(float damage)
    {
        if (currentHealth - damage >0)
        {
        currentHealth = currentHealth-damage;
        }
        else
        {
            currentHealth =0;
        }

    }

    void UpdateHealthBar()
    {

    }
}
