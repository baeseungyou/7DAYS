using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingPanelUI : MonoBehaviour
{
    public AudioClip buttonSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnReplayClick()
    {
        PlayButtonSound();
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }

        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.ResetInventory();
        }

        SceneManager.LoadScene("Day");
    }

    public void OnQuitClick()
    {
        PlayButtonSound();
        Application.Quit();
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }
}
