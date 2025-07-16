using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour
{
    public GameObject gameInfoPanel;

    public void OnClickStart(){
        SceneManager.LoadScene("Day");
    }

    public void OnClickExplain(){
        gameInfoPanel.SetActive(true);
    }

    public void OnClickCloseExplain(){
        gameInfoPanel.SetActive(false);
    }

    public void OnClickQuit(){
        Application.Quit();
    }
}