using System.Runtime.CompilerServices;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform healthBar;
    public Transform mask;

    private float maxHealth=100f;
    private float currentHealth;
    private Vector3 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        targetPosition = healthBar.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.localPosition = Vector3.Lerp(healthBar.localPosition, targetPosition, 5f * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("space");
            TakeDamage(10f); // Press Space to take damage
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            Debug.Log("h key");
            Heal(10f);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health never goes below 0

        UpdateHealthBar();
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health never exceeds max health

        // Recalculate target position based on current health
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float healthPercent = currentHealth / maxHealth;
        targetPosition = new Vector3(mask.localPosition.x - (1 - healthPercent) * mask.GetComponent<SpriteRenderer>().bounds.size.x, healthBar.localPosition.y, healthBar.localPosition.z);
    }
}

