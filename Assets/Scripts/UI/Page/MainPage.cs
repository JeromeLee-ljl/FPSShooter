using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;


// todo 不用继承PageElement
public class MainPage : PageManager.PageElement
{
    public Text title;

    public override void OnShow(params object[] datas)
    {
        base.OnShow(datas);
        if (datas?.Length > 0)
        {
//            title.text = datas[0] as string;
        }
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public static void Open(string title, bool closeOtherPage = false)
    {
        PageManager.Instance.Open<MainPage>(closeOtherPage, title);
    }

    #region click event

    public void ClickStartGame()
    {
        GameProcessManager.Instance.StartGame();
    }

    public void ClickOpenHelpPage()
    {
        PageManager.Instance.Open<HelpPage>(true);
    }

    public void ClickExitGame()
    {
        GameProcessManager.Instance.ExitGame();
    }

    #endregion
}