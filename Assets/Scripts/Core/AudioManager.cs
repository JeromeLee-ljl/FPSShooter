using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private Dictionary<AudioSource, float> audioPitches = new Dictionary<AudioSource, float>();

    public void Play(AudioSource audioSource, AudioClip audioClip, float pitchRange)
    {
        if (audioSource.isPlaying && audioSource.clip == audioClip) return;
        float originalPitch = audioSource.pitch;
        if (audioPitches.ContainsKey(audioSource))
            originalPitch = audioPitches[audioSource];
        else
            audioPitches.Add(audioSource, audioSource.pitch);

        audioSource.pitch = originalPitch + Random.Range(-pitchRange, pitchRange);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void Play(AudioSource audioSource, float pitchRange)
    {
        Play(audioSource, audioSource.clip, pitchRange);
    }
}