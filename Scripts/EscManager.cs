using UnityEngine;
using UnityEngine.SceneManagement;

public class EscManager : MonoBehaviour
{
    public GameObject EscPanel;
    public AudioClip buttonSound;
    private AudioSource audioSource;
    private bool isEsced = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayButtonSound();

            if (!isEsced)
            {
                EscGame();
            }
            else
            {
                ReStartGame();
            }
        }
    }

    public void EscGame()
    {
        EscPanel.SetActive(true);
        Time.timeScale = 0f;
        isEsced = true;
    }

    public void ReStartGame()
    {
        PlayButtonSound();
        EscPanel.SetActive(false);
        Time.timeScale = 1f;
        isEsced = false;
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }
}