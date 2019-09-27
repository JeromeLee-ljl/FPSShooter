using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPage : PageManager.PageElement
{
    public Text helpText;
    public Text introduceText;

    public override void OnShow(params object[] datas)
    {
        base.OnShow(datas);
        var data = ConfigsManager.Instance.GetConfig<JsonConfig<PagesConfig>>().Data.helpPage;
        helpText.text = data.help;
        introduceText.text = data.introduce;
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public void ClickReturnMainPage()
    {
        MainPage.Open("Return form Help Page", true);
    }
}