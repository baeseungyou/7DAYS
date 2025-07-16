using UnityEngine;
using UnityEngine.UI;

public class DayDisplay : MonoBehaviour
{
    public Text dayText;

    void Start()
    {
        UpdateDayText(GameManager.Instance.nowDay);
    }

    public void UpdateDayText(int day)
    {
        if (dayText != null)
        {
            dayText.text = day + " Day";
        }
    }
}
