using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonEffect : MonoBehaviour
{
    public AudioClip clickSound;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (clickSound != null && ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlaySound(clickSound);
        }
    }
}