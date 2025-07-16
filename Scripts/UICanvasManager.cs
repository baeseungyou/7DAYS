using UnityEngine;
using UnityEngine.SceneManagement;

public class UIInitializer : MonoBehaviour
{
    private static GameObject uiInstance;
    private static GameObject inventoryInstance;

    void Awake()
    {

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Day" || currentScene == "Night")
        {
            if (uiInstance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("UICanvas");
                if (prefab != null)
                {
                    uiInstance = Instantiate(prefab);
                    uiInstance.name = "UICanvas";
                    DontDestroyOnLoad(uiInstance);
                }
            }

            if (inventoryInstance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("InventoryCanvas");
                if (prefab != null)
                {
                    inventoryInstance = Instantiate(prefab);
                    inventoryInstance.name = "InventoryCanvas";
                    DontDestroyOnLoad(inventoryInstance);
                }
            }
        }
    }
}
