using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform healthBar;
    public Transform mask;

    public float maxHealth = 100;
    private float currentHealth;
    private Vector3 targetPosition;

    private Vector3 originalPosition;

    void Start()
    {
        currentHealth = maxHealth;
        originalPosition = mask.localPosition;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Smoothly move the health bar's position towards the target position
        mask.localPosition = Vector3.Lerp(mask.localPosition, targetPosition, 5f * Time.deltaTime);

        // Detect Space bar press to take damage
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed - Taking damage");
            TakeDamage(10f);  // Press Space to take damage
        }

        // Detect H key press to heal
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H key pressed - Healing");
            Heal(10f);  // Press H to heal
        }
    }

    public void TakeDamage(float amount)
    {
        // Ensure we are taking damage only when health is > 0
        if (currentHealth > 0)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health never goes below 0
            Debug.Log("Current Health after damage: " + currentHealth);
        }

        UpdateHealthBar();
    }

    public void Heal(float amount)
    {
        // Ensure health never exceeds maxHealth
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health never exceeds max health
            Debug.Log("Current Health after healing: " + currentHealth);
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // Calculate the percentage of health remaining
        float healthPercent = currentHealth / maxHealth;

        // Calculate target position of the health bar based on health percentage
        targetPosition = new Vector3(originalPosition.x - ((1.0f - healthPercent) * healthBar.GetComponent<SpriteRenderer>().bounds.size.x) / 2.0f, healthBar.localPosition.y, healthBar.localPosition.z);
        Debug.Log("Health Percent: " + healthPercent);
    }
}