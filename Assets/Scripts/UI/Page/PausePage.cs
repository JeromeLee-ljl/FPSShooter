using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePage : PageManager.PageElement
{
    public Animator amAnimator;
    public Text scoreText;

    public override void OnHide()
    {
        base.OnHide();
        Cursor.lockState = CursorLockMode.Locked;
        GameProcessManager.Instance.ResumeGame();
        amAnimator.SetBool("Open", false);
    }

    public override void OnShow(params object[] datas)
    {
        base.OnShow(datas);
        GameProcessManager.Instance.PauseGame();
        amAnimator.SetBool("Open", true);
//        int score = -1;
//        if (datas?.Length > 0)
//            score = (int) datas[0];
        scoreText.text = $"分数：{GameProcessManager.Instance.score}";
        Cursor.lockState = CursorLockMode.None;
    }

    #region click event

    public void ClickResumeGame()
    {
        PageManager.Instance.Close<PausePage>();
    }

    public void ClickRestartGame()
    {
        GameProcessManager.Instance.StartGame();
    }

    public void ClickEndGame()
    {
        GameProcessManager.Instance.BackMainPage();
        Cursor.lockState = CursorLockMode.None;
    }

    #endregion
}