using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * 所有page Prefags放在Resources/UI/Page下
 */
public class PageManager : MonoBehaviour
{
    // 所有页面的抽象类
    public abstract class PageElement : MonoBehaviour
    {
        public virtual void OnShow(params object[] datas)
        {
            Debug.Log($"Opened Page: {this.GetType()}");
        }

        public virtual void OnHide()
        {
            Debug.Log($"Closed Page: {this.GetType()}");
        }
    }

    public static PageManager Instance { get; private set; }

    private readonly Dictionary<Type, PageElement> pageDict = new Dictionary<Type, PageElement>();

    public void Open<T>(bool closeOtherPage = false, params object[] datas) where T : PageElement
    {
        if (closeOtherPage) CloseAllPage();

        pageDict.TryGetValue(typeof(T), out var page);
        if (page == null)
        {
            // 页面未打开过 需要加载实例化
            page = CreatePage<T>();
            pageDict[typeof(T)] = page;
        }
        else
        {
            // 页面打开过
            page.gameObject.SetActive(true);
            page.transform.SetAsLastSibling();
        }

        page.OnShow(datas);
    }

    public void Close<T>() where T : PageElement
    {
        pageDict.TryGetValue(typeof(T), out var page);
        ClosePage(page);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    #region private method

    private void CloseAllPage()
    {
        foreach (var page in pageDict.Values)
            ClosePage(page);
    }

    private void ClosePage(PageElement page)
    {
        if (page == null) return;
        if (page.gameObject.activeInHierarchy) page.OnHide();
        page.gameObject.SetActive(false);
    }

    private Canvas canvas;

    // 从Resources/UI/Page加载并实例化页面
    private T CreatePage<T>() where T : PageElement
    {
        // page需要加载到canvas节点下    考虑到切换场景，需要重新获得canvas
        if (!canvas)
        {
            canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
            if (!canvas)
            {
                Debug.LogError("场景中没有Tag为MainCanvas的Canvas");
                return null;
            }
        }

        // 加载prefab
        T prefab = Resources.Load<T>($"UI/Page/{typeof(T)}");
        if (!prefab)
        {
            Debug.LogError($"UI/Page目录下没有{typeof(T)}预制体");
            return null;
        }

        // 实例化Page
        T page = Instantiate(prefab, canvas.transform);
        var transform1 = page.transform;
        transform1.localPosition = Vector3.zero;
        transform1.localScale = Vector3.one;

        return page;
    }

    #endregion
}