using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Food,
    Weapon,
    Key
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    
    public int healAmount;
    public int damage;
    public GameObject weaponPrefab;

    public Vector3 equipPosition = Vector3.zero;
    public Vector3 equipRotation = Vector3.zero;
    public Vector3 equipScale = Vector3.one;

    public AudioClip equipSound;
    public AudioClip eatSound;

    public bool isKey;

    public string pickupMessage;
    public Sprite pickupImage;
}
