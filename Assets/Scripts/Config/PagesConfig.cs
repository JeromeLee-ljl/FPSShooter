using System;
/// <summary>
/// 页面配置信息的实体类
/// </summary>
public class PagesConfig : IJsonConfigEntity
{
    public HelpPageData helpPage;

    public override string ToString()
    {
        return $@"{GetType()}:{{
    helpPage: {helpPage}
}}";
    }
}

[Serializable]
public class HelpPageData
{
    public string help;
    public string introduce;

    public override string ToString()
    {
        return $@"{GetType()}: {{
    help: {help},
    introduce: {introduce}
}}";
    }
}