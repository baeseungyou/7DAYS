using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyStamina : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int attackPower;

    private Animator animator;
    private bool isDead = false;
    [Header("Health UI")]
    public Slider healthSlider;


    void Start()
    {
        InitializeStats(GameManager.Instance != null ? GameManager.Instance.nowDay : 1);
        animator = GetComponent<Animator>();

        UpdateHealthUI();
    }

    public void InitializeStats(int day)
    {
        maxHealth = 90 + (day * 10);
        attackPower = 8 + (day * 2);
        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        ZombieAI zombieAI = GetComponent<ZombieAI>();
        if (zombieAI != null)
            {
                zombieAI.PlaySound(zombieAI.dieSound);
            }

        Destroy(gameObject, 3.333f);
    }
}
