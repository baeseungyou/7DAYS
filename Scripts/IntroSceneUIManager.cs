using UnityEngine;

public class IntroSceneUIManager : MonoBehaviour
{
    void Start()
    {
        GameObject ui = GameObject.Find("UICanvas");
        if (ui != null) ui.SetActive(false);

        GameObject inventory = GameObject.Find("InventoryCanvas");
        if (inventory != null) inventory.SetActive(false);

        GameObject dayText = GameObject.Find("DayDisplay");
        if (dayText != null) dayText.SetActive(false);
    }
}
