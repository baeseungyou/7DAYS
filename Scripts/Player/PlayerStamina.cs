using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class PlayerStamina : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;
    public static PlayerStamina Instance;
    public GameObject endingPanel;
    public Text gameEndingText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        int before = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"[PlayerStamina] 체력 {before} → {currentHealth} (+{amount})");
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        int before = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);

            if (gameEndingText != null)
            {
                gameEndingText.text = "Game Over";
                gameEndingText.color = Color.red;
            }

            Time.timeScale = 0f;
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (healthBar == null)
        {
            GameObject hb = GameObject.FindWithTag("HealthBar");
            if (hb != null)
                healthBar = hb.GetComponent<Slider>();
        }

        if (endingPanel == null)
        {
            GameObject ep = GameObject.Find("EndingPanel");
            if (ep != null)
            {
                endingPanel = ep;
                ep.SetActive(false);
            }
        }

        if (gameEndingText == null)
        {
            GameObject gt = GameObject.Find("GameEndingText");
            if (gt != null)
                gameEndingText = gt.GetComponent<Text>();
        }

        if (healthBar != null)
        {
            string name = scene.name;
            bool show = (name == "Day" || name == "Night");
            healthBar.gameObject.SetActive(show);
            UpdateHealthUI();
        }
    }

    public void ResetStamina()
    {
        currentHealth = maxHealth;
    }


    void Update()
    {
        // 필요 시 체력 감소 테스트 등을 여기에 넣을 수 있음
    }
}