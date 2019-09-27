using UnityEngine;

/// <summary>
/// 继承自Config ，指定了配置文件保存的路径和名称， 并以json格式读取配置文件，并保存信息到T中
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonConfig<T> : Config where T : IJsonConfigEntity
{
    public override string FilePath => $"{Application.streamingAssetsPath}/Configs/{typeof(T)}.json";
    public T Data { get; private set; }

    public override void LoadConfigFromText(string text)
    {
        Data = JsonUtility.FromJson<T>(text);
//        Debug.Log($"LoadConfig: {Data}");
    }
}

public interface IJsonConfigEntity
{
}