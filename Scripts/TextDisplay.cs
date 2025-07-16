using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    public GameObject panel;
    public Text messageText;

    private Coroutine activeRoutine;

    public void Show(string message, float duration = 5f)
    {
        if (panel == null || messageText == null) return;

        panel.SetActive(true);
        messageText.gameObject.SetActive(true);
        messageText.text = message;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(HideAfterDelay(duration));
    }

    IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        panel.SetActive(false);
        messageText.gameObject.SetActive(false);
        activeRoutine = null;
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
    }
}
