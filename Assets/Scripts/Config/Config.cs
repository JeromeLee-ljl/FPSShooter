using UnityEngine;
/// <summary>
/// 配置的基类 在子类中 指定配置文件路径 和 读取方法
/// </summary>
public abstract class Config
{
    public virtual string FilePath => $"{Application.streamingAssetsPath}/Configs/{this}.json";

    public abstract void LoadConfigFromText(string text);
}


