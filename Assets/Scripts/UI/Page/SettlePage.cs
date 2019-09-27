using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlePage : PageManager.PageElement
{
    public Text scoreText;

    public override void OnHide()
    {
        base.OnHide();
        GameProcessManager.Instance.ResumeGame();
    }

    public override void OnShow(params object[] datas)
    {
        base.OnShow(datas);
        GameProcessManager.Instance.PauseGame();
//        int score = -1;
//        if (datas?.Length > 0)
//            score = (int) datas[0];
        scoreText.text = $"分数：{GameProcessManager.Instance.score}";
    }
    

    #region click event

    public void ClickRestartGame()
    {
        GameProcessManager.Instance.StartGame();
    }

    public void ClickEndGame()
    {
        GameProcessManager.Instance.BackMainPage();
    }

    #endregion
}