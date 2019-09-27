using System;
using System.Collections.Generic;
using UnityEngine;

/// 状态机中的状态类：保存可以转换到的其他状态  
public abstract class FSMState
{
    private HashSet<Type> transitions = new HashSet<Type>();
    private FSMSystem fsmSystem;
    /// 所属的状态机 不需要设置   添加到状态机时会自动设置
    public FSMSystem FsmSystem
    {
        get => fsmSystem;
        set
        {
            if (fsmSystem != null) return;
            fsmSystem = value;
        }
    }

    /// 添加到T状态的转换  
    public FSMState AddTransitionTo<T>() where T : FSMState
    {
        if (typeof(T) == GetType())
            Debug.LogError("FSMState ERROR: 不能添加到自身的转换");
        else if (ContainsTransitionTo<T>())
            Debug.LogError($"FSMState ERROR: 状态 {this} 已经包含了 到状态 {typeof(T)} 的转换，不可添加已存在的转换");
        else
            transitions.Add(typeof(T));
        return this;
    }

    /// 删除到T状态的转换
    public void DeleteTransitionTo<T>() where T : FSMState
    {
        if (!ContainsTransitionTo<T>())
            Debug.LogError($"FSMState ERROR:  状态 {this} 到状态 {typeof(T)} 的转换不存在, 不可删除不存在的转换");
        else
            transitions.Remove(typeof(T));
    }

    /// 是否包含到状态T的转换
    public bool ContainsTransitionTo<T>() where T : FSMState
    {
        return transitions.Contains(typeof(T));
    }

    /// 进入状态之前执行  
    public virtual void OnStateEnter()
    {
//        Debug.Log($"进入{this}状态--------");
    }

    /// 离开状态之前执行  
    public virtual void OnStateExit()
    {
//        Debug.Log($"离开{this}状态--------");
    }

    /// 状态转换条件  决定触发哪个转换
    public abstract void Reason();
    
    /// 在当前状态下的行为 
    public abstract void Update();
}