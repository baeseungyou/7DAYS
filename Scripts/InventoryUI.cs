using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public Image[] itemIcons;               // 아이콘 이미지들
    public TextMeshProUGUI[] itemCounts;               // 아이템 수량 텍스트들
    public SlotData[] slots = new SlotData[10];  // 슬롯 데이터들

    private PlayerStamina playerStamina;

    public WeaponManager weaponManager;
    private ItemData equippedWeaponItem;

    public TextMeshProUGUI keyPickupText;//////////////
    public Image pickupImageUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        playerStamina = FindObjectOfType<PlayerStamina>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new SlotData(); // null 방지 초기화
        }

        weaponManager = FindObjectOfType<WeaponManager>();

        if (keyPickupText != null)
        {
            keyPickupText.gameObject.SetActive(false); // 시작할 때 비활성화
        }
        if (pickupImageUI != null)
        {
            pickupImageUI.gameObject.SetActive(false); // 시작할 때 비활성화
        }
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(i == 9 ? KeyCode.Alpha0 : KeyCode.Alpha1 + i))
            {
                UseItem(i);
            }
        }
    }

    public void AddItem(ItemData item)
    {
        // 이미 존재하는 아이템이면 개수만 추가
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].count++;
                UpdateSlotUI(i);

                if ((item.itemType == ItemType.Key || item.itemType == ItemType.Weapon) && keyPickupText != null)
                {
                    StartCoroutine(ShowKeyPickupText(item.pickupMessage));
                }
                if (item.pickupImage != null && pickupImageUI != null)
                {
                    pickupImageUI.sprite = item.pickupImage;
                    pickupImageUI.gameObject.SetActive(true);
                    StartCoroutine(HidePickupImageAfterDelay());
                }

                return;
            }
        }

        // 비어있는 슬롯 찾기
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                slots[i].count = 1;
                itemIcons[i].sprite = item.icon;
                itemIcons[i].gameObject.SetActive(true);
                UpdateSlotUI(i);

                // 🔑 키라면 텍스트 띄우기
                if ((item.itemType == ItemType.Key || item.itemType == ItemType.Weapon) && keyPickupText != null)
                {
                    StartCoroutine(ShowKeyPickupText(item.pickupMessage));
                }
                if (item.pickupImage != null && pickupImageUI != null && item.isKey)
                {
                    pickupImageUI.sprite = item.pickupImage;
                    pickupImageUI.gameObject.SetActive(true);
                    StartCoroutine(HidePickupImageAfterDelay());
                }

                return;
            }
        }

        Debug.Log("인벤토리 가득 참");
    }

    IEnumerator ShowKeyPickupText(string message)
    {
        keyPickupText.text = message;
        keyPickupText.gameObject.SetActive(true); // 텍스트 보이기
        yield return new WaitForSeconds(3f); // 2초 동안 유지
        keyPickupText.gameObject.SetActive(false); // 다시 숨기기
    }
    IEnumerator HidePickupImageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        pickupImageUI.gameObject.SetActive(false);
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if (slots[index].item == null || slots[index].count <= 0) return;

        var item = slots[index].item;

        if (item.itemType == ItemType.Key)
        {
            EndingDoor[] endingDoors = FindObjectsOfType<EndingDoor>();
            foreach (EndingDoor endingDoor in endingDoors)
            {
                endingDoor.Unlock();
            }

            DoorScript[] doors = FindObjectsOfType<DoorScript>();
            foreach (DoorScript door in doors)
            {
                door.Unlock();
            }
            Debug.Log("[Inventory] 키 사용 → 문 열기 가능 상태로 변경!");
            

            // 키 아이템 소모
            slots[index].count--;
            if (slots[index].count <= 0)
            {
                slots[index].item = null;
                itemIcons[index].gameObject.SetActive(false);
                itemIcons[index].sprite = null;
                itemCounts[index].text = "";
            }
            else
            {
                UpdateSlotUI(index);
            }

            return; // 이후 코드 실행 X
        }

        if (item.itemType == ItemType.Food)
        {
            PlayerStamina.Instance.Heal(item.healAmount);

            if (item.eatSound != null)
            {
                AudioSource.PlayClipAtPoint(item.eatSound, Camera.main.transform.position);
            }
        }
        else if (item.itemType == ItemType.Weapon && item.weaponPrefab != null)
        {
            Debug.Log($"[UseItem] 무기 사용 시도: {item.name}");
            if (weaponManager == null)
            {
                weaponManager = FindObjectOfType<WeaponManager>();
                if (weaponManager == null)
                {
                    Debug.LogWarning("WeaponManager를 찾을 수 없습니다. 무기를 장착할 수 없습니다.");
                    return;
                }
                else
                {
                    Debug.Log($"[UseItem] {item.name} 장착 조건 만족 → EquipWeapon 실행 예정");
                }
            }

            var oldWeaponObj = weaponManager.GetEquippedWeapon();

            // 교체 시 기존 무기를 되돌려 받음
            ItemData oldWeapon = weaponManager.EquipWeapon(item.weaponPrefab, item);
            equippedWeaponItem = item;

            var newWeaponObj = weaponManager.GetEquippedWeapon();
            var newSwinger = newWeaponObj?.GetComponent<IWeaponSwing>();

            if (newSwinger != null)
            {
                newSwinger.hasWeapon = true;
            }

            if (oldWeaponObj != null)
            {
                var oldSwinger = oldWeaponObj.GetComponent<IWeaponSwing>();
                if (oldSwinger != null)
                {
                    oldSwinger.hasWeapon = false;
                }
            }

            // 기존 무기 다시 해당 슬롯에 덮어쓰기
            if (oldWeapon != null)
            {
                slots[index].item = oldWeapon;
                slots[index].count = 1; // 무기는 1개만
                itemIcons[index].sprite = oldWeapon.icon;
                itemIcons[index].gameObject.SetActive(true);
                UpdateSlotUI(index);

                return; // 교체했으므로 이후 코드 실행하지 않음
            }
        }

        slots[index].count--;

        if (slots[index].count <= 0)
        {
            slots[index].item = null;
            itemIcons[index].gameObject.SetActive(false);
            itemIcons[index].sprite = null;
            itemCounts[index].text = "";
        }
        else
        {
            UpdateSlotUI(index);
        }
    }

    private void UpdateSlotUI(int index)
    {
        if (slots[index].count > 1)
        {
            itemCounts[index].text = slots[index].count.ToString();
            itemCounts[index].gameObject.SetActive(true);
        }
        else
        {
            itemCounts[index].text = "";
            itemCounts[index].gameObject.SetActive(false); // 한 개일 땐 비활성화
        }
    }

    public void ResetInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].item = null;
            slots[i].count = 0;
            itemIcons[i].gameObject.SetActive(false);
            itemIcons[i].sprite = null;
            itemCounts[i].text = "";
        }

        equippedWeaponItem = null;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        weaponManager = FindObjectOfType<WeaponManager>();

        if (weaponManager != null && equippedWeaponItem != null)
        {
            weaponManager.EquipWeapon(equippedWeaponItem.weaponPrefab, equippedWeaponItem);
        }
    }

    public bool HasItem(ItemData targetItem)
    {
        foreach (SlotData slot in slots)
        {
            if (slot.item == targetItem)
            {
                return true;
            }
        }
        return false;
    }
    
    public void ConsumeKeyItem()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item.itemType == ItemType.Key)
            {
                slots[i].count--;
                if (slots[i].count <= 0)
                {
                    slots[i].item = null;
                    itemIcons[i].gameObject.SetActive(false);
                    itemIcons[i].sprite = null;
                    itemCounts[i].text = "";
                }
                else
                {
                    UpdateSlotUI(i);
                }
                Debug.Log("[Inventory] 문 열림 시 → 키 소모 완료!");
                break;
            }
        }
    }


}