using System;
using System.Collections;
using System.IO;
using dreamcube.unity.Core.Scripts.Signals.Events;
using Serilog;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    protected AudioSource AudioSource;
    [SerializeField] protected VideoPlayer player;
    [SerializeField] protected VideoClip fallbackClip;

    [SerializeField] protected bool shouldAutoPlay;
    [SerializeField] protected bool shouldLoop;

    protected virtual void Awake()
    {
        Log.Debug("Video Controller awake");

        if (player == null)
            player = gameObject.GetComponent<VideoPlayer>();

        player.Stop();
        player.playOnAwake = false;
        player.isLooping = true;
        player.loopPointReached += VideoEnded;

        player.errorReceived += VideoErrorReceived;

        AudioSource = gameObject.AddComponent<AudioSource>();

        // https://forum.unity.com/threads/video-player-is-not-playing-audio.486924/
        player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        player.controlledAudioTrackCount = 1;
        player.EnableAudioTrack(0, true);
        player.SetTargetAudioSource(0, AudioSource);
    }

    private void VideoErrorReceived(VideoPlayer source, string message)
    {
        Log.Warning($"Video load from URL failed, switching to local");
        //Debug.LogWarning($"Video load from URL failed, switching to local");
        SetPlayerURL(String.Empty);
    }

    public virtual void SetPlayerURL(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Log.Warning($"<color=red>{player.gameObject.name} video URL was empty, referring to fallback clip</color>");
            player.source = VideoSource.VideoClip;
            player.clip = fallbackClip;
        }
        else
        {
            player.source = VideoSource.Url;
            player.url = url;
        }
        if (isActiveAndEnabled && !player.isPlaying)
        {
            Play();
        }
    }


    protected virtual void OnEnable()
    {
        if (shouldAutoPlay)
        {
            //Log.Debug($"Video Controller AUTO-playing with url: {player.url}");
            Play();
        }
    }
    private void OnDisable()
    {
        player.Stop();
    }

    public virtual void Play()
    {
        //Log.Debug($"Video Controller playing with url: {player.url}");
        if ( string.IsNullOrEmpty(player.url) )
        {
            Log.Warning($"<color=red>{player.gameObject.name} video URL was empty</color>");
            //VideoEnded(player);
            //return;
        }

        StartCoroutine(PrepareAndPlayVideo());
    }

    private IEnumerator PrepareAndPlayVideo()
    {
        player.Prepare();
        while (!player.isPrepared) yield return new WaitForEndOfFrame();
        player.Play();
        Log.Debug($"Video Controller sending VIDEO_START event with player name: {player.name}");
    }

   

    protected virtual void VideoEnded(VideoPlayer endedPlayer)
    {
        if (shouldLoop == false)
        {
            endedPlayer.Stop();
            EventManager.Instance.TriggerEvent(EventStrings.EventOnVideoEnded, player.name, obj:gameObject );
            Log.Debug($"Video Controller sending VIDEO_END event with player name: {player.name}");
        }
    }
}