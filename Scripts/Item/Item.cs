using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public AudioClip pickupSound;

    void Start()
    {
        if (data.isKey && InventoryUI.Instance.HasItem(data))
        {
            gameObject.SetActive(false);
        }
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventoryUI.Instance != null)
            {
                InventoryUI.Instance.AddItem(data);

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}
