using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoPlayer : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public VideoClip firstClip;
    public VideoClip secondClip;
    public string nextSceneName = "MainMenu";

    private bool isSecondClipPlaying = false;

    void Start()
    {
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoEnd;

        // 첫 번째 영상 시작
        PlayVideo(firstClip);
    }

    void PlayVideo(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoPlayer.Prepare();
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSecondClipPlaying)
        {
            // 두 번째 영상으로 전환
            isSecondClipPlaying = true;
            PlayVideo(secondClip);
        }
        else
        {
            // 두 번째 영상도 끝나면 씬 전환!
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
