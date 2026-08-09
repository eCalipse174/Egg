using System;
using UnityEngine;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private VideoPlayer videoPlayer;

    private Action onCutsceneComplete;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        panel.SetActive(false);
    }

    public void Play(VideoClip clip, Action onComplete)
    {
        if (clip == null)
        {
            onComplete?.Invoke();
            return;
        }

        onCutsceneComplete = onComplete;

        panel.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.loopPointReached += HandlePlaybackFinished;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += HandlePrepared;
    }

    private void HandlePrepared(VideoPlayer source)
    {
        videoPlayer.prepareCompleted -= HandlePrepared;
        videoPlayer.Play();
    }

    private void HandlePlaybackFinished(VideoPlayer source)
    {
        videoPlayer.loopPointReached -= HandlePlaybackFinished;
        panel.SetActive(false);

        Action callback = onCutsceneComplete;
        onCutsceneComplete = null;
        callback?.Invoke();
    }
}