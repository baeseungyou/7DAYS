using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform handTransform;
    public AudioSource audioSource;

    private GameObject equippedWeapon;
    private ItemData equippedItemData;


    // 무기 장착 -> 이전 무기를 반환
    public ItemData EquipWeapon(GameObject weaponPrefab, ItemData itemData)
    {
        ItemData previousItem = equippedItemData;

        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon);
        }

        equippedWeapon = Instantiate(weaponPrefab, handTransform);

        // 무기별 위치, 회전, 크기 조정 적용
        equippedWeapon.transform.localPosition = itemData.equipPosition;
        equippedWeapon.transform.localRotation = Quaternion.Euler(itemData.equipRotation);
        equippedWeapon.transform.localScale = itemData.equipScale;

        equippedItemData = itemData;

        if (itemData.equipSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(itemData.equipSound);
        }

        return previousItem;
    }
    
    public GameObject GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public ItemData GetEquippedItemData()
    {
        return equippedItemData;
    }

}
