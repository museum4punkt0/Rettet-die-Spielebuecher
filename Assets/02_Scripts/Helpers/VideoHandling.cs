using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoHandling : MonoBehaviour
{
    [SerializeField] private string videoName;
    [SerializeField] private float timeToStartPhasingIn;
    
    public GameObject PlateToShowOnVideoEnd;
    public CanvasGroup canvasGroup;

    private VideoPlayer videoPlayer;
    private bool endReached;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.url =  System.IO.Path.Combine (Application.streamingAssetsPath, videoName);
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }

    private void Update()
    {
        if (!endReached)
        {
            canvasGroup.alpha = Mathf.Clamp01((float)videoPlayer.time - timeToStartPhasingIn);
            if (canvasGroup.alpha > 0.0)
            {
                PlateToShowOnVideoEnd.SetActive(true);
            }
        }
        else
        {
            canvasGroup.alpha = 1.0f;
            gameObject.SetActive(false);
        }
    }

    void EndReached(VideoPlayer vp)
    {
        endReached = true;
    }
}