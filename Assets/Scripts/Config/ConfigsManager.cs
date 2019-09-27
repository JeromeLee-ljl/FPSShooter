using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 读取并保存Config
/// </summary>
public class ConfigsManager : MonoBehaviour
{
    public static ConfigsManager Instance { get; private set; }

    private Dictionary<Type, Config> configDict = new Dictionary<Type, Config>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // 添加配置信息
        AddConfig<JsonConfig<WeaponsConfig>>();
        AddConfig<JsonConfig<MonstersConfig>>();
        AddConfig<JsonConfig<PagesConfig>>();
    }

    public void AddConfig<T>() where T : Config, new()
    {
        StartCoroutine(LoadConfig(new T()));
    }

    private IEnumerator LoadConfig(Config config)
    {
        var request = new UnityWebRequest(config.FilePath) {downloadHandler = new DownloadHandlerBuffer()};
        yield return request.SendWebRequest();
        config.LoadConfigFromText(request.downloadHandler.text);
        configDict.Add(config.GetType(), config);
    }

    public T GetConfig<T>() where T : Config
    {
        Type t = typeof(T);
        if (!configDict.ContainsKey(t))
        {
            Debug.LogError($"ConfigManager不存在{t}");
            return null;
        }

        return configDict[t] as T;
    }
}