using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int nowDay = 1;
    public int totalDay = 7;
    public float DayTime = 300f;

    private float timer = 0f;
    public bool isDay = true;

    public TextMeshProUGUI timeText;
    private float currentDayTime;
    private float currentNightTime;

    public static GameManager Instance;
    public GameObject endingPanel;

    public Image[] gameEndingImages;

    private int healthDrainDay = -1;
    private bool isHealthDraining = false;

    private Coroutine healthDrainCoroutine;

    private TextDisplay textDisplay;

    public WeaponPickup[] allWeapons;

    private Vector3 savedPlayerPosition;
    private Quaternion savedPlayerRotation;
    private bool isFirstSpawn = true;

    private bool isGameOver = false;


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (endingPanel == null)
        {
            GameObject obj = GameObject.Find("EndingPanel");
            if (obj != null)
            {
                endingPanel = obj;
                Debug.Log("✅ Night 씬에서 EndingPanel 다시 연결됨");
            }
        }
    }

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

    void ShowGameOver()
    {
        if (endingPanel != null)
            endingPanel.SetActive(true);

        foreach (Image img in gameEndingImages)
        {
            if (img != null)
                img.enabled = true;
        }

        Time.timeScale = 0f;
    }

    void Start()
    {
        healthDrainDay = Random.Range(1, totalDay + 1);
        Debug.Log($"체력 감소{healthDrainDay}");

        if (endingPanel != null)
            endingPanel.SetActive(false);

        foreach (Image img in gameEndingImages)
        {
            if (img != null)
                img.enabled = false;
        }

        LoadDayScene();
    }

    void Update()
    {
        timer += Time.deltaTime;

        currentNightTime = 120f + ((nowDay - 1) * 10f);
        currentDayTime = DayTime - currentNightTime;
                
        UpdateTimeText();

        if (isDay && timer >= currentDayTime)
        {
            SavePlayerTransform();

            timer = 0f;
            isDay = false;

            LoadNightScene();

            if (nowDay == healthDrainDay)
            {
                isHealthDraining = true;
                healthDrainCoroutine = StartCoroutine(DrainPlayerHealth());
            }
        }
        else if (!isDay && timer >= currentNightTime)
        {
            SavePlayerTransform();

            if (nowDay >= totalDay)
            {
                if (endingPanel != null)
                {
                    Debug.Log("✅ endingPanel 직접 활성화");
                    endingPanel.SetActive(true);
                }
                else
                {
                    Debug.LogError("❌ endingPanel이 null이야! Inspector에서 연결 필요");
                }

                foreach (Image img in gameEndingImages)
                {
                    if (img != null)
                        img.enabled = true;
                }

                StartCoroutine(DelayStopTime());

                isGameOver = true;
                return;
            
            }
            else
            {
                timer = 0f;
                isDay = true;
                nowDay++;
                LoadDayScene();

                RefreshWeaponsByDay();

                if (isHealthDraining)
                {
                    isHealthDraining = false;
                    if (healthDrainCoroutine != null)
                        StopCoroutine(healthDrainCoroutine);
                }
            }
        }
    }

    IEnumerator DelayStopTime()
    {
        yield return null;
        Time.timeScale = 0f;
    }

    void UpdateTimeText()
    {
        if (timeText == null)
        {
            GameObject obj = GameObject.Find("TimeText");
            if (obj != null)
            {
                timeText = obj.GetComponent<TextMeshProUGUI>();
            }
        }

        if (timeText == null) return;

        float remaining = isDay ? (currentDayTime - timer) : (currentNightTime - timer);
        remaining = Mathf.Max(remaining, 0f);

        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    void LoadDayScene()
    {
        SceneManager.LoadScene("Day");
        StartCoroutine(WaitAndSetupNextDay());
    }

    void LoadNightScene()
    {
        SceneManager.LoadScene("Night");
        StartCoroutine(WaitAndSetupTimeText());
        StartCoroutine(ShowNoticeAfterSceneReady());
    }

    IEnumerator WaitAndSetupNextDay()
    {
        yield return new WaitForSeconds(0.1f);

        TryUpdateDayTextUI();
        UpdateAllZombieStats();
        TryFindTimeText();
        TryFindNoticeText();
        RestorePlayerTransform();
    }

    IEnumerator WaitAndSetupTimeText()
    {
        yield return new WaitForSeconds(0.1f);
        TryFindTimeText();
        TryFindNoticeText();
        RestorePlayerTransform();
    }

    void TryFindTimeText()
    {
        GameObject obj = GameObject.Find("TimeText");
        if (obj != null)
        {
            timeText = obj.GetComponent<TextMeshProUGUI>();
        }
    }

    void TryFindNoticeText()
    {
        textDisplay = FindObjectOfType<TextDisplay>();

        if (textDisplay != null)
        {
            Debug.Log($"[TextDisplay] 연결됨: {textDisplay.gameObject.name}");
        }
        else
        {
            Debug.LogError("[TextDisplay] 찾을 수 없음! TextManager 오브젝트 확인 필요");
        }
    }

    void TryUpdateDayTextUI()
    {
        DayDisplay display = FindObjectOfType<DayDisplay>();
        if (display != null)
        {
            display.UpdateDayText(nowDay);
        }
    }

    void UpdateAllZombieStats()
    {
        EnemyStamina[] allEnemies = FindObjectsOfType<EnemyStamina>();
        foreach (EnemyStamina enemy in allEnemies)
        {
            enemy.InitializeStats(nowDay);
        }
    }

    IEnumerator ShowNotice(string message, float duration)
    {
        if (textDisplay != null)
        {
            textDisplay.Show(message, duration);
            Debug.Log("[Notice] TextDisplay로 메시지 전달됨");
        }
        else
        {
            Debug.LogWarning("[Notice] TextDisplay가 null이라 메시지 전달 실패");
        }

        yield return null;
    }

    IEnumerator DrainPlayerHealth()
    {
        yield return new WaitForSeconds(10f);
        while (isHealthDraining)
        {
            PlayerStamina.Instance?.TakeDamage(5);
            yield return new WaitForSeconds(10f);
        }
    }

    void RefreshWeaponsByDay()
    {
        foreach (WeaponPickup weapon in allWeapons)
        {
            weapon.Refresh();
        }
    }

    void SavePlayerTransform()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            savedPlayerPosition = player.transform.position;
            savedPlayerRotation = player.transform.rotation;
            Debug.Log($"[Save] 플레이어 위치 저장: {savedPlayerPosition}");
        }
    }

    void RestorePlayerTransform()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            if (isFirstSpawn)
            {
                player.transform.position = new Vector3(27.789f, 0.731f, 75.916f);
                player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                Debug.Log("[Init] 처음 위치로 리스폰");
                isFirstSpawn = false;
            }
            else
            {
                player.transform.position = savedPlayerPosition;
                player.transform.rotation = savedPlayerRotation;

                Debug.Log($"[Load] 저장된 위치로 복원: {savedPlayerPosition}");
            }
        }
        else
        {
            Debug.LogWarning("[Load] 플레이어 오브젝트를 찾을 수 없음!");
        }
    }

    IEnumerator ShowNoticeAfterSceneReady()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Night");
        yield return new WaitUntil(() => isDay == false);

        textDisplay = FindObjectOfType<TextDisplay>();
        Debug.Log($"[디버그] 현재 날짜: {nowDay}, 체력 깎이는 날: {healthDrainDay}");
        if (textDisplay != null)
        {
            Debug.Log("✅ TextDisplay 연결됨: " + textDisplay.name);
            if (nowDay == healthDrainDay && isDay == false)
            {
                textDisplay.Show("오늘 밤이 끝날 때까지 체력이 10초마다 5씩 깎입니다", 5f);
            }
        }
        else
        {
            Debug.LogWarning("❌ TextDisplay를 찾지 못했습니다");
        }
    }

    public void ResetGame()
    {
        nowDay = 1;
        DayTime = 600f;
        timer = 0f;
        isDay = true;
        currentDayTime = 0f;
        currentNightTime = 0f;

        if (PlayerStamina.Instance != null)
        {
            PlayerStamina.Instance.ResetStamina();
        }
        
    }

   
}
