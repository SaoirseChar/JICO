using System;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    public static Health instance;
    
    public Image healthBar;

    public float health, maxHealth = 100;
    private float lerpSpeed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        health = maxHealth;
        healthBar = GetComponent<Image>();
    }

    private void Update()
    {
        if (health > maxHealth) health = maxHealth;

        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Damage(10f);
        }
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (health / maxHealth), lerpSpeed);
    }
    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        healthBar.color = healthColor;
    }

    public void Damage(float damagePoints)
    {
        if (health > 0)
            health -= damagePoints;
    }
    public void Heal(float healingPoints)
    {
        if (health < maxHealth)
            health += healingPoints;
    }
}
