using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcessManager : MonoBehaviour
{
    public static GameProcessManager Instance { get; private set; }
    public float score;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        oldTimeScale = Time.timeScale;
    }

    private float oldTimeScale;
    public bool Pausing { get; private set; }

    public void PauseGame()
    {
        if(Pausing) return;
        Pausing = true;
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Pausing = false;
        Time.timeScale = oldTimeScale;
    }


    public void StartGame()
    {
        ResumeGame();
        SceneManager.LoadScene("GameScene");
        score = 0;
    }

    public void EndGame()
    {
        PauseGame();
        PageManager.Instance.Open<SettlePage>();
    }

    public void BackMainPage()
    {
        ResumeGame();
        SceneManager.LoadScene("MainPage");
        PageManager.Instance.Open<MainPage>();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}