using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public int unlockDay = 1;  // 이 무기가 몇 일차에 등장할지 설정
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        Refresh(); // 시작할 때 한 번 적용
    }

    public void Refresh()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager.nowDay >= unlockDay)
        {
            gameObject.SetActive(true); // 등장 가능일 이상이면 활성화
        }
        else
        {
            gameObject.SetActive(false); // 아직 이르면 비활성화
        }
    }
}
