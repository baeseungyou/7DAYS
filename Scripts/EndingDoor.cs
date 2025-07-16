using UnityEngine;
using TMPro;
using System.Collections;

public class EndingDoor : MonoBehaviour
{
    public string PlayerHeadTag = "MainCamera";
    private Transform player;

    public Animation doorAnimation;
    public bool isUnlocked = false;

    private bool isOpened = false;
    private bool inZone = false;

    public GameObject endingPanel;
     public GameObject keyEndingPanel;

    [System.Serializable]
    public class DoorTexts
    {
        public bool enabled = true;
        public string openingText = "Press [E] to open";
        public string lockText = "You need a key!";
        public GameObject TextPrefab;
    }
    public DoorTexts doorTexts = new DoorTexts();

    private Canvas textObj;
    private TextMeshProUGUI theText;

    void Start()
    {
        player = GameObject.FindWithTag(PlayerHeadTag)?.transform;

        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (keyEndingPanel != null)
            keyEndingPanel.SetActive(false); // 키 엔딩 패널 비활성화

        if (doorTexts.enabled && doorTexts.TextPrefab != null)
        {
            GameObject go = Instantiate(doorTexts.TextPrefab, Vector3.zero, Quaternion.identity);
            textObj = go.GetComponent<Canvas>();
            theText = textObj.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (!inZone)
        {
            HideHint();
            return;
        }

        if (GameManager.Instance.nowDay == 7 && !GameManager.Instance.isDay && isUnlocked)
        {
            ShowHint(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                InventoryUI inventory = FindObjectOfType<InventoryUI>();
                inventory.ConsumeKeyItem();

                OpenDoor();
                isUnlocked = false;
            }
        } 

        if (isUnlocked) // 언제든지 열 수 있도록 수정
        {
            ShowHint(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                InventoryUI inventory = FindObjectOfType<InventoryUI>();
                inventory.ConsumeKeyItem();

                OpenDoor();
                isUnlocked = false;
            }
        }

        else
        {
            ShowHint(false);
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
    }

    void OpenDoor()
    {
        if (!isOpened)
        {
            isOpened = true;
            HideHint();

            //ShowSuccessEnding();

            if (isUnlocked)  // isUnlocked가 true이면 키를 사용한 엔딩
            {
                ShowKeyEnding();
            }
            else
            {
                ShowDefaultEnding();
            }////////////////////////////////////////////////////
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerHeadTag))
        {
            inZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerHeadTag))
        {
            inZone = false;
            HideHint();
        }
    }

    void ShowHint(bool canOpen)
    {
        if (!doorTexts.enabled || doorTexts.TextPrefab == null) return;

        string msg = canOpen ? doorTexts.openingText : doorTexts.lockText;
        msg = msg.Replace("[BUTTON]", "'E'");

        textObj.enabled = false;
        theText.text = msg;
        textObj.enabled = true;
    }

    void HideHint()
    {
        if (doorTexts.enabled && textObj != null)
        {
            textObj.enabled = false;
        }
    }

/*
    public void ShowSuccessEnding()
    {
        if (endingPanel != null)
            endingPanel.SetActive(true);

        Time.timeScale = 0f;
    }

*/
///////////////////////이거 삭제하면됌
void ShowDefaultEnding()
    {
        if (endingPanel != null)
            endingPanel.SetActive(true);  // 기본 엔딩 패널 활성화

        Time.timeScale = 0f;
    }

    void ShowKeyEnding()
    {
        Debug.Log("ShowKeyEnding() 호출됨!");
        if (keyEndingPanel != null)
        {
            Debug.Log("KeyEndingPanel 활성화 중!");
            keyEndingPanel.SetActive(true);
        }
        else
        {
            Debug.Log("KeyEndingPanel 연결 안 됨!");
        }
        Time.timeScale = 0f;
    }

////////////////여기까지 삭제
}
