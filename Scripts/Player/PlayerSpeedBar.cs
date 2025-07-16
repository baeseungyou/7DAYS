using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSpeedBar : MonoBehaviour
{
    [Header("스태미너 바")]
    public Slider staminaBar;
    public float maxStamina = 5f;
    public float currentStamina;

    public static PlayerSpeedBar Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0f)
            currentStamina = 0f;

        UpdateStaminaUI();
    }

    public void RecoverStamina(float amount)
    {
        float before = currentStamina;
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        
        UpdateStaminaUI();
    }

    void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina / maxStamina;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (staminaBar == null)
        {
            GameObject sb = GameObject.FindWithTag("SpeedBar");
            if (sb != null)
                staminaBar = sb.GetComponent<Slider>();
        }

        if (staminaBar != null)
        {
            string name = scene.name;
            bool show = (name == "Day" || name == "Night");
            staminaBar.gameObject.SetActive(show);
            UpdateStaminaUI();
        }
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
        UpdateStaminaUI();
    }
}
